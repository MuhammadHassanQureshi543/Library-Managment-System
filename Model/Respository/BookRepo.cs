namespace LibraryMangamentSystem.Model.Respository
{
    public class BookRepo : Common<Books>, IBookRepo
    {
        public BookRepo(DBContexts dbContext) : base(dbContext)
        {
        }
    }
}
