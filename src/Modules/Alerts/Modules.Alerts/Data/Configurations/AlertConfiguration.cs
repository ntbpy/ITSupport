using MIT.Modules.Alerts.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MIT.Modules.Alerts.Data.Configurations;

internal sealed class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("alerts");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.AlertType).HasMaxLength(64).IsRequired();
        builder.Property(a => a.Severity)
            .HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(a => a.Message).HasMaxLength(1000).IsRequired();
        builder.Property(a => a.SentViaJson).HasColumnType("jsonb").IsRequired();
        builder.HasIndex(a => a.MachineId);
        builder.HasIndex(a => a.SentAt);
        builder.HasIndex(a => a.AcknowledgedAt);
    }
}
