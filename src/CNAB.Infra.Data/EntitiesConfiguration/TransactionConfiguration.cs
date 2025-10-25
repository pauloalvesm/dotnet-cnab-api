using CNAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CNAB.Infra.Data.EntitiesConfiguration;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("Id");

        builder.Property(t => t.Type)
            .IsRequired()
            .HasColumnName("Type")
            .HasConversion<int>();

        builder.Property(t => t.OccurrenceDate)
            .IsRequired()
            .HasColumnName("OccurrenceDate")
            .HasColumnType("date");

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnName("Amount")
            .HasColumnType("numeric(18, 2)");

        builder.Property(t => t.CPF)
            .IsRequired()
            .HasColumnName("CPF")
            .HasMaxLength(11);

        builder.Property(t => t.CardNumber)
            .IsRequired()
            .HasColumnName("CardNumber")
            .HasMaxLength(20);

        builder.Property(t => t.Time)
            .IsRequired()
            .HasColumnName("Time")
            .HasColumnType("time");
            
        builder.HasOne(t => t.Store)
            .WithMany(s => s.Transactions)
            .HasForeignKey("StoreId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}