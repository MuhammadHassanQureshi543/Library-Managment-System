namespace LibraryMangamentSystem.Model
{
    public class BooksDTO
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public bool IsAvailable { get; set; }
    }
}
