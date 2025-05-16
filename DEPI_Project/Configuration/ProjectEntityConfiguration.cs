using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DEPI_Project.Configuration
{
    public class ProjectEntityConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder
                .HasKey(P => P.ProjectId);

            builder
                .HasOne(M => M.Manager)
                .WithMany(P => P.Projects)
                .HasForeignKey(M => M.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
