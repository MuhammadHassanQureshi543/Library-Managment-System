using LibraryMangamentSystem.Model.Config;
using Microsoft.EntityFrameworkCore;

namespace LibraryMangamentSystem.Model
{
    public class DBContexts: DbContext
    {
        public DBContexts(DbContextOptions<DBContexts>options):base(options)
        {
                
        }
        public DbSet<User> users { get; set; }
        public DbSet<Books> books { get; set; }
        public DbSet<BorrowRecords> borrowRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new BookConfig());
            modelBuilder.ApplyConfiguration(new BorrowConfig());
        }
    }
}
