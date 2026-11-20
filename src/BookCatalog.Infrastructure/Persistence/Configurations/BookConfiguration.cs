using BookCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookCatalog.Infrastructure.Persistence.Configurations;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books", t =>
        {
            t.HasCheckConstraint("CK_Books_Title_NotEmpty", "LEN([Title]) > 0");
            t.HasCheckConstraint("CK_Books_Author_Min2Chars", "LEN([Author]) >= 2");
            t.HasCheckConstraint("CK_Books_Year_MinValue", "[Year] >= -3500");
        });

        // DB enforces fixed invariants. So year <= nextyear stays in validation. it depends on current state, db clock would differ from timeprovider.
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title).HasMaxLength(200);
        builder.Property(b => b.Author).HasMaxLength(100);
    }
}