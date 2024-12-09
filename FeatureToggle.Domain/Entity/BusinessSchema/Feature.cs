namespace FeatureToggle.Domain.Entity.BusinessSchema
{
    public class Feature
    {
        public int FeatureId { get;  }
        public string FeatureName { get; } = string.Empty;
        public List<BusinessFeatureFlag>? BusinessFeatures { get; }
        public FeatureType FeatureType { get; } = null! ;
        public int FeatureTypeId { get; }
    }
}
