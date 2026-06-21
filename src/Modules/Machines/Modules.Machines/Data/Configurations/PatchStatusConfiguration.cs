using MIT.Modules.Machines.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MIT.Modules.Machines.Data.Configurations;

public sealed class PatchStatusConfiguration : IEntityTypeConfiguration<PatchStatus>
{
    public void Configure(EntityTypeBuilder<PatchStatus> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("PatchStatuses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MachineId).IsRequired();
        builder.Property(x => x.PendingUpdatesJson).HasColumnType("jsonb").HasDefaultValue("[]");
        builder.HasIndex(x => x.MachineId).IsUnique();
        builder.Ignore(x => x.DomainEvents);
    }
}
