# 📚 Library Management System — C# WinForms

A full-featured desktop library management application built with C# (.NET 8), Windows Forms, and SQLite.

---

## ✅ Features

| Feature | Description |
|---|---|
| 📖 Books | Add, edit, delete, search books. Tracks copies & availability |
| 👥 Members | Manage members with membership types, expiry dates, fines |
| 🔄 Borrow/Return | Issue and return books with automatic fine calculation |
| ⚠️ Overdue | View all overdue loans with fine totals |
| 📊 Reports | Summary statistics, top genres, most borrowed books |
| 💾 Database | SQLite — no external DB server needed |

---

## 🏗️ Project Structure

```
LibraryManagementSystem/
│
├── Program.cs                  ← App entry point
├── LibraryManagementSystem.csproj
│
├── Models/
│   ├── Book.cs                 ← Book entity
│   ├── Member.cs               ← Member entity (with MembershipType enum)
│   └── BorrowRecord.cs         ← Borrow/return tracking (with fine logic)
│
├── Data/
│   └── DatabaseContext.cs      ← SQLite CRUD for all entities + seeding
│
└── UI/
    └── MainForm.cs             ← All WinForms UI (Dashboard, Books, Members, Borrows, Reports)
```

---

## 🖥️ How to Run on Your PC

### Prerequisites

1. **Install .NET 8 SDK**
   - Go to: https://dotnet.microsoft.com/download/dotnet/8.0
   - Download and install ".NET 8 SDK (Windows x64)"
   - Verify: open Command Prompt and type `dotnet --version`

2. **Any code editor** (optional but recommended):
   - **Visual Studio 2022** (Community — free): https://visualstudio.microsoft.com/
   - Or **Visual Studio Code** with the C# extension

---

### ▶️ Option A — Run with Command Prompt (Easiest)

```cmd
# 1. Extract / place the project folder somewhere (e.g. C:\Projects\LibraryManagementSystem)

# 2. Open Command Prompt and navigate to the project folder:
cd C:\Projects\LibraryManagementSystem

# 3. Restore NuGet packages:
dotnet restore

# 4. Run the application:
dotnet run
```

The app will open as a Windows desktop window. The database (`library.db`) is created automatically in the same folder.

---

### ▶️ Option B — Open in Visual Studio 2022

1. Open Visual Studio 2022
2. Click **"Open a project or solution"**
3. Browse to the folder and select `LibraryManagementSystem.csproj`
4. Press **F5** (or the green ▶ Run button) to build and run

---

### ▶️ Option C — Build an .exe to distribute

```cmd
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Find the `.exe` in: `bin\Release\net8.0-windows\win-x64\publish\`

---

## 🗄️ Database

- Uses **SQLite** (via `Microsoft.Data.Sqlite` NuGet package)
- Database file: `library.db` — created automatically on first run
- Auto-seeded with **10 sample books** and **5 sample members**
- No installation or configuration required

---

## 💡 How to Use

### Adding a Book
1. Click **Books** in the sidebar
2. Click **➕ Add Book**
3. Fill in the form → **✅ Add Book**

### Registering a Member
1. Click **Members** in the sidebar
2. Click **➕ Add Member**
3. Fill in details and choose membership type

### Borrowing a Book
1. Click **Borrow / Return** in the sidebar
2. Click **📤 Borrow Book**
3. Select a member, select a book, set due date → **Confirm**

### Returning a Book
1. Click **Borrow / Return**
2. Click **📥 Return Book**
3. Select the active borrow record — fine is calculated automatically → **Confirm**

### Viewing Overdue Books
- Click **⚠️ Overdue** in the sidebar

---

## 💰 Fine System

- **$0.50 per day** overdue
- Fine is added to the member's account on return
- Viewable in the Members list and Reports

---

## 🔐 Membership Types & Borrow Limits

| Type | Max Books |
|---|---|
| Standard | 3 |
| Premium | 10 |
| Student | 5 |
| Senior | 7 |

---

## 🛠️ Tech Stack

| Component | Technology |
|---|---|
| Language | C# 12 |
| Framework | .NET 8 |
| UI | Windows Forms (WinForms) |
| Database | SQLite |
| ORM | Raw SQL via Microsoft.Data.Sqlite |
| Packages | Microsoft.Data.Sqlite, Newtonsoft.Json |

---

## 📦 NuGet Packages

These are automatically installed when you run `dotnet restore`:

- `Microsoft.Data.Sqlite` — SQLite database access
- `Newtonsoft.Json` — JSON serialization (for future export features)

---

*Built with ❤️ using C# and .NET 8*
