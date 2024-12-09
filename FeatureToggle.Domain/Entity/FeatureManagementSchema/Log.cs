using FeatureToggle.Domain.Entity.Enum;

namespace FeatureToggle.Domain.Entity.FeatureManagementSchema
{
    public class Log(string userId, int featureId, string featureName, int? businessId, string? businessName, Actions action)
    {
        public int Id { get; private set; }
        public string UserId { get; private set; } = userId;
        public User User { get; private set; }
        public int FeatureId { get; private set; } = featureId;
        public string FeatureName { get; private set; } = featureName;
        public int? BusinessId { get; private set; } = businessId;

        public string? BusinessName { get; private set; } = businessName;
        public DateTime Time { get; private set; } = DateTime.UtcNow;
        public Actions Action { get; private set; } = action;
    }
}
