namespace FeatureToggle.Domain.ConfigurationModels
{
    public class Authentication
    {
        public string JWTSecret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryTime { get; set; }
    }
}
