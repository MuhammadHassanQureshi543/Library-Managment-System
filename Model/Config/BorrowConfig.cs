using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMangamentSystem.Model.Config
{
    public class BorrowConfig : IEntityTypeConfiguration<BorrowRecords>
    {
        public void Configure(EntityTypeBuilder<BorrowRecords> builder)
        {
            builder.ToTable("borrowRecords");

            builder.HasKey(br => br.BorrowId);

            builder.HasOne(br => br.User)
                   .WithMany(u => u.BorrowRecords)
                   .HasForeignKey(br => br.UserId)
                   .IsRequired();

            builder.HasOne(br => br.Books)
                   .WithMany(b => b.BorrowRecords)
                   .HasForeignKey(br => br.BookId)
                   .IsRequired();

            builder.Property(br => br.BorrowDate)
                   .IsRequired();

            builder.Property(br => br.ReturnDate)
                   .IsRequired(false); 
        }
    }
}
