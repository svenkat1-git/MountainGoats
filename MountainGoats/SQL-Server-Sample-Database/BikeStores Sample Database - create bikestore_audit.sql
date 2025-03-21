USE BikeStores;
GO

CREATE TABLE dbo.bikestore_audit
(
    id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    audit_timestamp DATETIME NOT NULL CONSTRAINT DF_bikestore_audit_audit_timestamp DEFAULT GETDATE(),
    activity_type VARCHAR(100) NULL,  -- This column can hold values such as "order created", "customer added", etc.
    record_info NTEXT NOT NULL        -- Contains order details like order id, customer id, staff id, store id
);
GO
