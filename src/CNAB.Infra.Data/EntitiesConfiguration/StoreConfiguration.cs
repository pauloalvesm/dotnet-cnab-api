using CNAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CNAB.Infra.Data.EntitiesConfiguration;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Stores");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("Id");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasColumnName("Name")
            .HasMaxLength(100);

        builder.Property(s => s.OwnerName)
            .IsRequired()
            .HasColumnName("OwnerName")
            .HasMaxLength(100);

        builder.HasMany(s => s.Transactions)
            .WithOne(t => t.Store)
            .HasForeignKey("StoreId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}