# External Test

1. [ Problem Definition ](#probdef)
2. [ Instructions ](#instructions)
3. [ Requirements ](#requirements)
4. [ Tech Stack ](#tech-stack)
5. [ High Level Architecture Design ](#high-level-arch)
6. [ Achieving Requirements ](#achieve-req)
    6.1 [ Able to process the updates received as fast as possible; to not pileup ](#first-req)
    6.2 [ Processing updates for the same entity identifier in order ](#second-req)
    6.3 [ Minimizing the chance of losing an update and not processing it ](#third-req)
    6.4 [ System is to be highly available, ideally having no Single-Point-Of-Failure and able to recover fromfaults should one specific service instance crash ](#fourth-req)
    6.5 [ Scaling (vertically/horizontally) with minimal effort (ideally no code changes) ](#fifth-req)
    

<a name="desc"></a>
## 1. Description


<a name="probdef"></a>
## Problem Definition

Design a system which is able to receive market updates (structure shown below) and process them as fast as possible. 
The updates will be received on a REST API you control, the API should offload the market update to other services for processing otherwise it will not keep up. 
The processing per update involves deserialization, saving data to a data store of your choice, and once saved publish a message indicating that it was done. 
The API will be receiving multiple updates for the same entity identifiers thus it is important that updates for the same entity identifier are processed in order, failure to do so will result on the wrong odds showing on the site.

Example Market Update Structure:
- Market ID (Entity Identifier) – 123
- Market Type – “Match Result”
- Market State - Open
- Market Selections:
    - Selection
        - Selection Name – “Home”
        - Selection Price – 1.6

<a name="instructions"></a>
## Instructions

Design the rest of the system to accomplish the requirement; you may use any technology/systems you are familiar with to create the best solution. Use any means familiar to you to convey the system design in a clear way, you will be presenting the design and providing reasoning/assumptions for the choices made and expected to defend them.

<a name="requirements"></a>
## Requirements

- Able to process the updates received as fast as possible; to not pileup
- Processing updates for the same entity identifier in order
- Minimizing the chance of losing an update and not processing it
- System is to be highly available, ideally having no Single-Point-Of-Failure and able to recover fromfaults should one specific service instance crash
- Scaling (vertically/horizontally) with minimal effort (ideally no code changes)

<a name="tech-stack"></a>
## Tech Stack

### Apache Kafka
Kafka has been chosen as the primary message broker notably for three reasons:
1. With the default scheme; any consumers within a consumer group will each be assigned a partition per topic. Thus, no competing consumers for a given partition. This essentially result in inherent message ordering. In the implementation, the partition key used is the MarketId
2. Kafka with replication and message persistence can offer out-of-the-box HA (high availability) and also the ability to persist and replay messages at a later stage 
3. Kafka consumers are stateless. Offsets are kept on the broker, therefore in case of crashes, elected consumers for the partition can pick up from the last offset that has been committed

### MongoDB
The reasons for selecting MongoDB as the main datastore are twofold: 
1. Better performance when compared to a traditional RDBMS when performing inserts since no relational constraints checks overheads. Given that our system is more insert heavy (this is an assumption I am doing)
2. The ability of not having a rigid schema. Some markets have different selections, thus I believe that a schema-less document database is more adequate for the task at hand

<a name="high-level-arch"></a>
## High Level Architecture Design
![HighLevel](highlevel.png)

<a name="achieve-req"></a>
## Achieving Requirements

<a name="first-req"></a>
### Able to process the updates received as fast as possible; to not pileup
The Web REST API is designed to be a thin 'layer' which will attempt to produce the messages immediately and return 202 Accepted to the caller in order to offload processing responsibility to the background consuming services. Thus, response times are kept to the minimum in order not to block any upstream calling services/processes. 

<a name="second-req"></a>
### Processing updates for the same entity identifier in order
As explained in the motivation for choosing Kafka as the message broker; Order is guaranteed by: 
- Producing messages to a topic, using Market Identifier as the partitioning key
- No retry policies on the producing part (the web api) in order to avoid any race-conditions
- All consumers form part of the same consumer group. Therefore no competing consumers per group, and thus each consumer will consume messages in-order from the respective partition
- Idempotency. From both kafka and mongo/datastore perspective

The image below is a screenshot from the Control Centre which shows that a key (the partition key which under the hood we specify it to be the MarketId), is always mapped to the same partition. Therefore, since each partition will be consumed solely by a single consumer in a consuming group, order is guaranteed.
![Ordering](ordering.png)

<a name="third-req"></a>
### Minimizing the chance of losing an update and not processing it
- Producer will wait for ACK.ALL, meaning that if a delivery report returns without error, the message has been replicated to all replicas in the in-sync replica set
- Retry policy with jittered exponential back-off on the consumer part to mitigate transient faults 

<a name="fourth-req"></a>
### System is to be highly available, ideally having no Single-Point-Of-Failure and able to recover from faults should one specific service instance crash
- Health checks for external system dependencies
- Given a consumer group leader within a consuming group, a rebalance will happen under the hood (done inherently by kafka) whenever one consumer crashes or leaves the group 
- If for any reason the database instance is down or un-reachable, kafka out-of-the-box persists messages for a duration of time (configurable). This enables business to replay events

<a name="fifth-req"></a>
### Scaling (vertically/horizontally) with minimal effort (ideally no code changes)
- Containerization. As shown in the “Load Tests” sections below, I was able to scale my application by having a LB in front as an ingress (I chose NGINX for simplicity, could be anything) running the following command on docker-compose: 

```bash
    docker-compose up -d --scale externalhost=5
```