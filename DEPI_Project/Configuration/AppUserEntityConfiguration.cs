using System.Reflection.Emit;
using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DEPI_Project.Configuration
{
    public class AppUserEntityConfiguration:IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("AspNetUsers");
        }

    }
}
