using FeatureToggle.API.Identity;
﻿using CsvHelper;
using System.Globalization;
using FeatureToggle.Application.DTOs;
using FeatureToggle.Application.Requests.Queries.Log;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FeatureToggle.API.Controllers
{
    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [Route("api/[controller]")]
    [ApiController]
    public class LogController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<PaginatedLogListDTO> GetLogs([FromQuery] GetLogQuery query)
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            return await mediator.Send(query,cancellationToken);
        }


        [HttpGet("download-logs")]
        public async Task<FileContentResult> GetAllLogs()
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            return await mediator.Send(new GetAllLogsQuery(),cancellationToken);
        }

    }
}
