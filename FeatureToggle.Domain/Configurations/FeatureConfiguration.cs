using System.Reflection.Emit;
using FeatureToggle.Domain.Entity.BusinessSchema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureToggle.Domain.Configurations
{
    public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
    {
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            builder.ToTable("Feature", "business");
            builder.HasKey(x => x.FeatureId);
            builder.Property(x => x.FeatureName).IsRequired()
                     .HasColumnType("nvarchar").HasMaxLength(50);


            builder.HasOne(f => f.FeatureType)
           .WithMany()
           .HasForeignKey(f => f.FeatureTypeId);
           
        }
    }
}
