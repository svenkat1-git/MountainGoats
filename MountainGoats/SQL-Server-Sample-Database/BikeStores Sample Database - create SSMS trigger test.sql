USE BikeStores;
GO

-- Drop the procedure if it already exists
IF OBJECT_ID('sales.sp_TestOrdersTrigger', 'P') IS NOT NULL
    DROP PROCEDURE sales.sp_TestOrdersTrigger;
GO

CREATE PROCEDURE sales.sp_TestOrdersTrigger
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Step a & b: Insert a dummy order record into sales.orders.
        -- Note: We assume that the referenced foreign key values (store_id = 1 and staff_id = 1)
        -- exist in the sales.stores and sales.staffs tables respectively.
        INSERT INTO sales.orders (customer_id, order_status, order_date, required_date, shipped_date, store_id, staff_id)
        VALUES (NULL,               -- customer_id (nullable)
                1,                  -- order_status (e.g., 1 = Pending)
                GETDATE(),          -- order_date (current date)
                DATEADD(day, 7, GETDATE()), -- required_date (7 days from now)
                NULL,               -- shipped_date (not yet shipped)
                1,                  -- store_id (assumed existing)
                1);                 -- staff_id (assumed existing)

        -- Capture the newly created order_id using SCOPE_IDENTITY()
        DECLARE @NewOrderID INT = SCOPE_IDENTITY();

        -- Step c: Insert two dummy order items for the newly created order.
        INSERT INTO sales.order_items (order_id, item_id, product_id, quantity, list_price, discount)
        VALUES (@NewOrderID, 1, 1, 2, 100.00, 0),
               (@NewOrderID, 2, 2, 1, 200.00, 0);

        -- If both insertions succeed, commit the transaction.
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- In case of any error, rollback the transaction.
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Raise the error for further handling
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrState INT = ERROR_STATE();
        RAISERROR(@ErrMsg, @ErrSeverity, @ErrState);
    END CATCH;
END;
GO
