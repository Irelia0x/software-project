namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime AddedDate { get; set; } = DateTime.Now;

        public bool IsAvailable => AvailableCopies > 0;

        public override string ToString() => $"{Title} by {Author}";
    }
}
