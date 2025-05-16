using System.Reflection.Emit;
using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DEPI_Project.Configuration
{
    internal class HolidayEntityConfiguration : IEntityTypeConfiguration<Holiday>
    {
        public void Configure(EntityTypeBuilder<Holiday> builder)
        {
           
        }

    }
}
