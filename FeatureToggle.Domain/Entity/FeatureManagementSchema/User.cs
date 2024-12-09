using Microsoft.AspNetCore.Identity;

namespace FeatureToggle.Domain.Entity.FeatureManagementSchema
{
    public class User : IdentityUser
    {
        public new string Id
        {
            get => base.Id;
            private set => base.Id = value;
        }
        public new string UserName
        {
            get => base.UserName ?? string.Empty;
            private set => base.UserName = value;
        }
        public string Name { get; private set; } = string.Empty;
        public bool IsAdmin { get; private set; }

        public User() { }

        public User(string email, string name)
        {
            Email = email;
            Name = name;
            UserName = email;
        }
    }
}
