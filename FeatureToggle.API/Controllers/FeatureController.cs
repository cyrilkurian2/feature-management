using FeatureToggle.Application.DTOs;
using FeatureToggle.Application.Requests.Queries.Filter;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeatureToggle.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<PaginatedFeatureListDTO> GetFilteredFeatures([FromQuery] GetFilteredFeaturesQuery query)
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            return await mediator.Send(query,cancellationToken);
        }

    }
}
