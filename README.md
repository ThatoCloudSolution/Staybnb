# Staybnb: Managed Property Rental Ecosystem

Staybnb is a comprehensive property rental platform inspired by the core functionality of Airbnb. It is built using **ASP.NET Core MVC**, **Entity Framework Core**, and **SQL Server**.

The heart of this project is its robust **User Lifecycle management**, allowing users to transition seamlessly from a Guest looking for a stay to a Host managing their own property.

---

## 🚀 Features

- **Identity & Role Evolution:** Secure user authentication. New registrants are automatically assigned the `Guest` role. The system enforces a strict "Guest OR Host" policy (a user cannot hold both roles simultaneously).
- **SuperAdmin Seeding:** A system role seeder automatically creates a `SuperAdmin` on startup, which is the only role capable of promoting users to the `Admin` role.
- **Host Application Workflow:** Guests can apply to become Hosts by submitting property details and uploading mandatory property images.
- **Host Dashboard:** Once approved, Hosts can manage property listings (toggle active status), review pending booking requests (Approve/Reject), and manage earnings.
- **Guest Experience:** Clean property discovery with filtering by city/property type. Features a dynamic Booking Engine that calculates total price: `(Price/Night × Nights) + Cleaning Fee + Service Fee`.
- **Admin Oversight:** Admins have access to a command center to review host applications, view platform revenue, and monitor system activity.
- **Communication & Logging:** Database-backed inbox and notifications for status updates. All critical actions (logins, role changes, booking updates) are tracked via a system-wide `ActivityLog`.

---

## 🛠️ Technology Stack

| Layer | Technology |
|---|---|
| **Framework** | .NET 10.0 (ASP.NET Core MVC) |
| **Database** | SQL Server (Entity Framework Core) |
| **Authentication** | ASP.NET Core Identity |
| **Frontend** | Razor Views, Bootstrap 5, HTML/CSS |
| **Architecture** | MVC (Model-View-Controller) with a dedicated Service Layer |

---

## ⚙️ Getting Started

To run this project locally, follow these steps:

### Prerequisites
- [Visual Studio 2026 (or 2022)](https://visualstudio.microsoft.com/) with the **ASP.NET and web development** workload.
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or LocalDB) installed.
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) (Optional, for viewing the database).

### Installation

**1. Clone the repository:**
    git clone https://github.com/ThatoCloudSolution/Staybnb.git

**2. Open the Solution:**
Open `Staybnb.slnx` in Visual Studio.

**3. Configure the Database:**
Open `appsettings.json` and ensure the connection string points to your local SQL Server instance:
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=StaybnbDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    }

**4. Apply Migrations:**
Open the **Package Manager Console** (Tools > NuGet Package Manager > Package Manager Console) and run:
    Update-Database
*(Note: Ensure the Package Source is set to `nuget.org` and the Default project is `Staybnb`)*

**5. Run the Application:**
Press `F5` or click the green "Start" button in Visual Studio.

---

## 🔐 Default Login Credentials

The system automatically seeds a SuperAdmin account on the first run. You can use this account to test the administrative workflows:

- **Email:** `superadmin@staybnb.com`
- **Password:** `SuperAdmin@123`

---

## 📁 Project Structure

    Staybnb/
    ├── Controllers/      - Handles routing and user input (Account, AdminDashboard, HostDashboard, Booking, HostApplication).
    ├── Models/           - Entity Framework Core database models and ViewModels.
    ├── Data/             - Contains ApplicationDbContext and the RoleSeeder.
    ├── Services/         - Business logic (e.g., BookingService for price calculations, HostApplicationService for role swapping).
    ├── Views/            - Razor UI templates.
    └── appsettings.json  - Connection string configuration.

---

## 👤 Author

**Thato Mashabela** — Programming 742
