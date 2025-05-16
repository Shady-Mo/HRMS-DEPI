using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DEPI_Project.Configuration
{
    public class AttendanceEntityConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("Attendances");

            builder.HasKey(a => a.AttendanceId);

            builder.Property(a => a.AttendanceId)
                   .UseIdentityColumn(1, 1);

            builder.Property(a => a.Status)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmpId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.AppUser)
                .WithMany()
                .HasForeignKey(a => a.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
