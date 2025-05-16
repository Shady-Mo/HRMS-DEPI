using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DEPI_Project.Configuration
{
    public class TaskEntityConfiguration : IEntityTypeConfiguration<ProjectTask>
    {

        public void Configure(EntityTypeBuilder<ProjectTask> builder)
        {

            builder
                .HasKey(T => T.TaskId);


            builder
                 .HasOne(P => P.Project)
                 .WithMany(p => p.Tasks)
                 .HasForeignKey(P => P.ProjectId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(E => E.Employee)
                .WithMany(E => E.AssignedTasks)
                .HasForeignKey(E => E.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(M => M.Manager)
                .WithMany(M => M.ManagedTasks)
                .HasForeignKey(M => M.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);





        }

    }

}
