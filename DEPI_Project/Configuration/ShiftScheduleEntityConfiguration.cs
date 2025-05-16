using System.Reflection.Emit;
using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DEPI_Project.Configuration
{
    internal class ShiftScheduleEntityConfiguration : IEntityTypeConfiguration<ShiftSchedule>
    {
        public void Configure(EntityTypeBuilder<ShiftSchedule> builder)
        {
            builder
        .HasOne(s => s.Employee)
        .WithMany(e => e.ShiftSchedules)
        .HasForeignKey(s => s.EmployeeId)
        .OnDelete(DeleteBehavior.Restrict);

            builder
            .HasOne(s => s.Manager)
            .WithMany()
            .HasForeignKey(s => s.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
