-- Run ONLY if you already have the base tables from your old worksheet
-- and need extra columns for the updated CRMConnect application.

ALTER TABLE Customers ADD (PolicyType VARCHAR2(50) DEFAULT 'Life Protection');
ALTER TABLE Customers ADD (LoginPassword VARCHAR2(100) DEFAULT 'customer123');

ALTER TABLE CommunicationLog ADD (CommType VARCHAR2(50) DEFAULT 'Note');

UPDATE Customers SET LoginPassword = 'customer123' WHERE LoginPassword IS NULL;
UPDATE Customers SET PolicyType = 'Life Protection' WHERE PolicyType IS NULL;

COMMIT;
