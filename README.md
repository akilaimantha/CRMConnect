Name: S A D Akila Imantha

# CRMConnect – Customer Relationship Management System

Web-based CRM for a sales organization built with **ASP.NET Core 8**, **Oracle Database**, **HTML**, **CSS**, and **JavaScript**. Branded for **Sri Lanka Insurance (SLIC)**.

## User Roles

|            Role          |    Panel    |         Login        |
|--------------------------|-------------|----------------------|
| **System Administrator** | Admin Panel | `admin` / `admin123` |
| **Sales Representative** | Sales Panel | `sales1` / `sales123` |

## Folder Structure

```
CRMConnect/
├── CRMConnect/
│   ├── Models/
│   │   ├── DatabaseHelper.cs      # Oracle connectivity
│   │   ├── SessionAuth.cs         # Session & roles
│   │   └── AuthorizeAttributes.cs # Admin / Sales guards
│   ├── Pages/
│   │   ├── Index.cshtml           # Login
│   │   ├── Logout.cshtml
│   │   ├── Sales/                 # Sales Representative Panel
│   │   │   ├── Index.cshtml       # Dashboard
│   │   │   ├── Customers.cshtml   # CRUD + Search
│   │   │   ├── Customers/Edit.cshtml
│   │   │   ├── SalesActivities.cshtml
│   │   │   ├── Tasks.cshtml
│   │   │   └── Communication.cshtml
│   │   ├── Admin/                 # Administrator Panel
│   │   │   ├── Index.cshtml       # Dashboard
│   │   │   ├── Users.cshtml
│   │   │   ├── Customers.cshtml
│   │   │   ├── Reports.cshtml
│   │   │   └── Settings.cshtml
│   │   └── Shared/
│   │       ├── _LayoutSales.cshtml   # Sidebar – Sales
│   │       └── _LayoutAdmin.cshtml   # Sidebar – Admin
│   ├── wwwroot/
│   │   ├── css/slic-theme.css
│   │   ├── js/crm.js
│   │   └── images/slic-logo.svg
│   └── Program.cs
└── Database/
    └── CRMConnect_Schema.sql
```

## Database Tables

1. **Users** – Admin & Sales accounts  
2. **Customers** – Customer profiles (Company, Status)  
3. **SalesActivities** – Lead tracking  
4. **Tasks** – TaskName, Deadline, Status  
5. **CommunicationLog** – Calls, emails, meetings  
6. **Settings** – System configuration  

## Setup

1. Run `Database/CRMConnect_Schema.sql` in Oracle SQL Developer (F5).
2. Set connection string in `CRMConnect/appsettings.json`.
3. Run: `cd CRMConnect` → `dotnet run`.

## Features by Panel

### Sales Representative
- Dashboard (customers, tasks, sales, communications)
- Customer CRUD + search + edit
- Sales activities (New Lead → Closed/Rejected)
- Task management (complete, update status)
- Communication log

### Administrator
- Dashboard (users, customers, sales, active users)
- User management (add, edit, delete, reset password)
- Customer monitoring (view all, remove records)
- Reports (sales, customers, users, tasks)
- System settings (notifications, backup flag)

## Technology

- ASP.NET Core Razor Pages + C#
- Oracle.ManagedDataAccess.Core
- Bootstrap 5 + SLIC theme (teal & gold)
- Sidebar navigation layout
- Session authentication & role-based routing
- AJAX task notifications (`/api/notifications`)
