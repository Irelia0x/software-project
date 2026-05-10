namespace LibraryManagementSystem.Models
{
    public enum BorrowStatus
    {
        Borrowed,
        Returned,
        Overdue,
        Lost
    }

    public class BorrowRecord
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(14);
        public DateTime? ReturnDate { get; set; }
        public BorrowStatus Status { get; set; } = BorrowStatus.Borrowed;
        public decimal FineAmount { get; set; } = 0;
        public string Notes { get; set; } = string.Empty;

        // Navigation properties (loaded separately)
        public Book? Book { get; set; }
        public Member? Member { get; set; }

        public bool IsOverdue => Status == BorrowStatus.Borrowed && DateTime.Now > DueDate;
        public int DaysOverdue => IsOverdue ? (int)(DateTime.Now - DueDate).TotalDays : 0;

        // Fine is $0.50 per day overdue
        public decimal CalculatedFine => DaysOverdue * 0.50m;
    }
}
