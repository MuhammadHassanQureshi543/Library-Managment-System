namespace LibraryMangamentSystem.Model
{
    public class BorrowRecords
    {
        public int BorrowId { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int BookId { get; set; }
        public virtual Books Books { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }

    }
}
