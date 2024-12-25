namespace LibraryMangamentSystem.Model.Respository
{
    public class borrowRepo : Common<BorrowRecords>, IBorrowRepo
    {
        public borrowRepo(DBContexts dbContext) : base(dbContext)
        {
        }
    }
}
