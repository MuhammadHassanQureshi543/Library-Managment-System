using System.ComponentModel.DataAnnotations;

namespace LibraryMangamentSystem.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public virtual ICollection<BorrowRecords> BorrowRecords { get; set; }
    }
}
