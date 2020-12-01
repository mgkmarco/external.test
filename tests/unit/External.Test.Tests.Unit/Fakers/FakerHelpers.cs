using Bogus;
using External.Test.Contracts.Commands;
using External.Test.Contracts.Enums;
using External.Test.Contracts.Models;
using External.Test.Data.Contracts.Entities;
using System;
using System.Collections.Generic;

namespace External.Test.Tests.Unit.Fakers
{
    public class FakerHelpers
    {
        private static string[] _selections = { "over", "under", "1", "X", "2", "Goal", "No Goal" };
        private static string[] _markets = { "Over and Under", "1X2", "Goal No Goal" };

        public static UpdateMarketCommand FakeUpdateMarketCommand(int selectionCount)
        {
            var updateMarketCommand = new Faker<UpdateMarketCommand>()
                .RuleFor(x => x.CorrelationId, Guid.NewGuid)
                .RuleFor(x => x.MatchId, f => f.Random.Number(1, 1000))
                .RuleFor(x => x.MarketId, f => f.Random.Number(1, 1000))
                .RuleFor(x => x.MarketState, f => f.PickRandom<MarketState>())
                .RuleFor(x => x.MarketType, f => f.PickRandom(_markets))
                .RuleFor(x => x.Selections, f => FakeUpdateMarketSelectionCommand(selectionCount))
                .Generate();

            return updateMarketCommand;
        }

        public static UpdateMarketSuccessEvent FakeUpdateMarketSuccessEvent(Guid correlationId, int selectionCount)
        {
            var updateMarketSuccessEvent = new Faker<UpdateMarketSuccessEvent>()
                .RuleFor(x => x.CorrelationId, correlationId)
                .RuleFor(x => x.MatchId, f => f.Random.Number(1, 1000))
                .RuleFor(x => x.MarketId, f => f.Random.Number(1, 1000))
                .RuleFor(x => x.MarketState, f => f.PickRandom<MarketState>())
                .RuleFor(x => x.MarketType, f => f.PickRandom(_markets))
                .RuleFor(x => x.Selections, f => FakeMarketUpdatedSelectionEvent(selectionCount))
                .Generate();

            return updateMarketSuccessEvent;
        }
        
        public static UpdateMarketFailedEvent FakeUpdateMarketFailedEvent(Guid correlationId, int selectionCount)
        {
            var updateMarketSuccessEvent = new Faker<UpdateMarketFailedEvent>()
                .RuleFor(x => x.CorrelationId, correlationId)
                .RuleFor(x => x.MatchId, f => f.Random.Number(1, 1000))
                .RuleFor(x => x.MarketId, f => f.Random.Number(1, 1000))
                .RuleFor(x => x.MarketState, f => f.PickRandom<MarketState>())
                .RuleFor(x => x.MarketType, f => f.PickRandom(_markets))
                .RuleFor(x => x.Selections, f => FakeMarketUpdatedSelectionEvent(selectionCount))
                .Generate();

            return updateMarketSuccessEvent;
        }
        
        public static MarketUpdateEntity FakeMarketUpdateEntity(Guid correlationId, int selectionCount)
        {
            var updateMarketSuccessEvent = new Faker<MarketUpdateEntity>()
                .RuleFor(x => x.Id, correlationId)
                .RuleFor(x => x.CreatedAt, DateTime.UtcNow)
                .RuleFor(x => x.MatchId, f => f.Random.Number(1, 1000))
                .RuleFor(x => x.MarketId, f => f.Random.Number(1, 1000))
                .RuleFor(x => x.MarketState, f => f.Random.Int(0, 1))
                .RuleFor(x => x.MarketType, f => f.PickRandom(_markets))
                .RuleFor(x => x.Selections, f => FakeMarketSelectionEntity(selectionCount))
                .Generate();

            return updateMarketSuccessEvent;
        }

        private static IEnumerable<UpdateMarketSelectionCommand> FakeUpdateMarketSelectionCommand(int count = 1)
        {
            var updateMarketSelectionCommands = new List<UpdateMarketSelectionCommand>();
            
            for(int i = 0; i < count; i++)
            {
                updateMarketSelectionCommands.Add(new Faker<UpdateMarketSelectionCommand>()
                    .RuleFor(x => x.Name, f => f.PickRandom(_selections))
                    .RuleFor(x => x.Price, f => f.Random.Double(1, 20))
                    .Generate());
            }

            return updateMarketSelectionCommands;
        }
        
        private static IEnumerable<MarketUpdatedSelectionEvent> FakeMarketUpdatedSelectionEvent(int count = 1)
        {
            var updateMarketSelectionEvents = new List<MarketUpdatedSelectionEvent>();
            
            for(int i = 0; i < count; i++)
            {
                updateMarketSelectionEvents.Add(new Faker<MarketUpdatedSelectionEvent>()
                    .RuleFor(x => x.Name, f => f.PickRandom(_selections))
                    .RuleFor(x => x.Price, f => f.Random.Double(1, 20))
                    .Generate());
            }

            return updateMarketSelectionEvents;
        }
        
        private static IEnumerable<MarketSelectionEntity> FakeMarketSelectionEntity(int count = 1)
        {
            var marketSelectionEntities = new List<MarketSelectionEntity>();
            
            for(int i = 0; i < count; i++)
            {
                marketSelectionEntities.Add(new Faker<MarketSelectionEntity>()
                    .RuleFor(x => x.Name, f => f.PickRandom(_selections))
                    .RuleFor(x => x.Price, f => f.Random.Double(1, 20))
                    .Generate());
            }

            return marketSelectionEntities;
        }
    }
}