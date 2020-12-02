#External Test

1. [ Problem Definition ](#probdef)
2. [ Instructions ](#instructions)
2. [ Requirements ](#requirements)
2. [ Tech Stack ](#tech-stack)
<a name="desc"></a>
## 1. Description


<a name="probdef"></a>
##Problem Definition

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
##Instructions

Design the rest of the system to accomplish the requirement; you may use any technology/systems you are familiar with to create the best solution. Use any means familiar to you to convey the system design in a clear way, you will be presenting the design and providing reasoning/assumptions for the choices made and expected to defend them.

<a name="requirements"></a>
##Requirements

- Able to process the updates received as fast as possible; to not pileup
- Processing updates for the same entity identifier in order
- Minimizing the chance of losing an update and not processing it
- System is to be highly available, ideally having no Single-Point-Of-Failure and able to recover fromfaults should one specific service instance crash
- Scaling (vertically/horizontally) with minimal effort (ideally no code changes)

<a name="tech-stack"></a>
##Tech Stack

###Apache Kafka
Kafka has been chosen as the primary message broker notably for three reasons:
1. With the default scheme; any consumers within a consumer group will each be assigned a partition per topic. Thus, no competing consumers for a given partition. This essentially result in inherent message ordering. In the implementation, the partition key used is the MarketId
2. Kafka with replication and message persistence can offer out-of-the-box HA (high availability) and also the ability to persist and replay messages at a later stage 
3. Kafka consumers are stateless. Offsets are kept on the broker, therefore in case of crashes, elected consumers for the partition can pick up from the last offset that has been committed

###MongoDB
The reasons for selecting MongoDB as the main datastore are twofold: 
1. Better performance when compared to a traditional RDBMS when performing inserts since no relational constraints checks overheads. Given that our system is more insert heavy (this is an assumption I am doing)
2. The ability of not having a rigid schema. Some markets have different selections, thus I believe that a schema-less document database is more adequate for the task at hand 