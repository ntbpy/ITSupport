using MIT.Modules.Machines.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MIT.Modules.Machines.Data.Configurations;

public sealed class MachineConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("Machines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MachineName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.IpAddress).IsRequired().HasMaxLength(45);
        builder.Property(x => x.MacAddress).IsRequired().HasMaxLength(17);
        builder.HasIndex(x => x.MacAddress).IsUnique();
        builder.Property(x => x.OsVersion).HasMaxLength(100);
        builder.Property(x => x.CpuModel).HasMaxLength(200);
        builder.Property(x => x.AgentVersion).HasMaxLength(20);
        builder.Property(x => x.AssignedUser).HasMaxLength(200);
        builder.Property(x => x.Notes).HasMaxLength(4096);
        builder.Property(x => x.EncryptedApiKey).HasMaxLength(512);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.LastSeenAt);
        builder.Ignore(x => x.DomainEvents);
    }
}
