using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DEPI_Project.Configuration
{
    public class PerformanceEntityConfguration : IEntityTypeConfiguration<Performance>
    {
        public void Configure(EntityTypeBuilder<Performance> builder)
        {
            builder
                .HasKey(P => P.PerformanceId);

            builder
                .HasOne(M => M.Manager)
                .WithMany(P => P.AssignedPerformances)
                .HasForeignKey(M => M.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(E => E.Employee)
                .WithMany()
                .HasForeignKey(E => E.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(H => H.AppUser)
                .WithMany()
                .HasForeignKey(H => H.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(T => T.Task)
                .WithOne(P => P.Performance)
                .HasForeignKey<Performance>(P => P.TaskId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
