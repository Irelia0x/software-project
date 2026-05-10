using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.UI
{
    public class MainForm : Form
    {
        private readonly DatabaseContext _db;
        private Panel _sidePanel = null!;
        private Panel _contentPanel = null!;
        private Label _titleLabel = null!;

        // Color palette
        private readonly Color PrimaryDark = Color.FromArgb(15, 23, 42);
        private readonly Color PrimaryBlue = Color.FromArgb(37, 99, 235);
        private readonly Color AccentGold = Color.FromArgb(245, 158, 11);
        private readonly Color SidebarBg = Color.FromArgb(22, 33, 62);
        private readonly Color CardBg = Color.FromArgb(30, 41, 59);
        private readonly Color TextLight = Color.FromArgb(226, 232, 240);
        private readonly Color TextMuted = Color.FromArgb(148, 163, 184);
        private readonly Color SuccessGreen = Color.FromArgb(34, 197, 94);
        private readonly Color DangerRed = Color.FromArgb(239, 68, 68);
        private readonly Color WarningOrange = Color.FromArgb(251, 146, 60);

        public MainForm()
        {
            _db = new DatabaseContext();
            InitializeComponent();
            ShowDashboard();
        }

        private void InitializeComponent()
        {
            this.Text = "📚 Library Management System";
            this.Size = new Size(1280, 800);
            this.MinimumSize = new Size(1024, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = PrimaryDark;
            this.ForeColor = TextLight;
            this.Font = new Font("Segoe UI", 9.5f);

            // ORDER MATTERS: Fill panels must be added before Left/Right docked panels
            BuildContentArea();
            BuildSidebar();
        }

        private void BuildSidebar()
        {
            _sidePanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 240,
                BackColor = SidebarBg,
                Padding = new Padding(0, 0, 0, 0)
            };

            // Logo area
            var logoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(15, 23, 42),
                Padding = new Padding(20, 0, 0, 0)
            };
            var logoLabel = new Label
            {
                Text = "📚 LibraSys",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            logoPanel.Controls.Add(logoLabel);
            _sidePanel.Controls.Add(logoPanel);

            // Nav buttons
            var navItems = new[]
            {
                ("🏠", "Dashboard", (Action)ShowDashboard),
                ("📖", "Books", (Action)ShowBooks),
                ("👥", "Members", (Action)ShowMembers),
                ("🔄", "Borrow / Return", (Action)ShowBorrows),
                ("⚠️", "Overdue", (Action)ShowOverdue),
                ("📊", "Reports", (Action)ShowReports)
            };

            var navContainer = new Panel
            {
                Dock = DockStyle.Top,
                Height = navItems.Length * 54 + 20,
                BackColor = SidebarBg,
                Padding = new Padding(12, 12, 12, 0)
            };

            int yPos = 12;
            foreach (var (icon, label, action) in navItems)
            {
                var btn = new Button
                {
                    Text = $"  {icon}  {label}",
                    ForeColor = TextMuted,
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10f),
                    Size = new Size(216, 44),
                    Location = new Point(0, yPos),
                    Cursor = Cursors.Hand,
                    Tag = action
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 80, 160);
                var capturedAction = action;
                btn.Click += (s, e) =>
                {
                    capturedAction.Invoke();
                    HighlightNavButton(btn, navContainer);
                };
                navContainer.Controls.Add(btn);
                yPos += 54;
            }

            _sidePanel.Controls.Add(navContainer);

            // Footer
            var footerLabel = new Label
            {
                Text = $"v1.0  |  {DateTime.Now.Year}",
                ForeColor = TextMuted,
                Font = new Font("Segoe UI", 8f),
                Dock = DockStyle.Bottom,
                Height = 36,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(15, 23, 42)
            };
            _sidePanel.Controls.Add(footerLabel);

            this.Controls.Add(_sidePanel);
        }

        private void HighlightNavButton(Button active, Panel container)
        {
            foreach (Control c in container.Controls)
            {
                if (c is Button b)
                {
                    b.ForeColor = TextMuted;
                    b.BackColor = Color.Transparent;
                    b.Font = new Font("Segoe UI", 10f);
                }
            }
            active.ForeColor = Color.White;
            active.BackColor = Color.FromArgb(37, 99, 235);
            active.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        }

        private void BuildContentArea()
        {
            // Top bar
            var topBar = new Panel
            {
                Name = "topBar",
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = SidebarBg,
                Padding = new Padding(24, 0, 24, 0)
            };
            _titleLabel = new Label
            {
                Text = "Dashboard",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                Dock = DockStyle.Left,
                AutoSize = false,
                Width = 400,
                TextAlign = ContentAlignment.MiddleLeft
            };
            var timeLabel = new Label
            {
                Text = DateTime.Now.ToString("dddd, MMMM dd yyyy"),
                ForeColor = TextMuted,
                Font = new Font("Segoe UI", 9.5f),
                Dock = DockStyle.Right,
                AutoSize = false,
                Width = 260,
                TextAlign = ContentAlignment.MiddleRight
            };
            topBar.Controls.Add(_titleLabel);
            topBar.Controls.Add(timeLabel);

            // Main content panel (Fill)
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = PrimaryDark,
                Padding = new Padding(20),
                AutoScroll = true
            };

            // Add Fill first, then Top — WinForms respects Z-order for docking
            this.Controls.Add(_contentPanel);
            this.Controls.Add(topBar);
        }

        private void SetTitle(string title) => _titleLabel.Text = title;

        private void ClearContent()
        {
            _contentPanel.Controls.Clear();
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  DASHBOARD
        // ═══════════════════════════════════════════════════════════════════════
        private void ShowDashboard()
        {
            ClearContent();
            SetTitle("Dashboard");

            var stats = _db.GetDashboardStats();
            var borrows = _db.GetActiveBorrows();

            var statCards = new[]
            {
                ("📚", "Total Books", stats.TotalBooks.ToString(), PrimaryBlue),
                ("👥", "Active Members", stats.TotalMembers.ToString(), SuccessGreen),
                ("🔄", "Active Borrows", stats.ActiveBorrows.ToString(), AccentGold),
                ("⚠️", "Overdue", stats.OverdueBooks.ToString(), DangerRed)
            };

            // Stat cards row
            var cardFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 120,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 12)
            };

            foreach (var (icon, label, value, color) in statCards)
            {
                var card = MakeStatCard(icon, label, value, color);
                cardFlow.Controls.Add(card);
            }

            // Recent borrows table
            var recentLabel = new Label
            {
                Text = "Recent & Active Borrows",
                ForeColor = TextLight,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = Color.Transparent
            };

            var grid = MakeDataGrid();
            grid.Dock = DockStyle.Fill;
            grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Width = 50 },
                new DataGridViewTextBoxColumn { HeaderText = "Book", Width = 220 },
                new DataGridViewTextBoxColumn { HeaderText = "Member", Width = 160 },
                new DataGridViewTextBoxColumn { HeaderText = "Borrow Date", Width = 120 },
                new DataGridViewTextBoxColumn { HeaderText = "Due Date", Width = 120 },
                new DataGridViewTextBoxColumn { HeaderText = "Status", Width = 100 }
            );

            foreach (var r in borrows)
            {
                var row = grid.Rows.Add(
                    r.Id,
                    r.Book?.Title ?? "-",
                    r.Member?.FullName ?? "-",
                    r.BorrowDate.ToString("MMM dd, yyyy"),
                    r.DueDate.ToString("MMM dd, yyyy"),
                    r.IsOverdue ? "⚠️ OVERDUE" : "✅ Active"
                );
                if (r.IsOverdue)
                    grid.Rows[row].DefaultCellStyle.ForeColor = DangerRed;
            }

            var tablePanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            tablePanel.Controls.Add(grid);
            tablePanel.Controls.Add(recentLabel);

            _contentPanel.Controls.Add(tablePanel);
            _contentPanel.Controls.Add(cardFlow);
        }

        private Panel MakeStatCard(string icon, string label, string value, Color accent)
        {
            var card = new Panel
            {
                Width = 190,
                Height = 100,
                BackColor = CardBg,
                Margin = new Padding(0, 0, 14, 0)
            };

            // Left accent bar
            var bar = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accent };

            // Content area with padding from bar
            var iconLbl = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 18f),
                ForeColor = accent,
                Location = new Point(16, 10),
                AutoSize = true
            };
            var valueLbl = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(16, 42),
                AutoSize = true
            };
            var nameLbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = TextMuted,
                Location = new Point(16, 74),
                AutoSize = true
            };
            card.Controls.AddRange(new Control[] { bar, iconLbl, valueLbl, nameLbl });
            return card;
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  BOOKS
        // ═══════════════════════════════════════════════════════════════════════
        private void ShowBooks()
        {
            ClearContent();
            SetTitle("Books");

            // Toolbar
            var toolbar = BuildToolbar("Search books…", out var searchBox);

            var addBtn = MakeButton("➕ Add Book", PrimaryBlue);
            addBtn.Location = new Point(295, 8);
            addBtn.Click += (s, e) => ShowAddEditBook(null);
            toolbar.Controls.Add(addBtn);

            var grid = MakeDataGrid();
            grid.Dock = DockStyle.Fill;
            grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Width = 50 },
                new DataGridViewTextBoxColumn { HeaderText = "Title", Width = 220 },
                new DataGridViewTextBoxColumn { HeaderText = "Author", Width = 160 },
                new DataGridViewTextBoxColumn { HeaderText = "Genre", Width = 100 },
                new DataGridViewTextBoxColumn { HeaderText = "Year", Width = 70 },
                new DataGridViewTextBoxColumn { HeaderText = "Available", Width = 90 },
                new DataGridViewTextBoxColumn { HeaderText = "Total", Width = 70 },
                new DataGridViewTextBoxColumn { HeaderText = "ISBN", Width = 140 }
            );

            Action loadBooks = () =>
            {
                grid.Rows.Clear();
                var query = searchBox.Text.Trim();
                var books = string.IsNullOrEmpty(query) ? _db.GetAllBooks() : _db.SearchBooks(query);
                foreach (var b in books)
                {
                    var row = grid.Rows.Add(b.Id, b.Title, b.Author, b.Genre, b.PublishedYear,
                        b.AvailableCopies, b.TotalCopies, b.ISBN);
                    if (!b.IsAvailable)
                        grid.Rows[row].DefaultCellStyle.ForeColor = DangerRed;
                    else if (b.AvailableCopies < b.TotalCopies)
                        grid.Rows[row].DefaultCellStyle.ForeColor = AccentGold;
                }
            };
            loadBooks();

            searchBox.TextChanged += (s, e) => loadBooks();

            grid.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                int id = (int)grid.Rows[e.RowIndex].Cells[0].Value;
                ShowAddEditBook(_db.GetBookById(id));
            };

            // Context menu
            var ctx = new ContextMenuStrip();
            ctx.Items.Add("✏️ Edit", null, (s, e) =>
            {
                if (grid.SelectedRows.Count == 0) return;
                int id = (int)grid.SelectedRows[0].Cells[0].Value;
                ShowAddEditBook(_db.GetBookById(id));
            });
            ctx.Items.Add("🗑️ Delete", null, (s, e) =>
            {
                if (grid.SelectedRows.Count == 0) return;
                int id = (int)grid.SelectedRows[0].Cells[0].Value;
                if (Confirm("Delete this book?")) { _db.DeleteBook(id); loadBooks(); }
            });
            grid.ContextMenuStrip = ctx;

            var wrapper = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            wrapper.Controls.Add(grid);
            wrapper.Controls.Add(toolbar);
            _contentPanel.Controls.Add(wrapper);
        }

        private void ShowAddEditBook(Book? book)
        {
            var form = new Form
            {
                Text = book == null ? "Add New Book" : "Edit Book",
                Size = new Size(520, 600),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = PrimaryDark,
                ForeColor = TextLight,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24) };

            var fields = new Dictionary<string, Control>
            {
                { "Title *", new TextBox { Text = book?.Title ?? "" } },
                { "Author *", new TextBox { Text = book?.Author ?? "" } },
                { "ISBN", new TextBox { Text = book?.ISBN ?? "" } },
                { "Genre", new TextBox { Text = book?.Genre ?? "" } },
                { "Publisher", new TextBox { Text = book?.Publisher ?? "" } },
                { "Published Year", new NumericUpDown { Minimum = 1000, Maximum = DateTime.Now.Year, Value = book?.PublishedYear > 0 ? book.PublishedYear : DateTime.Now.Year } },
                { "Total Copies", new NumericUpDown { Minimum = 1, Maximum = 999, Value = book?.TotalCopies > 0 ? book.TotalCopies : 1 } },
                { "Description", new TextBox { Text = book?.Description ?? "", Multiline = true, Height = 80 } }
            };

            int y = 16;
            foreach (var (lbl, ctrl) in fields)
            {
                var label = new Label { Text = lbl, ForeColor = TextMuted, Font = new Font("Segoe UI", 9f), Location = new Point(24, y), AutoSize = true };
                ctrl.Location = new Point(24, y + 20);
                ctrl.Width = 440;
                StyleInput(ctrl);
                panel.Controls.Add(label);
                panel.Controls.Add(ctrl);
                y += ctrl.Height + 36;
            }

            var saveBtn = MakeButton(book == null ? "✅ Add Book" : "💾 Save Changes", PrimaryBlue);
            saveBtn.Location = new Point(24, y);
            saveBtn.Width = 200;
            saveBtn.Click += (s, e) =>
            {
                var title = ((TextBox)fields["Title *"]).Text.Trim();
                var author = ((TextBox)fields["Author *"]).Text.Trim();
                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(author))
                { MessageBox.Show("Title and Author are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                int copies = (int)((NumericUpDown)fields["Total Copies"]).Value;
                var b = new Book
                {
                    Id = book?.Id ?? 0,
                    Title = title,
                    Author = author,
                    ISBN = ((TextBox)fields["ISBN"]).Text.Trim(),
                    Genre = ((TextBox)fields["Genre"]).Text.Trim(),
                    Publisher = ((TextBox)fields["Publisher"]).Text.Trim(),
                    PublishedYear = (int)((NumericUpDown)fields["Published Year"]).Value,
                    TotalCopies = copies,
                    AvailableCopies = book == null ? copies : book.AvailableCopies,
                    Description = ((TextBox)fields["Description"]).Text.Trim()
                };
                if (book == null) _db.AddBook(b); else _db.UpdateBook(b);
                form.Close();
                ShowBooks();
            };
            panel.Controls.Add(saveBtn);
            form.Controls.Add(panel);
            form.ShowDialog(this);
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  MEMBERS
        // ═══════════════════════════════════════════════════════════════════════
        private void ShowMembers()
        {
            ClearContent();
            SetTitle("Members");

            var toolbar = BuildToolbar("Search members…", out var searchBox);
            var addBtn = MakeButton("➕ Add Member", SuccessGreen);
            addBtn.Location = new Point(295, 8);
            addBtn.Click += (s, e) => ShowAddEditMember(null);
            toolbar.Controls.Add(addBtn);

            var grid = MakeDataGrid();
            grid.Dock = DockStyle.Fill;
            grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Width = 50 },
                new DataGridViewTextBoxColumn { HeaderText = "Name", Width = 180 },
                new DataGridViewTextBoxColumn { HeaderText = "Email", Width = 200 },
                new DataGridViewTextBoxColumn { HeaderText = "Phone", Width = 120 },
                new DataGridViewTextBoxColumn { HeaderText = "Membership", Width = 110 },
                new DataGridViewTextBoxColumn { HeaderText = "Expires", Width = 110 },
                new DataGridViewTextBoxColumn { HeaderText = "Fine", Width = 80 },
                new DataGridViewTextBoxColumn { HeaderText = "Status", Width = 80 }
            );

            Action loadMembers = () =>
            {
                grid.Rows.Clear();
                var query = searchBox.Text.Trim();
                var members = string.IsNullOrEmpty(query) ? _db.GetAllMembers() : _db.SearchMembers(query);
                foreach (var m in members)
                {
                    var row = grid.Rows.Add(m.Id, m.FullName, m.Email, m.Phone,
                        m.MembershipType.ToString(),
                        m.MembershipEndDate.ToString("MMM dd, yyyy"),
                        m.FineAmount > 0 ? $"${m.FineAmount:F2}" : "-",
                        m.IsActive ? "✅ Active" : "❌ Inactive");
                    if (!m.IsActive || m.MembershipExpired)
                        grid.Rows[row].DefaultCellStyle.ForeColor = DangerRed;
                    if (m.FineAmount > 0)
                        grid.Rows[row].Cells[6].Style.ForeColor = WarningOrange;
                }
            };
            loadMembers();
            searchBox.TextChanged += (s, e) => loadMembers();

            grid.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                int id = (int)grid.Rows[e.RowIndex].Cells[0].Value;
                ShowAddEditMember(_db.GetMemberById(id));
            };

            var wrapper = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            wrapper.Controls.Add(grid);
            wrapper.Controls.Add(toolbar);
            _contentPanel.Controls.Add(wrapper);
        }

        private void ShowAddEditMember(Member? member)
        {
            var form = new Form
            {
                Text = member == null ? "Add New Member" : "Edit Member",
                Size = new Size(520, 620),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = PrimaryDark,
                ForeColor = TextLight,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24) };

            var fnBox = new TextBox { Text = member?.FirstName ?? "" };
            var lnBox = new TextBox { Text = member?.LastName ?? "" };
            var emailBox = new TextBox { Text = member?.Email ?? "" };
            var phoneBox = new TextBox { Text = member?.Phone ?? "" };
            var addrBox = new TextBox { Text = member?.Address ?? "" };
            var dobPicker = new DateTimePicker { Value = member?.DateOfBirth > DateTime.MinValue ? member.DateOfBirth : new DateTime(1990, 1, 1), Format = DateTimePickerFormat.Short };
            var typeCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            typeCombo.Items.AddRange(Enum.GetNames(typeof(MembershipType)));
            typeCombo.SelectedIndex = member != null ? (int)member.MembershipType : 0;
            var endPicker = new DateTimePicker { Value = member?.MembershipEndDate > DateTime.Now ? member.MembershipEndDate : DateTime.Now.AddYears(1), Format = DateTimePickerFormat.Short };

            var controls = new (string label, Control ctrl)[]
            {
                ("First Name *", fnBox), ("Last Name *", lnBox), ("Email", emailBox),
                ("Phone", phoneBox), ("Address", addrBox), ("Date of Birth", dobPicker),
                ("Membership Type", typeCombo), ("Membership End Date", endPicker)
            };

            int y = 16;
            foreach (var (lbl, ctrl) in controls)
            {
                var label = new Label { Text = lbl, ForeColor = TextMuted, Font = new Font("Segoe UI", 9f), Location = new Point(24, y), AutoSize = true };
                ctrl.Location = new Point(24, y + 20);
                ctrl.Width = 440;
                StyleInput(ctrl);
                panel.Controls.Add(label);
                panel.Controls.Add(ctrl);
                y += 54;
            }

            var saveBtn = MakeButton(member == null ? "✅ Add Member" : "💾 Save", SuccessGreen);
            saveBtn.Location = new Point(24, y);
            saveBtn.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(fnBox.Text) || string.IsNullOrEmpty(lnBox.Text))
                { MessageBox.Show("First and Last name required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                var m = new Member
                {
                    Id = member?.Id ?? 0,
                    FirstName = fnBox.Text.Trim(),
                    LastName = lnBox.Text.Trim(),
                    Email = emailBox.Text.Trim(),
                    Phone = phoneBox.Text.Trim(),
                    Address = addrBox.Text.Trim(),
                    DateOfBirth = dobPicker.Value,
                    MembershipStartDate = member?.MembershipStartDate ?? DateTime.Now,
                    MembershipEndDate = endPicker.Value,
                    MembershipType = (MembershipType)typeCombo.SelectedIndex,
                    IsActive = true,
                    FineAmount = member?.FineAmount ?? 0
                };
                if (member == null) _db.AddMember(m); else _db.UpdateMember(m);
                form.Close();
                ShowMembers();
            };
            panel.Controls.Add(saveBtn);
            form.Controls.Add(panel);
            form.ShowDialog(this);
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  BORROWS
        // ═══════════════════════════════════════════════════════════════════════
        private void ShowBorrows()
        {
            ClearContent();
            SetTitle("Borrow / Return");

            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 8, 0, 8)
            };

            var borrowBtn = MakeButton("📤 Borrow Book", PrimaryBlue);
            borrowBtn.Location = new Point(0, 8);
            borrowBtn.Click += (s, e) => ShowBorrowDialog();
            toolbar.Controls.Add(borrowBtn);

            var returnBtn = MakeButton("📥 Return Book", SuccessGreen);
            returnBtn.Location = new Point(165, 8);
            returnBtn.Click += (s, e) => ShowReturnDialog();
            toolbar.Controls.Add(returnBtn);

            var grid = MakeDataGrid();
            grid.Dock = DockStyle.Fill;
            grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Width = 50 },
                new DataGridViewTextBoxColumn { HeaderText = "Book", Width = 220 },
                new DataGridViewTextBoxColumn { HeaderText = "Member", Width = 160 },
                new DataGridViewTextBoxColumn { HeaderText = "Borrowed", Width = 110 },
                new DataGridViewTextBoxColumn { HeaderText = "Due", Width = 110 },
                new DataGridViewTextBoxColumn { HeaderText = "Returned", Width = 110 },
                new DataGridViewTextBoxColumn { HeaderText = "Fine", Width = 80 },
                new DataGridViewTextBoxColumn { HeaderText = "Status", Width = 110 }
            );

            var records = _db.GetAllBorrowRecords();
            foreach (var r in records)
            {
                var statusText = r.Status switch
                {
                    BorrowStatus.Returned => "✅ Returned",
                    BorrowStatus.Overdue => "⚠️ Overdue",
                    BorrowStatus.Lost => "❌ Lost",
                    _ => r.IsOverdue ? "⚠️ Overdue" : "📖 Borrowed"
                };

                var row = grid.Rows.Add(r.Id, r.Book?.Title ?? "-", r.Member?.FullName ?? "-",
                    r.BorrowDate.ToString("MMM dd, yyyy"),
                    r.DueDate.ToString("MMM dd, yyyy"),
                    r.ReturnDate?.ToString("MMM dd, yyyy") ?? "-",
                    r.FineAmount > 0 ? $"${r.FineAmount:F2}" : "-",
                    statusText);

                if (r.IsOverdue)
                    grid.Rows[row].DefaultCellStyle.ForeColor = DangerRed;
                else if (r.Status == BorrowStatus.Returned)
                    grid.Rows[row].DefaultCellStyle.ForeColor = SuccessGreen;
            }

            var wrapper = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            wrapper.Controls.Add(grid);
            wrapper.Controls.Add(toolbar);
            _contentPanel.Controls.Add(wrapper);
        }

        private void ShowBorrowDialog()
        {
            var form = new Form
            {
                Text = "Borrow a Book",
                Size = new Size(480, 380),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = PrimaryDark,
                ForeColor = TextLight,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24) };

            var memberCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 400 };
            var members = _db.GetAllMembers().Where(m => m.IsActive).ToList();
            members.ForEach(m => memberCombo.Items.Add(m));
            memberCombo.DisplayMember = "FullName";

            var bookCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 400 };
            var books = _db.GetAllBooks().Where(b => b.IsAvailable).ToList();
            books.ForEach(b => bookCombo.Items.Add(b));
            bookCombo.DisplayMember = "Title";

            var duePicker = new DateTimePicker { Value = DateTime.Now.AddDays(14), Format = DateTimePickerFormat.Short, Width = 400 };
            var notesBox = new TextBox { Width = 400, Multiline = true, Height = 60 };

            int y = 16;
            foreach (var (lbl, ctrl) in new (string, Control)[] { ("Member *", memberCombo), ("Book *", bookCombo), ("Due Date", duePicker), ("Notes", notesBox) })
            {
                panel.Controls.Add(new Label { Text = lbl, ForeColor = TextMuted, Location = new Point(24, y), AutoSize = true });
                ctrl.Location = new Point(24, y + 20);
                StyleInput(ctrl);
                panel.Controls.Add(ctrl);
                y += ctrl.Height + 36;
            }

            var confirmBtn = MakeButton("📤 Confirm Borrow", PrimaryBlue);
            confirmBtn.Location = new Point(24, y);
            confirmBtn.Click += (s, e) =>
            {
                if (memberCombo.SelectedItem == null || bookCombo.SelectedItem == null)
                { MessageBox.Show("Select both member and book.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                var member = (Member)memberCombo.SelectedItem;
                var book = (Book)bookCombo.SelectedItem;
                var activeBorrows = _db.GetMemberActiveBorrowCount(member.Id);

                if (activeBorrows >= member.MaxBooksAllowed)
                { MessageBox.Show($"{member.FullName} has reached their borrow limit ({member.MaxBooksAllowed} books).", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (_db.BorrowBook(book.Id, member.Id, duePicker.Value, notesBox.Text))
                { MessageBox.Show($"✅ '{book.Title}' borrowed by {member.FullName}.\nDue: {duePicker.Value:MMM dd, yyyy}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); form.Close(); ShowBorrows(); }
                else
                    MessageBox.Show("Failed to borrow. Book may no longer be available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            panel.Controls.Add(confirmBtn);
            form.Controls.Add(panel);
            form.ShowDialog(this);
        }

        private void ShowReturnDialog()
        {
            var form = new Form
            {
                Text = "Return a Book",
                Size = new Size(480, 340),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = PrimaryDark,
                ForeColor = TextLight,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24) };

            var recordCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 400 };
            var activeBorrows = _db.GetActiveBorrows();
            foreach (var r in activeBorrows)
            {
                var label = $"[{r.Id}] {r.Book?.Title ?? "?"} → {r.Member?.FullName ?? "?"}  (Due: {r.DueDate:MMM dd})";
                recordCombo.Items.Add(label);
            }

            var fineLabel = new Label { Text = "Fine: $0.00", ForeColor = SuccessGreen, Font = new Font("Segoe UI", 11f, FontStyle.Bold), Location = new Point(24, 110), AutoSize = true };

            recordCombo.SelectedIndexChanged += (s, e) =>
            {
                if (recordCombo.SelectedIndex < 0) return;
                var r = activeBorrows[recordCombo.SelectedIndex];
                decimal fine = r.CalculatedFine;
                fineLabel.Text = fine > 0 ? $"⚠️ Fine: ${fine:F2} ({r.DaysOverdue} days overdue)" : "✅ No fine";
                fineLabel.ForeColor = fine > 0 ? DangerRed : SuccessGreen;
            };

            panel.Controls.Add(new Label { Text = "Select Borrow Record *", ForeColor = TextMuted, Location = new Point(24, 16), AutoSize = true });
            recordCombo.Location = new Point(24, 36);
            StyleInput(recordCombo);
            panel.Controls.Add(recordCombo);
            panel.Controls.Add(fineLabel);

            var returnBtn = MakeButton("📥 Confirm Return", SuccessGreen);
            returnBtn.Location = new Point(24, 150);
            returnBtn.Click += (s, e) =>
            {
                if (recordCombo.SelectedIndex < 0) { MessageBox.Show("Select a borrow record.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                var r = activeBorrows[recordCombo.SelectedIndex];
                decimal fine = r.CalculatedFine;
                if (_db.ReturnBook(r.Id, fine))
                { MessageBox.Show($"✅ Book returned successfully!\n{(fine > 0 ? $"Fine collected: ${fine:F2}" : "No fine.")}", "Returned", MessageBoxButtons.OK, MessageBoxIcon.Information); form.Close(); ShowBorrows(); }
                else
                    MessageBox.Show("Return failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            panel.Controls.Add(returnBtn);
            form.Controls.Add(panel);
            form.ShowDialog(this);
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  OVERDUE
        // ═══════════════════════════════════════════════════════════════════════
        private void ShowOverdue()
        {
            ClearContent();
            SetTitle("Overdue Books");

            var grid = MakeDataGrid();
            grid.Dock = DockStyle.Fill;
            grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "Record ID", Width = 90 },
                new DataGridViewTextBoxColumn { HeaderText = "Book", Width = 220 },
                new DataGridViewTextBoxColumn { HeaderText = "Member", Width = 160 },
                new DataGridViewTextBoxColumn { HeaderText = "Due Date", Width = 120 },
                new DataGridViewTextBoxColumn { HeaderText = "Days Overdue", Width = 120 },
                new DataGridViewTextBoxColumn { HeaderText = "Fine", Width = 100 }
            );

            var overdue = _db.GetActiveBorrows().Where(r => r.IsOverdue).ToList();
            foreach (var r in overdue)
            {
                var row = grid.Rows.Add(r.Id, r.Book?.Title ?? "-", r.Member?.FullName ?? "-",
                    r.DueDate.ToString("MMM dd, yyyy"), r.DaysOverdue, $"${r.CalculatedFine:F2}");
                grid.Rows[row].DefaultCellStyle.ForeColor = DangerRed;
            }

            var summary = new Label
            {
                Text = $"Total overdue: {overdue.Count} | Estimated fines: ${overdue.Sum(r => r.CalculatedFine):F2}",
                ForeColor = DangerRed,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 36,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            var wrapper = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            wrapper.Controls.Add(grid);
            wrapper.Controls.Add(summary);
            _contentPanel.Controls.Add(wrapper);
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  REPORTS
        // ═══════════════════════════════════════════════════════════════════════
        private void ShowReports()
        {
            ClearContent();
            SetTitle("Reports & Statistics");

            var stats = _db.GetDashboardStats();
            var allRecords = _db.GetAllBorrowRecords();
            var books = _db.GetAllBooks();
            var members = _db.GetAllMembers();

            var reportText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = CardBg,
                ForeColor = TextLight,
                Font = new Font("Consolas", 10f),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(16)
            };

            var sb = new System.Text.StringBuilder();
            var divider = new string('=', 60);
            sb.AppendLine(divider);
            sb.AppendLine($"  LIBRARY MANAGEMENT SYSTEM — REPORT");
            sb.AppendLine($"  Generated: {DateTime.Now:dddd, MMMM dd yyyy  HH:mm}");
            sb.AppendLine(divider);
            sb.AppendLine();
            sb.AppendLine("📊 OVERVIEW");
            sb.AppendLine($"  Total Books         : {stats.TotalBooks}");
            sb.AppendLine($"  Active Members      : {stats.TotalMembers}");
            sb.AppendLine($"  Active Borrows      : {stats.ActiveBorrows}");
            sb.AppendLine($"  Overdue Books       : {stats.OverdueBooks}");
            sb.AppendLine($"  Total Fines Owed    : ${stats.TotalFines:F2}");
            sb.AppendLine();
            sb.AppendLine("📚 BOOKS BY GENRE");
            var byGenre = books.GroupBy(b => b.Genre).OrderByDescending(g => g.Count());
            foreach (var g in byGenre)
                sb.AppendLine($"  {g.Key,-20}: {g.Count()} books");
            sb.AppendLine();
            sb.AppendLine("🔄 MOST BORROWED BOOKS (Top 5)");
            var topBorrowed = allRecords.GroupBy(r => r.Book?.Title ?? "?")
                .OrderByDescending(g => g.Count()).Take(5);
            int rank = 1;
            foreach (var g in topBorrowed)
                sb.AppendLine($"  {rank++}. {g.Key,-35}: {g.Count()} borrows");
            sb.AppendLine();
            sb.AppendLine("👥 MEMBERS WITH OUTSTANDING FINES");
            foreach (var m in members.Where(m => m.FineAmount > 0).OrderByDescending(m => m.FineAmount))
                sb.AppendLine($"  {m.FullName,-30}: ${m.FineAmount:F2}");
            sb.AppendLine();
            sb.AppendLine(divider);

            reportText.Text = sb.ToString();
            _contentPanel.Controls.Add(reportText);
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════════════════════
        private Panel BuildToolbar(string searchHint, out TextBox searchBox)
        {
            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.Transparent
            };

            searchBox = new TextBox
            {
                PlaceholderText = searchHint,
                Width = 280,
                Height = 32,
                Location = new Point(0, 10),
                BackColor = CardBg,
                ForeColor = TextLight,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10f)
            };
            toolbar.Controls.Add(searchBox);
            return toolbar;
        }

        private DataGridView MakeDataGrid()
        {
            var grid = new DataGridView
            {
                BackgroundColor = CardBg,
                ForeColor = TextLight,
                GridColor = Color.FromArgb(40, 55, 80),
                BorderStyle = BorderStyle.None,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = CardBg,
                    ForeColor = TextLight,
                    SelectionBackColor = PrimaryBlue,
                    SelectionForeColor = Color.White,
                    Font = new Font("Segoe UI", 9.5f),
                    Padding = new Padding(4)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = SidebarBg,
                    ForeColor = TextMuted,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    SelectionBackColor = SidebarBg
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(25, 35, 55)
                },
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ColumnHeadersHeight = 36,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                EnableHeadersVisualStyles = false
            };
            return grid;
        }

        private Button MakeButton(string text, Color bg)
        {
            return new Button
            {
                Text = text,
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Size = new Size(150, 36),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void StyleInput(Control ctrl)
        {
            ctrl.BackColor = CardBg;
            ctrl.ForeColor = TextLight;
            if (ctrl is TextBox tb) tb.BorderStyle = BorderStyle.FixedSingle;
            if (ctrl is ComboBox cb) { cb.BackColor = CardBg; cb.ForeColor = TextLight; }
        }

        private bool Confirm(string message) =>
            MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}
