using AutoMapper;
using External.Test.Contracts.Commands;
using External.Test.Contracts.Models;
using External.Test.Data.Contracts.Entities;
using External.Test.Host.Contracts.Public.Models;
using System;

namespace External.Test.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MarketSelectionUpdateRequest, UpdateMarketSelectionCommand>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => Math.Round(src.Price, 2)))
                .ReverseMap();
            
            CreateMap<MarketUpdateRequest, UpdateMarketCommand>()
                .ReverseMap();    

            CreateMap<UpdateMarketSelectionCommand, MarketUpdateSelection>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => Math.Round(src.Price, 2)))
                .ReverseMap();
            
            CreateMap<UpdateMarketCommand, MarketUpdate>()
                .ReverseMap();
            
            CreateMap<UpdateMarketSelectionCommand, MarketSelectionEntity>()
                .ReverseMap();
            
            CreateMap<UpdateMarketCommand, MarketUpdateEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CorrelationId))
                .ReverseMap();
            
            CreateMap<UpdateMarketSelectionCommand, MarketUpdatedSelectionEvent>()
                .ReverseMap();     
            
            CreateMap<UpdateMarketCommand, UpdateMarketSuccessEvent>()
                .ReverseMap();
            
            CreateMap<UpdateMarketCommand, UpdateMarketFailedEvent>()
                .ReverseMap();
        }
    }
}