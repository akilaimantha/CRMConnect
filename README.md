# CRMConnect

**Customer Relationship Management System** for a sales organization — built with **ASP.NET Core 8**, **Oracle Database**, **HTML**, **CSS**, and **JavaScript**.

Branded for **Sri Lanka Insurance Corporation (SLIC)** with separate **Admin** and **Customer** portals.

![SLIC Theme](CRMConnect/wwwroot/images/slic-logo.svg)

## Features

| Module | Description |
|--------|-------------|
| **Customer Profiles** | CRUD for customer records with policy types |
| **Sales Tracking** | Log leads, pipeline stages, conversion metrics |
| **Task Management** | Assign tasks to sales reps with due dates |
| **Communication Log** | Phone, email, meeting, and note history |
| **Admin Panel** | User management and system settings |
| **Customer Portal** | Profile, policies, messages, support requests |
| **Notifications** | AJAX alerts for tasks due within 3 days |

## Technology Stack

- ASP.NET Core 8 Razor Pages Web Application
- Oracle Database (Oracle.ManagedDataAccess.Core)
- Bootstrap 5 + custom SLIC theme CSS
- jQuery for client-side validation and notifications

## Quick Start

### 1. Oracle Database Setup

1. Install Oracle Database (XE or higher).
2. Open and run **`Database/CRMConnect_Schema.sql`** in Oracle SQL Developer (press F5).

### 2. Connection String

Edit `CRMConnect/appsettings.json`:

```json
"ConnectionStrings": {
  "OracleDB": "User Id=system;Password=YOUR_PASSWORD;Data Source=localhost:1521/XE;"
}
```

### 3. Run the Application

```bash
cd CRMConnect
dotnet run
```

Open **https://localhost:7xxx** (see console for port).

## Demo Login Credentials

| Portal | Credentials |
|--------|-------------|
| **Admin** | `admin` / `admin123` |
| **Sales User** | `salesuser` / `sales123` |
| **Customer** | `contact@abc.com` / `customer123` (also `info@xyz.com`, `sales@techsolutions.com`) |

## Portal Structure

### Admin Portal (after admin login)

- `/Admin` — Dashboard with statistics
- `/Customers` — Customer CRUD
- `/Sales` — Sales activities & pipeline
- `/Tasks` — Task management
- `/Communication` — Communication log
- `/Admin/Users` — User management
- `/Admin/Settings` — System settings

### Customer Portal (after customer login)

- `/Portal` — Customer dashboard
- `/Portal/Profile` — View profile
- `/Portal/Communications` — Message history
- `/Portal/Policies` — Insurance policies
- `/Portal/Support` — Submit support requests

## Database Tables

- `Customers` — Customer profiles with portal login
- `SalesActivities` — Sales and lead tracking
- `Tasks` — Task assignments
- `CommunicationLog` — Customer communications
- `AppUsers` — Admin and sales users
- `Settings` — Application configuration

## Assignment Alignment

This project fulfills the CRMConnect case study requirements:

1. **Backend** — ASP.NET Web Application with Oracle CRUD operations
2. **Frontend** — HTML structure via Razor Pages, SLIC-themed CSS, JavaScript validation & notifications
3. **Integration** — Frontend connected to backend; task notification API at `/api/notifications`
4. **Dual interfaces** — Admin side for sales organization; Customer side for policyholders

## Project Structure

```
CRMConnect/
├── CRMConnect/           # ASP.NET Core web project
│   ├── Pages/            # Razor Pages (Admin + Portal + Features)
│   ├── Models/           # Database helper, session auth
│   └── wwwroot/          # CSS, JS, SLIC logo
└── Database/
    └── CRMConnect_Schema.sql  # Full Oracle worksheet (run in SQL Developer)
```

## License

Educational assignment project.
