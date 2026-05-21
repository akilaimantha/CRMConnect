-- ============================================================
-- CRMConnect - Oracle SQL Developer Worksheet
-- Customer Relationship Management System
-- Sri Lanka Insurance (SLIC) themed application
-- Run this entire script in SQL Developer (F5)
-- ============================================================

-- STEP 1: Drop existing objects (safe re-run)
BEGIN
   EXECUTE IMMEDIATE 'DROP TABLE Tasks CASCADE CONSTRAINTS';
   EXECUTE IMMEDIATE 'DROP TABLE CommunicationLog CASCADE CONSTRAINTS';
   EXECUTE IMMEDIATE 'DROP TABLE SalesActivities CASCADE CONSTRAINTS';
   EXECUTE IMMEDIATE 'DROP TABLE Customers CASCADE CONSTRAINTS';
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

-- ============================================================
-- STEP 2: Create tables
-- ============================================================

CREATE TABLE Customers (
    CustomerID    NUMBER PRIMARY KEY,
    Name          VARCHAR2(100) NOT NULL,
    Email         VARCHAR2(100),
    Phone         VARCHAR2(20),
    Address       VARCHAR2(255),
    PolicyType    VARCHAR2(50) DEFAULT 'Life Protection',
    LoginPassword VARCHAR2(100) DEFAULT 'customer123',
    CreatedDate   DATE DEFAULT SYSDATE
);

CREATE TABLE SalesActivities (
    ActivityID   NUMBER PRIMARY KEY,
    CustomerID   NUMBER REFERENCES Customers(CustomerID),
    ActivityDate DATE DEFAULT SYSDATE,
    LeadStatus   VARCHAR2(50),
    Notes        VARCHAR2(500)
);

CREATE TABLE Tasks (
    TaskID          NUMBER PRIMARY KEY,
    AssignedTo      VARCHAR2(100),
    CustomerID      NUMBER REFERENCES Customers(CustomerID),
    TaskDescription VARCHAR2(500),
    DueDate         DATE,
    Status          VARCHAR2(50) DEFAULT 'Pending'
);

CREATE TABLE CommunicationLog (
    LogID             NUMBER PRIMARY KEY,
    CustomerID        NUMBER REFERENCES Customers(CustomerID),
    CommunicationDate DATE DEFAULT SYSDATE,
    Notes             VARCHAR2(500),
    CommType          VARCHAR2(50) DEFAULT 'Note'
);

CREATE TABLE AppUsers (
    UserID   NUMBER PRIMARY KEY,
    Username VARCHAR2(50) UNIQUE NOT NULL,
    Password VARCHAR2(100) NOT NULL,
    Role     VARCHAR2(20) DEFAULT 'User'
);

CREATE TABLE Settings (
    SettingID    NUMBER PRIMARY KEY,
    SettingName  VARCHAR2(100),
    SettingValue VARCHAR2(500)
);

-- ============================================================
-- STEP 3: Create sequences
-- ============================================================

CREATE SEQUENCE CustomerSeq START WITH 4;
CREATE SEQUENCE ActivitySeq START WITH 1;
CREATE SEQUENCE TaskSeq START WITH 1;
CREATE SEQUENCE LogSeq START WITH 1;
CREATE SEQUENCE UserSeq START WITH 3;
CREATE SEQUENCE SettingSeq START WITH 4;

-- ============================================================
-- STEP 4: Insert sample customers (portal login = Email + customer123)
-- ============================================================

INSERT INTO Customers (CustomerID, Name, Email, Phone, Address, PolicyType, LoginPassword, CreatedDate)
VALUES (1, 'ABC Corporation', 'contact@abc.com', '555-0100', '123 Business St, New York, NY 10001', 'Life Protection', 'customer123', SYSDATE);

INSERT INTO Customers (CustomerID, Name, Email, Phone, Address, PolicyType, LoginPassword, CreatedDate)
VALUES (2, 'XYZ Industries', 'info@xyz.com', '555-0200', '456 Factory Rd, Chicago, IL 60601', 'Health', 'customer123', SYSDATE);

INSERT INTO Customers (CustomerID, Name, Email, Phone, Address, PolicyType, LoginPassword, CreatedDate)
VALUES (3, 'Tech Solutions LLC', 'sales@techsolutions.com', '555-0300', '789 Tech Park, San Francisco, CA 94105', 'Investment', 'customer123', SYSDATE);

-- ============================================================
-- STEP 5: Insert sample sales activities
-- ============================================================

INSERT INTO SalesActivities VALUES (ActivitySeq.NEXTVAL, 1, SYSDATE - 5, 'Hot Lead', 'Customer showed strong interest in premium life plan');
INSERT INTO SalesActivities VALUES (ActivitySeq.NEXTVAL, 2, SYSDATE - 3, 'Warm Lead', 'Sent product brochure via email');
INSERT INTO SalesActivities VALUES (ActivitySeq.NEXTVAL, 3, SYSDATE - 1, 'Negotiation', 'Discussing pricing and contract terms');

-- ============================================================
-- STEP 6: Insert sample tasks
-- ============================================================

INSERT INTO Tasks VALUES (TaskSeq.NEXTVAL, 'John Smith', 1, 'Follow up with ABC Corp regarding proposal', SYSDATE + 2, 'Pending');
INSERT INTO Tasks VALUES (TaskSeq.NEXTVAL, 'Sarah Johnson', 2, 'Send quote to XYZ Industries', SYSDATE + 5, 'Pending');
INSERT INTO Tasks VALUES (TaskSeq.NEXTVAL, 'Mike Brown', 3, 'Schedule product demo for Tech Solutions', SYSDATE + 1, 'Pending');

-- ============================================================
-- STEP 7: Insert sample communication logs (with CommType)
-- ============================================================

INSERT INTO CommunicationLog VALUES (LogSeq.NEXTVAL, 1, SYSDATE - 2, 'Discussed pricing options. Customer interested in volume discount.', 'Phone Call');
INSERT INTO CommunicationLog VALUES (LogSeq.NEXTVAL, 2, SYSDATE - 1, 'Sent product brochure and case studies.', 'Email');
INSERT INTO CommunicationLog VALUES (LogSeq.NEXTVAL, 3, SYSDATE, 'Scheduled demo for next week.', 'Meeting');

-- ============================================================
-- STEP 8: Insert application users (admin portal login)
-- ============================================================

INSERT INTO AppUsers VALUES (1, 'admin', 'admin123', 'Admin');
INSERT INTO AppUsers VALUES (2, 'salesuser', 'sales123', 'User');

-- ============================================================
-- STEP 9: Insert system settings
-- ============================================================

INSERT INTO Settings VALUES (1, 'CompanyName', 'Sri Lanka Insurance Corporation');
INSERT INTO Settings VALUES (2, 'NotificationDays', '3');
INSERT INTO Settings VALUES (3, 'SupportEmail', 'support@slic.lk');

COMMIT;

-- ============================================================
-- STEP 10: Verify installation
-- ============================================================

SELECT 'Customers: ' || COUNT(*) AS Result FROM Customers
UNION ALL
SELECT 'Tasks: ' || COUNT(*) FROM Tasks
UNION ALL
SELECT 'Sales Activities: ' || COUNT(*) FROM SalesActivities
UNION ALL
SELECT 'Communication Log: ' || COUNT(*) FROM CommunicationLog
UNION ALL
SELECT 'App Users: ' || COUNT(*) FROM AppUsers
UNION ALL
SELECT 'Settings: ' || COUNT(*) FROM Settings;

-- ============================================================
-- Demo login credentials (for assignment testing)
-- ============================================================
-- ADMIN PORTAL:    admin     / admin123
-- SALES USER:      salesuser / sales123
-- CUSTOMER PORTAL: contact@abc.com / customer123
--                  info@xyz.com    / customer123
--                  sales@techsolutions.com / customer123
-- ============================================================
