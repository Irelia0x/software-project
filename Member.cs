namespace LibraryManagementSystem.Models
{
    public enum MembershipType
    {
        Standard,
        Premium,
        Student,
        Senior
    }

    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime MembershipStartDate { get; set; } = DateTime.Now;
        public DateTime MembershipEndDate { get; set; } = DateTime.Now.AddYears(1);
        public MembershipType MembershipType { get; set; } = MembershipType.Standard;
        public bool IsActive { get; set; } = true;
        public decimal FineAmount { get; set; } = 0;

        public string FullName => $"{FirstName} {LastName}";
        public bool MembershipExpired => DateTime.Now > MembershipEndDate;
        public int MaxBooksAllowed => MembershipType switch
        {
            MembershipType.Premium => 10,
            MembershipType.Student => 5,
            MembershipType.Senior => 7,
            _ => 3
        };

        public override string ToString() => $"{FullName} (ID: {Id})";
    }
}
