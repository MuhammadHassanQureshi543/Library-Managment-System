namespace LibraryMangamentSystem.Model.Respository
{
    public class UserRepo : Common<User>, IUserRepo 
    {
        public UserRepo(DBContexts dbContext) : base(dbContext)
        {
        }
    }
}
