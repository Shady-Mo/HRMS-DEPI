using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DEPI_Project.Configuration
{
    public class CandidateEntityConfiguration:IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {


            builder
                .HasOne(h => h.AppUser)
                .WithMany(p => p.Candidates)
                .HasForeignKey(h => h.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(h => h.Job)
                .WithMany(p => p.Candidates)
                .HasForeignKey(h => h.JobId)
                .OnDelete(DeleteBehavior.Restrict);


        }

    }
}
