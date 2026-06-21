using MIT.Modules.Diagnostics.Contracts.Dtos;
using MIT.Modules.Diagnostics.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MIT.Modules.Diagnostics.Data.Configurations;

internal sealed class DiagnosticReportConfiguration : IEntityTypeConfiguration<DiagnosticReport>
{
    public void Configure(EntityTypeBuilder<DiagnosticReport> builder)
    {
        builder.ToTable("diagnostic_reports");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.MachineId).IsRequired();
        builder.Property(r => r.Severity)
            .HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(r => r.IssuesJson).HasColumnType("jsonb").IsRequired();
        builder.Property(r => r.FixesJson).HasColumnType("jsonb").IsRequired();
        builder.Property(r => r.AiSummary).HasMaxLength(1000);
        builder.HasIndex(r => r.MachineId);
        builder.HasIndex(r => r.AnalyzedAt);
    }
}
