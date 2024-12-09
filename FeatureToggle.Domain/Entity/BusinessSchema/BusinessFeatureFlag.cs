namespace FeatureToggle.Domain.Entity.BusinessSchema
{
    public class BusinessFeatureFlag
    {
        public int FeatureFlagId { get; private set; }
        public Business? Business { get; private set; }
        public int? BusinessId { get; private set; }
        public Feature? Feature { get; private set; }
        public int? FeatureId { get; private set; }

        public bool IsEnabled { get; private set; }
        private BusinessFeatureFlag()
        {
            
        }
        public BusinessFeatureFlag(Feature feature)
        {
            Business = null;
            Feature = feature;
            IsEnabled = true;
        }

        public BusinessFeatureFlag(Feature feature, Business business)
        {
            Business = business;
            Feature = feature;
            IsEnabled = true;
        }

        public void UpdateIsenabled (bool isEnabled)
        {
           IsEnabled = isEnabled;
        }



    }
}
