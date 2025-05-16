using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DEPI_Project.Configuration
{
    public class LeaveEntityConfiguration : IEntityTypeConfiguration<Leave>
    {
        public void Configure(EntityTypeBuilder<Leave> builder)
        {
            builder.ToTable("Leaves");

            builder.HasKey(l => l.LeaveId);

            builder.Property(l => l.LeaveId)
                   .UseIdentityColumn(1, 1);

            builder.Property(l => l.Reason)
                   .HasMaxLength(300)
                   .IsRequired();

            builder.Property(l => l.Status)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.HasOne(l => l.AppUser)
                .WithMany(h => h.Leaves)
                .HasForeignKey(l => l.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.EmpId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.AppUser)
                .WithMany()
                .HasForeignKey(l => l.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
