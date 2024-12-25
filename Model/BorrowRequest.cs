namespace LibraryMangamentSystem.Model
{
    public class BorrowRequest
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
    }
}
