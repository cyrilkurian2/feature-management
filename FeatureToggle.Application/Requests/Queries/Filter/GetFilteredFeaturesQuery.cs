using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureToggle.Application.DTOs;
using MediatR;

namespace FeatureToggle.Application.Requests.Queries.Filter
{
    public class GetFilteredFeaturesQuery : IRequest<PaginatedFeatureListDTO>
    {
        public int PageNumber { get; set; }
        public string? SearchQuery { get; set; }
        public bool? FeatureToggleFilter { get; set; }
        public bool? ReleaseToggleFilter { get; set; }
        public bool? EnabledFilter { get; set; } 
        public bool? DisabledFilter { get; set; }
                
    }
}
