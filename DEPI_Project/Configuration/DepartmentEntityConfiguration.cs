using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DEPI_Project.Configuration
{
    public class DepartmentEntityConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder
                .ToTable("Departments");

            builder
                .HasKey(d => d.DepartmentId);

            builder
                .Property(d => d.DepartmentId)
                .UseIdentityColumn(1, 1);

            builder
                .Property(d => d.Name)
                .HasMaxLength(30)
                .IsRequired();

            builder
                .Property(d => d.Location)
                .HasMaxLength(150);

            builder
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DeptId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(d => d.Manager)
                .WithOne()
                .HasForeignKey<Department>(d => d.ManagerId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}
