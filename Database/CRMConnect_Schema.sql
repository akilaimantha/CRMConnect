-- ============================================================
-- CRMConnect - Oracle SQL Developer Worksheet
-- Tables: Users, Customers, SalesActivities, Tasks, CommunicationLog, Settings
-- Roles: Admin | Sales
-- ============================================================

BEGIN
   EXECUTE IMMEDIATE 'DROP TABLE Tasks CASCADE CONSTRAINTS';
   EXECUTE IMMEDIATE 'DROP TABLE CommunicationLog CASCADE CONSTRAINTS';
   EXECUTE IMMEDIATE 'DROP TABLE SalesActivities CASCADE CONSTRAINTS';
   EXECUTE IMMEDIATE 'DROP TABLE Customers CASCADE CONSTRAINTS';
   EXECUTE IMMEDIATE 'DROP TABLE Users CASCADE CONSTRAINTS';
   EXECUTE IMMEDIATE 'DROP TABLE AppUsers CASCADE CONSTRAINTS';
   EXECUTE IMMEDIATE 'DROP TABLE Settings CASCADE CONSTRAINTS';
   EXECUTE IMMEDIATE 'DROP SEQUENCE CustomerSeq';
   EXECUTE IMMEDIATE 'DROP SEQUENCE ActivitySeq';
   EXECUTE IMMEDIATE 'DROP SEQUENCE TaskSeq';
   EXECUTE IMMEDIATE 'DROP SEQUENCE LogSeq';
   EXECUTE IMMEDIATE 'DROP SEQUENCE UserSeq';
   EXECUTE IMMEDIATE 'DROP SEQUENCE SettingSeq';
EXCEPTION
   WHEN OTHERS THEN NULL;
END;
/

-- Users (Admin + Sales Representatives)
CREATE TABLE Users (
    UserID    NUMBER PRIMARY KEY,
    Name      VARCHAR2(100) NOT NULL,
    Email     VARCHAR2(100),
    Username  VARCHAR2(50) UNIQUE NOT NULL,
    Password  VARCHAR2(100) NOT NULL,
    Role      VARCHAR2(20) DEFAULT 'Sales',
    Status    VARCHAR2(20) DEFAULT 'Active'
);

-- Customers
CREATE TABLE Customers (
    CustomerID  NUMBER PRIMARY KEY,
    Name        VARCHAR2(100) NOT NULL,
    Email       VARCHAR2(100),
    Phone       VARCHAR2(20),
    Company     VARCHAR2(100),
    Address     VARCHAR2(255),
    Status      VARCHAR2(20) DEFAULT 'Active',
    CreatedDate DATE DEFAULT SYSDATE
);

-- Sales Activities
CREATE TABLE SalesActivities (
    ActivityID   NUMBER PRIMARY KEY,
    CustomerID   NUMBER REFERENCES Customers(CustomerID),
    ActivityDate DATE DEFAULT SYSDATE,
    LeadStatus   VARCHAR2(50),
    Notes        VARCHAR2(500)
);

-- Tasks
CREATE TABLE Tasks (
    TaskID       NUMBER PRIMARY KEY,
    TaskName     VARCHAR2(500) NOT NULL,
    AssignedTo   VARCHAR2(100),
    CustomerID   NUMBER REFERENCES Customers(CustomerID),
    Deadline     DATE,
    Status       VARCHAR2(50) DEFAULT 'Pending'
);

-- Communication Log
CREATE TABLE CommunicationLog (
    LogID             NUMBER PRIMARY KEY,
    CustomerID        NUMBER REFERENCES Customers(CustomerID),
    CommunicationDate DATE DEFAULT SYSDATE,
    CommType          VARCHAR2(50) DEFAULT 'Note',
    Notes             VARCHAR2(500)
);

-- System Settings
CREATE TABLE Settings (
    SettingID    NUMBER PRIMARY KEY,
    SettingName  VARCHAR2(100),
    SettingValue VARCHAR2(500)
);

CREATE SEQUENCE CustomerSeq START WITH 4;
CREATE SEQUENCE ActivitySeq START WITH 1;
CREATE SEQUENCE TaskSeq START WITH 1;
CREATE SEQUENCE LogSeq START WITH 1;
CREATE SEQUENCE UserSeq START WITH 3;
CREATE SEQUENCE SettingSeq START WITH 4;

-- Users
INSERT INTO Users VALUES (1, 'System Administrator', 'admin@slic.lk', 'admin', 'admin123', 'Admin', 'Active');
INSERT INTO Users VALUES (2, 'John Smith', 'john@slic.lk', 'sales1', 'sales123', 'Sales', 'Active');
INSERT INTO Users VALUES (3, 'Sarah Johnson', 'sarah@slic.lk', 'sales2', 'sales123', 'Sales', 'Active');

-- Customers
INSERT INTO Customers VALUES (1, 'ABC Corporation', 'contact@abc.com', '555-0100', 'ABC Corp', '123 Business St, New York', 'Active', SYSDATE);
INSERT INTO Customers VALUES (2, 'XYZ Industries', 'info@xyz.com', '555-0200', 'XYZ Industries', '456 Factory Rd, Chicago', 'Active', SYSDATE);
INSERT INTO Customers VALUES (3, 'Tech Solutions LLC', 'sales@techsolutions.com', '555-0300', 'Tech Solutions', '789 Tech Park, San Francisco', 'Active', SYSDATE);

-- Sales Activities
INSERT INTO SalesActivities VALUES (ActivitySeq.NEXTVAL, 1, SYSDATE - 5, 'New Lead', 'Initial inquiry for enterprise plan');
INSERT INTO SalesActivities VALUES (ActivitySeq.NEXTVAL, 2, SYSDATE - 3, 'Contacted', 'Sent product brochure via email');
INSERT INTO SalesActivities VALUES (ActivitySeq.NEXTVAL, 3, SYSDATE - 1, 'Negotiation', 'Discussing pricing and contract terms');
INSERT INTO SalesActivities VALUES (ActivitySeq.NEXTVAL, 1, SYSDATE, 'Closed', 'Contract signed successfully');

-- Tasks
INSERT INTO Tasks VALUES (TaskSeq.NEXTVAL, 'Follow up with ABC Corp', 'John Smith', 1, SYSDATE + 2, 'Pending');
INSERT INTO Tasks VALUES (TaskSeq.NEXTVAL, 'Send quote to XYZ Industries', 'Sarah Johnson', 2, SYSDATE + 5, 'Pending');
INSERT INTO Tasks VALUES (TaskSeq.NEXTVAL, 'Schedule product demo', 'John Smith', 3, SYSDATE + 1, 'Pending');

-- Communication Log
INSERT INTO CommunicationLog VALUES (LogSeq.NEXTVAL, 1, SYSDATE - 2, 'Phone Call', 'Discussed pricing options and timeline.');
INSERT INTO CommunicationLog VALUES (LogSeq.NEXTVAL, 2, SYSDATE - 1, 'Email', 'Sent follow-up with product specifications.');
INSERT INTO CommunicationLog VALUES (LogSeq.NEXTVAL, 3, SYSDATE, 'Meeting', 'Demo scheduled for next Tuesday.');

-- Settings
INSERT INTO Settings VALUES (1, 'CompanyName', 'Sri Lanka Insurance Corporation');
INSERT INTO Settings VALUES (2, 'NotificationDays', '3');
INSERT INTO Settings VALUES (3, 'BackupEnabled', 'true');
INSERT INTO Settings VALUES (4, 'EmailNotifications', 'true');

COMMIT;

SELECT 'Users: ' || COUNT(*) FROM Users
UNION ALL SELECT 'Customers: ' || COUNT(*) FROM Customers
UNION ALL SELECT 'Sales: ' || COUNT(*) FROM SalesActivities
UNION ALL SELECT 'Tasks: ' || COUNT(*) FROM Tasks
UNION ALL SELECT 'Communications: ' || COUNT(*) FROM CommunicationLog;

-- LOGIN: admin / admin123  (Admin Panel)
-- LOGIN: sales1 / sales123  (Sales Representative Panel)
-- LOGIN: sales2 / sales123
