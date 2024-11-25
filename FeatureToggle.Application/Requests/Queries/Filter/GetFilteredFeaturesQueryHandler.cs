﻿using FeatureToggle.Application.DTOs;
using FeatureToggle.Domain.Entity.BusinessSchema;
using FeatureToggle.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FeatureToggle.Application.Requests.Queries.Filter
{
    public class GetFilteredFeaturesQueryHandler(BusinessContext businessContext) : IRequestHandler<GetFilteredFeaturesQuery,PaginatedFeatureListDTO>
    {
        private readonly BusinessContext _businessContext = businessContext;
        private int pageSize = 12;
        public async Task<PaginatedFeatureListDTO> Handle(GetFilteredFeaturesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<BusinessFeatureFlag> query = _businessContext.BusinessFeatureFlag
                .Include(bf => bf.Feature)
                .ThenInclude(f => f.FeatureType)
                .AsQueryable();

            // Filter by enabled/disabled state
            if (request.IsEnabledFilter.HasValue && request.IsDisabledFilter.HasValue)
            {
                // Both Enabled and Disabled
            }
            else
            {
                if (request.IsEnabledFilter.HasValue)
                {
                    query = query.Where(bf => bf.IsEnabled == request.IsEnabledFilter.Value);
                }

                if (request.IsDisabledFilter.HasValue)
                {
                    query = query.Where(bf => bf.IsEnabled == !request.IsDisabledFilter.Value);
                }
            }

            // Filter by feature/release toggle type
            if (request.ReleaseToggleFilter.HasValue && request.FeatureToggleFilter.HasValue)
            {
                // Both Release and Feature
            }
            else
            {
                if (request.ReleaseToggleFilter.HasValue)
                {
                    query = query.Where(bf => bf.Feature.FeatureTypeId == 1);
                }

                if (request.FeatureToggleFilter.HasValue)
                {
                    query = query.Where(bf => bf.Feature.FeatureTypeId == 2);
                }
            }

            // Features in BusinessFeatureFlag
            IQueryable<FilteredFeatureDTO> featuresWithFlags = query.Select(bf => new FilteredFeatureDTO
            {
                FeatureFlagId = bf.FeatureFlagId,
                FeatureId = bf.FeatureId,
                FeatureName = bf.Feature.FeatureName,
                FeatureType = bf.Feature.FeatureTypeId,
                isEnabled = bf.IsEnabled
            });

            // release toggles in Feature but not in BusinessFeatureFlag
            if (request.ReleaseToggleFilter.HasValue || (!request.IsEnabledFilter.HasValue && !request.IsDisabledFilter.HasValue && !request.FeatureToggleFilter.HasValue))
            {
                IQueryable<FilteredFeatureDTO> releaseTogglesWithoutFlags = _businessContext.Feature
                    .Where(f => f.FeatureTypeId == 1)  // Only Release toggles
                    .GroupJoin(
                        _businessContext.BusinessFeatureFlag,
                        feature => feature.FeatureId,
                        businessFeatureFlag => businessFeatureFlag.FeatureId,
                        (feature, businessFeatureFlags) => new { Feature = feature, BusinessFeatureFlags = businessFeatureFlags }
                    )
                    .SelectMany(
                        result => result.BusinessFeatureFlags.DefaultIfEmpty(),
                        (result, businessFeatureFlag) => new
                        {
                            Feature = result.Feature,
                            BusinessFeatureFlag = businessFeatureFlag
                        }
                    )
                    .Where(result => result.BusinessFeatureFlag == null) // No flag in BusinessFeatureFlag
                    .Select(result => new FilteredFeatureDTO
                    {
                        FeatureFlagId = 0,  // No flag assigned
                        FeatureId = result.Feature.FeatureId,
                        FeatureName = result.Feature.FeatureName,
                        FeatureType = result.Feature.FeatureTypeId,
                        isEnabled = null
                    });

                featuresWithFlags = featuresWithFlags.Concat(releaseTogglesWithoutFlags);
            }

            // If 'FeatureToggleFilter' is set, include feature toggles that are in the Feature table but not in BusinessFeatureFlag
            if (request.FeatureToggleFilter.HasValue || (!request.IsEnabledFilter.HasValue && !request.IsDisabledFilter.HasValue))
            {
                IQueryable<FilteredFeatureDTO> featureTogglesWithoutFlags = _businessContext.Feature
                    .Where(f => f.FeatureTypeId == 2)  // Only Feature toggles
                    .GroupJoin(
                        _businessContext.BusinessFeatureFlag,
                        feature => feature.FeatureId,
                        businessFeatureFlag => businessFeatureFlag.FeatureId,
                        (feature, businessFeatureFlags) => new { Feature = feature, BusinessFeatureFlags = businessFeatureFlags }
                    )
                    .SelectMany(
                        result => result.BusinessFeatureFlags.DefaultIfEmpty(),
                        (result, businessFeatureFlag) => new
                        {
                            Feature = result.Feature,
                            BusinessFeatureFlag = businessFeatureFlag
                        }
                    )
                    .Where(result => result.BusinessFeatureFlag == null) // No flag in BusinessFeatureFlag
                    .Select(result => new FilteredFeatureDTO
                    {
                        FeatureFlagId = 0,  // No flag assigned
                        FeatureId = result.Feature.FeatureId,
                        FeatureName = result.Feature.FeatureName,
                        FeatureType = result.Feature.FeatureTypeId,
                        isEnabled = null
                    });

                featuresWithFlags = featuresWithFlags.Concat(featureTogglesWithoutFlags);
            }

            // If IsEnabledFilter is true, exclude release toggles that are present in the Feature table but not in the BusinessFeatureFlag table
            if (request.IsEnabledFilter.HasValue && request.IsEnabledFilter.Value)
            {
                featuresWithFlags = featuresWithFlags.Where(f => f.FeatureId != 0);  // Exclude toggles with FeatureFlagId == 0 (release toggles without flags)
            }

            // Combine results for both release toggles (with flags) and feature toggles (with or without flags)
            List<FilteredFeatureDTO> combinedQuery = await featuresWithFlags
                .GroupBy(f => f.FeatureId)
                .Select(x => x.First())     // Select the first feature for each unique FeatureId
                .ToListAsync(cancellationToken);

            if (request.SearchQuery is not null)
            {
                combinedQuery = combinedQuery.Where(cq => cq.FeatureName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            PaginatedFeatureListDTO paginatedFeatureList = new PaginatedFeatureListDTO()
            {
                FeatureCount = combinedQuery.Count(),
                TotalPages = ((combinedQuery.Count()) / pageSize) + 1,
                PageSize = pageSize,
                FeatureList = combinedQuery.Skip(pageSize*request.PageNumber).Take(pageSize).ToList(),
            };

            return paginatedFeatureList;
        }

    }
}
