﻿using AutoMapper;
using External.Test.Contracts.Commands;
using External.Test.Contracts.Services;
using External.Test.Host.Contracts.Public.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace External.Test.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly IProducerService<int, UpdateMarketCommand> _producerService;
        private readonly IMapper _mapper;
        private readonly ILogger<MatchController> _logger;
        
        public MatchController(IProducerService<int, UpdateMarketCommand> producerService, IMapper mapper, ILogger<MatchController> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _producerService = producerService ?? throw new ArgumentNullException(nameof(producerService));
        }

        [HttpPost("{matchId}/market")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> UpdateMarketAsync([FromRoute] int matchId, [FromBody] MarketUpdateRequest marketUpdateRequest)
        {
            if (matchId <= 0)
            {
                return BadRequest("MatchId must be a valid integer greater than 0");
            }
            
            var request = _mapper.Map<UpdateMarketCommand>(marketUpdateRequest);
            request.MatchId = matchId;
            await _producerService.ProduceAsync(request.MarketId, request);

            return Accepted();
        }
    }
}