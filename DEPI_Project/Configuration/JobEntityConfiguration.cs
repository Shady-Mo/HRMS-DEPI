using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DEPI_Project.Configuration
{
    public class JobEntityConfiguration:IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
          

            builder
                .HasOne(h => h.AppUser)
                .WithMany(p => p.Jobs)
                .HasForeignKey(h => h.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(h => h.Department)
                .WithMany(p => p.Jobs)
                .HasForeignKey(h => h.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
