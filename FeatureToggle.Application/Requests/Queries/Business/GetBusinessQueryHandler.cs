using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureToggle.Application.DTOs;
using FeatureToggle.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FeatureToggle.Application.Requests.Queries.Business
{
    public class GetBusinessQueryHandler(BusinessContext businessContext) : IRequestHandler<GetBusinessQuery, List<GetBusinessDTO>>
    {
        public async Task<List<GetBusinessDTO>> Handle(GetBusinessQuery request, CancellationToken cancellationToken)
        {
            if (request.FeatureStatus)
            {
                //Get Enabled 

                IQueryable<int?> businessesWithFlags = businessContext.BusinessFeatureFlag
                   .Where(bff => bff.FeatureId == request.FeatureId && (bff.IsEnabled == false || bff.BusinessId == null))
                   .Select(bff => bff.BusinessId);

                IQueryable<GetBusinessDTO> businessesWithoutFlags = businessContext.Business
                    .Where(b => !businessContext.BusinessFeatureFlag
                        .Where(bff => bff.FeatureId == request.FeatureId)
                        .Select(bff => bff.BusinessId)
                        .Contains(b.BusinessId))
                    .Select(b => new GetBusinessDTO
                    {
                        BusinessId = b.BusinessId,
                        BusinessName = b.BusinessName
                    });

                List<GetBusinessDTO> allBusinesses = await businessesWithoutFlags
                    .Union(
                        businessContext.Business
                            .Where(b => businessesWithFlags.Contains(b.BusinessId))
                            .Select(b => new GetBusinessDTO
                            {
                                BusinessId = b.BusinessId,
                                BusinessName = b.BusinessName
                            })
                    )
                    .ToListAsync(cancellationToken);

                return allBusinesses;

            }

            else
            {
                //Get Disabled business

                List<GetBusinessDTO> result = await businessContext.Business
                .Join(
                    businessContext.BusinessFeatureFlag.Where(ff => ff.FeatureId == request.FeatureId && ff.IsEnabled == true),
                    business => business.BusinessId,
                    featureFlag => featureFlag.BusinessId,
                    (business, featureFlag) => new GetBusinessDTO
                    {
                        BusinessId = business.BusinessId,
                        BusinessName = business.BusinessName
                    })
                .ToListAsync(cancellationToken);

                return result;
            }

            
        }
    }
}
