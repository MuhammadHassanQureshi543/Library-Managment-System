using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMangamentSystem.Model.Config
{
    public class BookConfig:IEntityTypeConfiguration<Books>
    {
        public void Configure(EntityTypeBuilder<Books> builder)
        {
            builder.ToTable("books");
            builder.HasKey(x => x.BookID);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Author).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Category).IsRequired().HasMaxLength(250);
            builder.Property(x => x.IsAvailable).IsRequired();
        }
    }
}
