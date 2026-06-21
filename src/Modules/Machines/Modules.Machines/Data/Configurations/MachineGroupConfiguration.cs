using MIT.Modules.Machines.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MIT.Modules.Machines.Data.Configurations;

public sealed class MachineGroupConfiguration : IEntityTypeConfiguration<MachineGroup>
{
    public void Configure(EntityTypeBuilder<MachineGroup> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("MachineGroups");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1024);
        builder.Property(x => x.AlertPolicyJson).HasColumnType("jsonb").HasDefaultValue("{}");
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Ignore(x => x.DomainEvents);
    }
}
