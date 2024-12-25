namespace LibraryMangamentSystem.Model
{
    public class BorrowRecordDTO
    {
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
