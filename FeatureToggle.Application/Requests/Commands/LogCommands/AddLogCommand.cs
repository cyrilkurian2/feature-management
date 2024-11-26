﻿using FeatureToggle.Domain.Entity.FeatureManagementSchema;
using MediatR;

namespace FeatureToggle.Application.Requests.Commands.LogCommands
{
    public class AddLogCommand : IRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int? BusinessId {  get; set; } 
        public string? BusinessName { get; set; }
        public int FeatureId { get; set; }
        public string FeatureName { get; set; } = string.Empty;
        public Actions Action { get; set; }
    }
}
