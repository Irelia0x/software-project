using Microsoft.Data.Sqlite;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class DatabaseContext
    {
        private readonly string _connectionString;

        public DatabaseContext(string dbPath = "library.db")
        {
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        private SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            return conn;
        }

        private void InitializeDatabase()
        {
            using var conn = GetConnection();

            // Books table
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Books (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Author TEXT NOT NULL,
                        ISBN TEXT UNIQUE,
                        Genre TEXT,
                        PublishedYear INTEGER,
                        TotalCopies INTEGER DEFAULT 1,
                        AvailableCopies INTEGER DEFAULT 1,
                        Publisher TEXT,
                        Description TEXT,
                        AddedDate TEXT DEFAULT CURRENT_TIMESTAMP
                    );";
                cmd.ExecuteNonQuery();
            }

            // Members table
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Members (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FirstName TEXT NOT NULL,
                        LastName TEXT NOT NULL,
                        Email TEXT UNIQUE,
                        Phone TEXT,
                        Address TEXT,
                        DateOfBirth TEXT,
                        MembershipStartDate TEXT,
                        MembershipEndDate TEXT,
                        MembershipType INTEGER DEFAULT 0,
                        IsActive INTEGER DEFAULT 1,
                        FineAmount REAL DEFAULT 0
                    );";
                cmd.ExecuteNonQuery();
            }

            // BorrowRecords table
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS BorrowRecords (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        BookId INTEGER NOT NULL,
                        MemberId INTEGER NOT NULL,
                        BorrowDate TEXT NOT NULL,
                        DueDate TEXT NOT NULL,
                        ReturnDate TEXT,
                        Status INTEGER DEFAULT 0,
                        FineAmount REAL DEFAULT 0,
                        Notes TEXT,
                        FOREIGN KEY(BookId) REFERENCES Books(Id),
                        FOREIGN KEY(MemberId) REFERENCES Members(Id)
                    );";
                cmd.ExecuteNonQuery();
            }

            SeedData(conn);
        }

        private void SeedData(SqliteConnection conn)
        {
            // Check if data already exists
            using var checkCmd = conn.CreateCommand();
            checkCmd.CommandText = "SELECT COUNT(*) FROM Books";
            var count = (long)(checkCmd.ExecuteScalar() ?? 0);
            if (count > 0) return;

            // Seed books
            var books = new[]
            {
                ("The Great Gatsby", "F. Scott Fitzgerald", "978-0743273565", "Classic", 1925, 3, "Scribner", "A story of the fabulously wealthy Jay Gatsby."),
                ("To Kill a Mockingbird", "Harper Lee", "978-0061935466", "Classic", 1960, 2, "HarperCollins", "The unforgettable novel of a childhood in a sleepy Southern town."),
                ("1984", "George Orwell", "978-0451524935", "Dystopian", 1949, 4, "Signet Classic", "A dystopian social science fiction novel."),
                ("Pride and Prejudice", "Jane Austen", "978-0141439518", "Romance", 1813, 2, "Penguin Classics", "A romantic novel of manners."),
                ("The Catcher in the Rye", "J.D. Salinger", "978-0316769174", "Fiction", 1951, 2, "Little, Brown", "Story of Holden Caulfield's experiences in New York City."),
                ("Harry Potter and the Philosopher's Stone", "J.K. Rowling", "978-0439708180", "Fantasy", 1997, 5, "Scholastic", "The first novel in the Harry Potter series."),
                ("The Lord of the Rings", "J.R.R. Tolkien", "978-0618640157", "Fantasy", 1954, 3, "Houghton Mifflin", "An epic high-fantasy novel."),
                ("The Alchemist", "Paulo Coelho", "978-0062315007", "Fiction", 1988, 4, "HarperOne", "A novel about a young Andalusian shepherd's journey."),
                ("Brave New World", "Aldous Huxley", "978-0060850524", "Dystopian", 1932, 3, "Harper Perennial", "A dystopian novel set in a futuristic World State."),
                ("The Da Vinci Code", "Dan Brown", "978-0307474278", "Thriller", 2003, 4, "Anchor", "A mystery thriller novel following symbologist Robert Langdon.")
            };

            foreach (var (title, author, isbn, genre, year, copies, publisher, desc) in books)
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO Books (Title, Author, ISBN, Genre, PublishedYear, TotalCopies, AvailableCopies, Publisher, Description)
                                    VALUES ($t, $a, $i, $g, $y, $tc, $ac, $p, $d)";
                cmd.Parameters.AddWithValue("$t", title);
                cmd.Parameters.AddWithValue("$a", author);
                cmd.Parameters.AddWithValue("$i", isbn);
                cmd.Parameters.AddWithValue("$g", genre);
                cmd.Parameters.AddWithValue("$y", year);
                cmd.Parameters.AddWithValue("$tc", copies);
                cmd.Parameters.AddWithValue("$ac", copies);
                cmd.Parameters.AddWithValue("$p", publisher);
                cmd.Parameters.AddWithValue("$d", desc);
                cmd.ExecuteNonQuery();
            }

            // Seed members
            var members = new[]
            {
                ("Alice", "Johnson", "alice@email.com", "555-0101", "123 Main St", "1990-05-15", 0),
                ("Bob", "Smith", "bob@email.com", "555-0102", "456 Oak Ave", "1985-08-22", 1),
                ("Carol", "Williams", "carol@email.com", "555-0103", "789 Pine Rd", "2000-03-10", 2),
                ("David", "Brown", "david@email.com", "555-0104", "321 Elm St", "1958-11-30", 3),
                ("Emma", "Davis", "emma@email.com", "555-0105", "654 Maple Dr", "1995-07-18", 0)
            };

            foreach (var (fn, ln, email, phone, addr, dob, mtype) in members)
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO Members (FirstName, LastName, Email, Phone, Address, DateOfBirth, MembershipStartDate, MembershipEndDate, MembershipType)
                                    VALUES ($fn, $ln, $e, $ph, $a, $dob, $ms, $me, $mt)";
                cmd.Parameters.AddWithValue("$fn", fn);
                cmd.Parameters.AddWithValue("$ln", ln);
                cmd.Parameters.AddWithValue("$e", email);
                cmd.Parameters.AddWithValue("$ph", phone);
                cmd.Parameters.AddWithValue("$a", addr);
                cmd.Parameters.AddWithValue("$dob", dob);
                cmd.Parameters.AddWithValue("$ms", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("$me", DateTime.Now.AddYears(1).ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("$mt", mtype);
                cmd.ExecuteNonQuery();
            }

            // Seed borrow records (3 active, 1 overdue, 1 returned)
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var overdue = DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd");
            var future = DateTime.Now.AddDays(10).ToString("yyyy-MM-dd");
            var future2 = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
            var past = DateTime.Now.AddDays(-20).ToString("yyyy-MM-dd");
            var returned = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");

            // Active borrow: member 1 borrowed book 1, due in 10 days
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO BorrowRecords (BookId,MemberId,BorrowDate,DueDate,Status,Notes)
                                    VALUES (1,1,$bd,$dd,0,'')";
                cmd.Parameters.AddWithValue("$bd", today);
                cmd.Parameters.AddWithValue("$dd", future);
                cmd.ExecuteNonQuery();
                // Reduce available copies
                using var u = conn.CreateCommand();
                u.CommandText = "UPDATE Books SET AvailableCopies=AvailableCopies-1 WHERE Id=1";
                u.ExecuteNonQuery();
            }
            // Active borrow: member 2 borrowed book 3, due in 7 days
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO BorrowRecords (BookId,MemberId,BorrowDate,DueDate,Status,Notes)
                                    VALUES (3,2,$bd,$dd,0,'')";
                cmd.Parameters.AddWithValue("$bd", today);
                cmd.Parameters.AddWithValue("$dd", future2);
                cmd.ExecuteNonQuery();
                using var u = conn.CreateCommand();
                u.CommandText = "UPDATE Books SET AvailableCopies=AvailableCopies-1 WHERE Id=3";
                u.ExecuteNonQuery();
            }
            // Overdue borrow: member 3 borrowed book 2, was due 5 days ago
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO BorrowRecords (BookId,MemberId,BorrowDate,DueDate,Status,Notes)
                                    VALUES (2,3,$bd,$dd,0,'')";
                cmd.Parameters.AddWithValue("$bd", past);
                cmd.Parameters.AddWithValue("$dd", overdue);
                cmd.ExecuteNonQuery();
                using var u = conn.CreateCommand();
                u.CommandText = "UPDATE Books SET AvailableCopies=AvailableCopies-1 WHERE Id=2";
                u.ExecuteNonQuery();
            }
            // Returned borrow: member 1 borrowed and returned book 4
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO BorrowRecords (BookId,MemberId,BorrowDate,DueDate,ReturnDate,Status,Notes)
                                    VALUES (4,1,$bd,$dd,$rd,1,'')";
                cmd.Parameters.AddWithValue("$bd", past);
                cmd.Parameters.AddWithValue("$dd", DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("$rd", returned);
                cmd.ExecuteNonQuery();
            }
        }

        // ─── BOOK CRUD ───────────────────────────────────────────────────────────

        public List<Book> GetAllBooks()
        {
            var books = new List<Book>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Books ORDER BY Title";
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) books.Add(MapBook(reader));
            return books;
        }

        public List<Book> SearchBooks(string query)
        {
            var books = new List<Book>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Books 
                                WHERE Title LIKE $q OR Author LIKE $q OR ISBN LIKE $q OR Genre LIKE $q
                                ORDER BY Title";
            cmd.Parameters.AddWithValue("$q", $"%{query}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) books.Add(MapBook(reader));
            return books;
        }

        public Book? GetBookById(int id)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Books WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapBook(reader) : null;
        }

        public bool AddBook(Book book)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Books (Title, Author, ISBN, Genre, PublishedYear, TotalCopies, AvailableCopies, Publisher, Description)
                                VALUES ($t, $a, $i, $g, $y, $tc, $ac, $p, $d)";
            cmd.Parameters.AddWithValue("$t", book.Title);
            cmd.Parameters.AddWithValue("$a", book.Author);
            cmd.Parameters.AddWithValue("$i", book.ISBN);
            cmd.Parameters.AddWithValue("$g", book.Genre);
            cmd.Parameters.AddWithValue("$y", book.PublishedYear);
            cmd.Parameters.AddWithValue("$tc", book.TotalCopies);
            cmd.Parameters.AddWithValue("$ac", book.AvailableCopies);
            cmd.Parameters.AddWithValue("$p", book.Publisher);
            cmd.Parameters.AddWithValue("$d", book.Description);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateBook(Book book)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Books SET Title=$t, Author=$a, ISBN=$i, Genre=$g, PublishedYear=$y,
                                TotalCopies=$tc, AvailableCopies=$ac, Publisher=$p, Description=$d
                                WHERE Id=$id";
            cmd.Parameters.AddWithValue("$t", book.Title);
            cmd.Parameters.AddWithValue("$a", book.Author);
            cmd.Parameters.AddWithValue("$i", book.ISBN);
            cmd.Parameters.AddWithValue("$g", book.Genre);
            cmd.Parameters.AddWithValue("$y", book.PublishedYear);
            cmd.Parameters.AddWithValue("$tc", book.TotalCopies);
            cmd.Parameters.AddWithValue("$ac", book.AvailableCopies);
            cmd.Parameters.AddWithValue("$p", book.Publisher);
            cmd.Parameters.AddWithValue("$d", book.Description);
            cmd.Parameters.AddWithValue("$id", book.Id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteBook(int id)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Books WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        // ─── MEMBER CRUD ─────────────────────────────────────────────────────────

        public List<Member> GetAllMembers()
        {
            var members = new List<Member>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Members ORDER BY LastName, FirstName";
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) members.Add(MapMember(reader));
            return members;
        }

        public List<Member> SearchMembers(string query)
        {
            var members = new List<Member>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Members 
                                WHERE FirstName LIKE $q OR LastName LIKE $q OR Email LIKE $q OR Phone LIKE $q
                                ORDER BY LastName";
            cmd.Parameters.AddWithValue("$q", $"%{query}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) members.Add(MapMember(reader));
            return members;
        }

        public Member? GetMemberById(int id)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Members WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapMember(reader) : null;
        }

        public bool AddMember(Member member)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Members (FirstName, LastName, Email, Phone, Address, DateOfBirth, MembershipStartDate, MembershipEndDate, MembershipType, IsActive)
                                VALUES ($fn, $ln, $e, $ph, $a, $dob, $ms, $me, $mt, $ia)";
            cmd.Parameters.AddWithValue("$fn", member.FirstName);
            cmd.Parameters.AddWithValue("$ln", member.LastName);
            cmd.Parameters.AddWithValue("$e", member.Email);
            cmd.Parameters.AddWithValue("$ph", member.Phone);
            cmd.Parameters.AddWithValue("$a", member.Address);
            cmd.Parameters.AddWithValue("$dob", member.DateOfBirth.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$ms", member.MembershipStartDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$me", member.MembershipEndDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$mt", (int)member.MembershipType);
            cmd.Parameters.AddWithValue("$ia", member.IsActive ? 1 : 0);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateMember(Member member)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Members SET FirstName=$fn, LastName=$ln, Email=$e, Phone=$ph, Address=$a,
                                DateOfBirth=$dob, MembershipEndDate=$me, MembershipType=$mt, IsActive=$ia, FineAmount=$fa
                                WHERE Id=$id";
            cmd.Parameters.AddWithValue("$fn", member.FirstName);
            cmd.Parameters.AddWithValue("$ln", member.LastName);
            cmd.Parameters.AddWithValue("$e", member.Email);
            cmd.Parameters.AddWithValue("$ph", member.Phone);
            cmd.Parameters.AddWithValue("$a", member.Address);
            cmd.Parameters.AddWithValue("$dob", member.DateOfBirth.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$me", member.MembershipEndDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$mt", (int)member.MembershipType);
            cmd.Parameters.AddWithValue("$ia", member.IsActive ? 1 : 0);
            cmd.Parameters.AddWithValue("$fa", member.FineAmount);
            cmd.Parameters.AddWithValue("$id", member.Id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteMember(int id)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Members WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        // ─── BORROW RECORD OPERATIONS ─────────────────────────────────────────

        public List<BorrowRecord> GetAllBorrowRecords()
        {
            var records = new List<BorrowRecord>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT br.*, b.Title, b.Author, m.FirstName, m.LastName
                                FROM BorrowRecords br
                                JOIN Books b ON br.BookId = b.Id
                                JOIN Members m ON br.MemberId = m.Id
                                ORDER BY br.BorrowDate DESC";
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) records.Add(MapBorrowRecord(reader));
            return records;
        }

        public List<BorrowRecord> GetActiveBorrows()
        {
            var records = new List<BorrowRecord>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT br.*, b.Title, b.Author, m.FirstName, m.LastName
                                FROM BorrowRecords br
                                JOIN Books b ON br.BookId = b.Id
                                JOIN Members m ON br.MemberId = m.Id
                                WHERE br.Status = 0 OR br.Status = 2
                                ORDER BY br.DueDate ASC";
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) records.Add(MapBorrowRecord(reader));
            return records;
        }

        public List<BorrowRecord> GetMemberBorrowHistory(int memberId)
        {
            var records = new List<BorrowRecord>();
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT br.*, b.Title, b.Author, m.FirstName, m.LastName
                                FROM BorrowRecords br
                                JOIN Books b ON br.BookId = b.Id
                                JOIN Members m ON br.MemberId = m.Id
                                WHERE br.MemberId = $mid
                                ORDER BY br.BorrowDate DESC";
            cmd.Parameters.AddWithValue("$mid", memberId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) records.Add(MapBorrowRecord(reader));
            return records;
        }

        public int GetMemberActiveBorrowCount(int memberId)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM BorrowRecords WHERE MemberId=$mid AND (Status=0 OR Status=2)";
            cmd.Parameters.AddWithValue("$mid", memberId);
            return (int)(long)(cmd.ExecuteScalar() ?? 0);
        }

        public bool BorrowBook(int bookId, int memberId, DateTime dueDate, string notes = "")
        {
            using var conn = GetConnection();
            using var transaction = conn.BeginTransaction();
            try
            {
                // Update available copies
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = "UPDATE Books SET AvailableCopies = AvailableCopies - 1 WHERE Id = $id AND AvailableCopies > 0";
                    cmd.Parameters.AddWithValue("$id", bookId);
                    if (cmd.ExecuteNonQuery() == 0) { transaction.Rollback(); return false; }
                }

                // Create borrow record
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = @"INSERT INTO BorrowRecords (BookId, MemberId, BorrowDate, DueDate, Status, Notes)
                                        VALUES ($bid, $mid, $bd, $dd, 0, $n)";
                    cmd.Parameters.AddWithValue("$bid", bookId);
                    cmd.Parameters.AddWithValue("$mid", memberId);
                    cmd.Parameters.AddWithValue("$bd", DateTime.Now.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("$dd", dueDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("$n", notes);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch { transaction.Rollback(); return false; }
        }

        public bool ReturnBook(int borrowRecordId, decimal fineAmount = 0)
        {
            using var conn = GetConnection();
            using var transaction = conn.BeginTransaction();
            try
            {
                int bookId = 0;
                int memberId = 0;

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = "SELECT BookId, MemberId FROM BorrowRecords WHERE Id = $id";
                    cmd.Parameters.AddWithValue("$id", borrowRecordId);
                    using var reader = cmd.ExecuteReader();
                    if (!reader.Read()) { transaction.Rollback(); return false; }
                    bookId = reader.GetInt32(0);
                    memberId = reader.GetInt32(1);
                }

                // Update borrow record
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = @"UPDATE BorrowRecords SET ReturnDate=$rd, Status=1, FineAmount=$fa WHERE Id=$id";
                    cmd.Parameters.AddWithValue("$rd", DateTime.Now.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("$fa", fineAmount);
                    cmd.Parameters.AddWithValue("$id", borrowRecordId);
                    cmd.ExecuteNonQuery();
                }

                // Restore available copy
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = "UPDATE Books SET AvailableCopies = AvailableCopies + 1 WHERE Id = $id";
                    cmd.Parameters.AddWithValue("$id", bookId);
                    cmd.ExecuteNonQuery();
                }

                // Add fine to member if any
                if (fineAmount > 0)
                {
                    using var cmd = conn.CreateCommand();
                    cmd.Transaction = transaction;
                    cmd.CommandText = "UPDATE Members SET FineAmount = FineAmount + $fa WHERE Id = $mid";
                    cmd.Parameters.AddWithValue("$fa", fineAmount);
                    cmd.Parameters.AddWithValue("$mid", memberId);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch { transaction.Rollback(); return false; }
        }

        // ─── DASHBOARD STATS ─────────────────────────────────────────────────────

        public (int TotalBooks, int TotalMembers, int ActiveBorrows, int OverdueBooks, decimal TotalFines) GetDashboardStats()
        {
            using var conn = GetConnection();

            int totalBooks = 0, totalMembers = 0, activeBorrows = 0, overdueBooks = 0;
            decimal totalFines = 0;

            using (var cmd = conn.CreateCommand()) { cmd.CommandText = "SELECT COUNT(*) FROM Books"; totalBooks = (int)(long)(cmd.ExecuteScalar() ?? 0L); }
            using (var cmd = conn.CreateCommand()) { cmd.CommandText = "SELECT COUNT(*) FROM Members WHERE IsActive=1"; totalMembers = (int)(long)(cmd.ExecuteScalar() ?? 0L); }
            using (var cmd = conn.CreateCommand()) { cmd.CommandText = "SELECT COUNT(*) FROM BorrowRecords WHERE Status=0 OR Status=2"; activeBorrows = (int)(long)(cmd.ExecuteScalar() ?? 0L); }
            using (var cmd = conn.CreateCommand()) { cmd.CommandText = "SELECT COUNT(*) FROM BorrowRecords WHERE (Status=0 OR Status=2) AND DueDate < date('now')"; overdueBooks = (int)(long)(cmd.ExecuteScalar() ?? 0L); }
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COALESCE(SUM(FineAmount),0) FROM Members";
                var raw = cmd.ExecuteScalar();
                totalFines = raw == null ? 0m : Convert.ToDecimal(raw);
            }

            return (totalBooks, totalMembers, activeBorrows, overdueBooks, totalFines);
        }

        // ─── MAPPERS ─────────────────────────────────────────────────────────────

        private static Book MapBook(SqliteDataReader r) => new Book
        {
            Id = r.GetInt32(0),
            Title = r.GetString(1),
            Author = r.GetString(2),
            ISBN = r.IsDBNull(3) ? "" : r.GetString(3),
            Genre = r.IsDBNull(4) ? "" : r.GetString(4),
            PublishedYear = r.IsDBNull(5) ? 0 : r.GetInt32(5),
            TotalCopies = r.GetInt32(6),
            AvailableCopies = r.GetInt32(7),
            Publisher = r.IsDBNull(8) ? "" : r.GetString(8),
            Description = r.IsDBNull(9) ? "" : r.GetString(9)
        };

        private static Member MapMember(SqliteDataReader r) => new Member
        {
            Id = r.GetInt32(0),
            FirstName = r.GetString(1),
            LastName = r.GetString(2),
            Email = r.IsDBNull(3) ? "" : r.GetString(3),
            Phone = r.IsDBNull(4) ? "" : r.GetString(4),
            Address = r.IsDBNull(5) ? "" : r.GetString(5),
            DateOfBirth = r.IsDBNull(6) ? DateTime.MinValue : DateTime.Parse(r.GetString(6)),
            MembershipStartDate = r.IsDBNull(7) ? DateTime.Now : DateTime.Parse(r.GetString(7)),
            MembershipEndDate = r.IsDBNull(8) ? DateTime.Now.AddYears(1) : DateTime.Parse(r.GetString(8)),
            MembershipType = r.IsDBNull(9) ? MembershipType.Standard : (MembershipType)r.GetInt32(9),
            IsActive = r.IsDBNull(10) ? true : r.GetInt32(10) == 1,
            FineAmount = r.IsDBNull(11) ? 0 : (decimal)r.GetDouble(11)
        };

        private static BorrowRecord MapBorrowRecord(SqliteDataReader r)
        {
            var record = new BorrowRecord
            {
                Id = r.GetInt32(0),
                BookId = r.GetInt32(1),
                MemberId = r.GetInt32(2),
                BorrowDate = DateTime.Parse(r.GetString(3)),
                DueDate = DateTime.Parse(r.GetString(4)),
                ReturnDate = r.IsDBNull(5) ? null : DateTime.Parse(r.GetString(5)),
                Status = (BorrowStatus)r.GetInt32(6),
                FineAmount = r.IsDBNull(7) ? 0 : (decimal)r.GetDouble(7),
                Notes = r.IsDBNull(8) ? "" : r.GetString(8)
            };

            // Joined fields
            if (r.FieldCount > 9)
            {
                record.Book = new Book { Id = record.BookId, Title = r.IsDBNull(9) ? "" : r.GetString(9), Author = r.IsDBNull(10) ? "" : r.GetString(10) };
                record.Member = new Member { Id = record.MemberId, FirstName = r.IsDBNull(11) ? "" : r.GetString(11), LastName = r.IsDBNull(12) ? "" : r.GetString(12) };
            }

            return record;
        }
    }
}
