using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.Lab2.CoffeeStore.Repositories.Extensions;
using PRN232.Lab2.CoffeeStore.Repositories.Models;

namespace PRN232.Lab2.CoffeeStore.Repositories.Configurations
{
    public class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable("Menus");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .IsRequired();

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.FromDate)
                .IsRequired();

            builder.Property(m => m.ToDate)
                .IsRequired();
            builder.ConfigureAuditableEntity();

            builder.HasMany(m => m.ProductInMenus)
               .WithOne(pim => pim.Menu)
               .HasForeignKey(pim => pim.MenuId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.FromDate);
            builder.HasIndex(m => m.ToDate);
            builder.HasIndex(m => m.Name);
        }
    }
}