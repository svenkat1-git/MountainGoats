USE BikeStores;
GO

-- Create the trigger "the_auditor" on the sales.orders table
CREATE TRIGGER the_auditor
ON sales.orders
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert an audit record for each row affected in the orders table.
    INSERT INTO sales.bikestore_audit (activity_type, record_info)
    SELECT
        -- Determine if the operation was an insert (no matching deleted row) or an update (matching deleted row exists)
        CASE 
            WHEN d.order_id IS NULL THEN 'order created'
            ELSE 'order updated'
        END AS activity_type,
        -- Concatenate order details into a single string
        'OrderID: ' + CAST(i.order_id AS VARCHAR(20)) +
        ', CustomerID: ' + CAST(i.customer_id AS VARCHAR(20)) +
        ', StaffID: ' + CAST(i.staff_id AS VARCHAR(20)) +
        ', StoreID: ' + CAST(i.store_id AS VARCHAR(20)) AS record_info
    FROM inserted i
    LEFT JOIN deleted d
        ON i.order_id = d.order_id;
END;
GO
