using FeatureToggle.Application.DTOs;
using FeatureToggle.Application.Requests.Queries.Filter;
using FeatureToggle.Domain.Entity.BusinessSchema;
using FeatureToggle.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetFilteredFeaturesQueryHandler(BusinessContext businessContext) : IRequestHandler<GetFilteredFeaturesQuery, PaginatedFeatureListDTO>
{
    private readonly BusinessContext _businessContext = businessContext;
    private int pageSize = 12;

    public async Task<PaginatedFeatureListDTO> Handle(GetFilteredFeaturesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Feature> baseQuery = _businessContext.Feature.Include(f => f.BusinessFeatures);

        
        if (!request.FeatureToggleFilter.HasValue && !request.ReleaseToggleFilter.HasValue &&
            !request.EnabledFilter.HasValue && !request.DisabledFilter.HasValue)
        {
            IQueryable<FilteredFeatureDTO> allFeatures =  baseQuery.Select(f => new FilteredFeatureDTO
            {
                FeatureFlagId = f.BusinessFeatures != null && f.BusinessFeatures.Any()
                    ? f.BusinessFeatures.FirstOrDefault()!.FeatureFlagId
                    : 0,
                FeatureId = f.FeatureId,
                FeatureName = f.FeatureName,
                FeatureType = f.FeatureTypeId,
                isEnabled = f.BusinessFeatures != null && f.BusinessFeatures.Any()
                    ? f.BusinessFeatures.FirstOrDefault()!.IsEnabled
                    : null
            });

            if(request.SearchQuery is not null)
            {
                string searchQuery = request.SearchQuery.ToLower();
                allFeatures = allFeatures.Where(af => EF.Functions.Like(af.FeatureName, $"%{searchQuery}%"));
            }

            List<FilteredFeatureDTO> allFeaturesList = await allFeatures.Skip(request.PageNumber * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return new PaginatedFeatureListDTO
            {
                FeatureCount = allFeatures.Count(),
                TotalPages = (allFeatures.Count() + pageSize - 1) / pageSize,
                PageSize = pageSize,
                FeatureList = allFeaturesList
            };
        }

        if (request.FeatureToggleFilter == true)
        {
            baseQuery = baseQuery.Where(f => f.FeatureTypeId == 2); 
        }

        if (request.ReleaseToggleFilter == true)
        {
            baseQuery = baseQuery.Where(f => f.FeatureTypeId == 1); 

            if (request.EnabledFilter == true && request.DisabledFilter == true)
            {
                baseQuery = baseQuery.Where(f =>
                    f.BusinessFeatures!.Any(bf => bf.IsEnabled == true) ||   
                    f.BusinessFeatures!.Any(bf => !bf.IsEnabled) ||  
                    !f.BusinessFeatures!.Any()                      
                );
            }
            else if (request.EnabledFilter == true)
            {
                baseQuery = baseQuery.Where(f =>
                    f.BusinessFeatures!.Any(bf => bf.IsEnabled == true)); 
            }
            else if (request.DisabledFilter == true)
            {
                baseQuery = baseQuery.Where(f =>
                    f.BusinessFeatures!.Any(bf => !bf.IsEnabled) ||      
                    !f.BusinessFeatures!.Any());                        
            }
        }

        IQueryable<FilteredFeatureDTO> combinedQuery = baseQuery.Select(f => new FilteredFeatureDTO
        {
            FeatureFlagId = f.BusinessFeatures != null && f.BusinessFeatures.Any()
                ? f.BusinessFeatures.First().FeatureFlagId
                : 0,
            FeatureId = f.FeatureId,
            FeatureName = f.FeatureName,
            FeatureType = f.FeatureTypeId,
            isEnabled = f.BusinessFeatures != null && f.BusinessFeatures.Any()
                ? f.BusinessFeatures.First().IsEnabled
                : null
        });

        if (request.SearchQuery is not null)
        {
            string searchQuery = request.SearchQuery.ToLower();
            combinedQuery = combinedQuery.Where(af => EF.Functions.Like(af.FeatureName, $"%{searchQuery}%"));
        }

        List<FilteredFeatureDTO> featureList = combinedQuery
            .Skip(request.PageNumber * pageSize)
            .Take(pageSize)
            .ToList();

        int totalCount = combinedQuery.Count();

        return new PaginatedFeatureListDTO
        {
            FeatureCount = totalCount,
            TotalPages = (totalCount + pageSize - 1) / pageSize,
            PageSize = pageSize,
            FeatureList = featureList
        };
    }
}