using MIT.Modules.Machines.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MIT.Modules.Machines.Data.Configurations;

public sealed class MachineCommandConfiguration : IEntityTypeConfiguration<MachineCommand>
{
    public void Configure(EntityTypeBuilder<MachineCommand> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("MachineCommands");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MachineId).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.PayloadJson).HasColumnType("jsonb");
        builder.Property(x => x.OutputJson).HasColumnType("jsonb");
        builder.Property(x => x.ErrorMessage).HasMaxLength(4096);
        builder.HasIndex(x => new { x.MachineId, x.Status });
        builder.Ignore(x => x.DomainEvents);
    }
}
