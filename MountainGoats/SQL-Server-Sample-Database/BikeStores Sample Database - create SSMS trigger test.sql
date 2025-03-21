USE BikeStores;
GO
--------------------------------------------------------------------------------
-- STEP 1: Ensure Valid Reference Data Exists (Based on IDs)
--------------------------------------------------------------------------------
-- Ensure a store exists with store_id = 1.
IF NOT EXISTS (SELECT 1 FROM sales.stores WHERE store_id = 1)
BEGIN
    INSERT INTO sales.stores (store_name, phone, email, street, city, state, zip_code)
    VALUES ('Store 1', '1234567890', 'store1@example.com', '123 Main St', 'CityX', 'ST', '12345');
END;
GO

-- Ensure a staff exists with staff_id = 1.
IF NOT EXISTS (SELECT 1 FROM sales.staffs WHERE staff_id = 1)
BEGIN
    INSERT INTO sales.staffs (first_name, last_name, email, phone, active, store_id)
    VALUES ('Staff', 'One', 'staff1@example.com', '1234567890', 1, 1);
END;
GO

-- Ensure a category exists with category_id = 1.
IF NOT EXISTS (SELECT 1 FROM production.categories WHERE category_id = 1)
BEGIN
    INSERT INTO production.categories (category_name)
    VALUES ('Category 1');
END;
GO

-- Ensure a brand exists with brand_id = 1.
IF NOT EXISTS (SELECT 1 FROM production.brands WHERE brand_id = 1)
BEGIN
    INSERT INTO production.brands (brand_name)
    VALUES ('Brand 1');
END;
GO

-- Ensure products exist with product_id 1 and 2.
IF NOT EXISTS (SELECT 1 FROM production.products WHERE product_id = 1)
BEGIN
    INSERT INTO production.products (product_name, brand_id, category_id, model_year, list_price)
    VALUES ('Product 1', 1, 1, 2020, 100.00);
END;
GO

IF NOT EXISTS (SELECT 1 FROM production.products WHERE product_id = 2)
BEGIN
    INSERT INTO production.products (product_name, brand_id, category_id, model_year, list_price)
    VALUES ('Product 2', 1, 1, 2020, 200.00);
END;
GO

-- Ensure that product_id 1 has stock in store_id = 1.
IF NOT EXISTS (SELECT 1 FROM production.stocks WHERE store_id = 1 AND product_id = 1)
BEGIN
    INSERT INTO production.stocks (store_id, product_id, quantity)
    VALUES (1, 1, 50);
END;
GO

-- Ensure that product_id 2 has stock in store_id = 1.
IF NOT EXISTS (SELECT 1 FROM production.stocks WHERE store_id = 1 AND product_id = 2)
BEGIN
    INSERT INTO production.stocks (store_id, product_id, quantity)
    VALUES (1, 2, 50);
END;
GO
--------------------------------------------------------------------------------
-- STEP 2: Create/Update the Test Stored Procedure
--------------------------------------------------------------------------------
IF OBJECT_ID('sales.sp_TestOrdersTrigger', 'P') IS NOT NULL
    DROP PROCEDURE sales.sp_TestOrdersTrigger;
GO

CREATE PROCEDURE sales.sp_TestOrdersTrigger
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Valid Test (Procedure): Insert a dummy order record into sales.orders.
        INSERT INTO sales.orders (customer_id, order_status, order_date, required_date, shipped_date, store_id, staff_id)
        VALUES (NULL, 1, GETDATE(), DATEADD(day, 7, GETDATE()), NULL, 1, 1);

        DECLARE @NewOrderID INT = SCOPE_IDENTITY();

        -- Insert two dummy order items.
        INSERT INTO sales.order_items (order_id, item_id, product_id, quantity, list_price, discount)
        VALUES (@NewOrderID, 1, 1, 2, 100.00, 0),
               (@NewOrderID, 2, 2, 1, 200.00, 0);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrState INT = ERROR_STATE();
        RAISERROR(@ErrMsg, @ErrSeverity, @ErrState);
    END CATCH;
END;
GO
--------------------------------------------------------------------------------
-- STEP 3: Valid Test 1 – Manual Valid Order Insertion with a New Customer
--------------------------------------------------------------------------------
PRINT '=== Valid Test 2: Manual valid order insertion ===';
BEGIN TRY
    BEGIN TRANSACTION;

    -- Insert a new customer.
    INSERT INTO sales.customers (first_name, last_name, phone, email, street, city, state, zip_code)
    VALUES ('Jane', 'Doe', '555-1234', 'jane.doe@example.com', '456 Main St', 'CityY', 'CY', '54321');

    DECLARE @CustomerID INT = SCOPE_IDENTITY();

    -- Insert a new order using the new customer.
    INSERT INTO sales.orders (customer_id, order_status, order_date, required_date, shipped_date, store_id, staff_id)
    VALUES (@CustomerID, 1, GETDATE(), DATEADD(day, 7, GETDATE()), NULL, 1, 1);

    DECLARE @OrderID INT = SCOPE_IDENTITY();

    -- Use known product IDs directly.
    DECLARE @Prod1 INT = 1, @Prod2 INT = 2;

    INSERT INTO sales.order_items (order_id, item_id, product_id, quantity, list_price, discount)
    VALUES (@OrderID, 1, @Prod1, 3, 150.00, 0),
           (@OrderID, 2, @Prod2, 2, 200.00, 0);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    PRINT 'Error in Valid Test 2: ' + ERROR_MESSAGE();
END CATCH;
GO

-- Display audit records after valid test 2.
SELECT *
FROM sales.bikestore_audit
ORDER BY id DESC;
GO
--------------------------------------------------------------------------------
-- STEP 5: Invalid Test 1 – Order with an Invalid Staff ID
-- This should fail due to the foreign key constraint on staff_id.
--------------------------------------------------------------------------------
PRINT '=== Invalid Test 1: Insert order with non-existing staff id ===';
BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO sales.orders (customer_id, order_status, order_date, required_date, shipped_date, store_id, staff_id)
    VALUES (NULL, 1, GETDATE(), DATEADD(day, 7, GETDATE()), NULL, 1, 9999);  -- Invalid staff_id

    DECLARE @NewOrderID INT = SCOPE_IDENTITY();

    -- Even if order_items were to be inserted, this transaction will fail.
    INSERT INTO sales.order_items (order_id, item_id, product_id, quantity, list_price, discount)
    VALUES (@NewOrderID, 1, 1, 2, 100.00, 0),
           (@NewOrderID, 2, 2, 1, 200.00, 0);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    PRINT 'Error in Invalid Test 1: ' + ERROR_MESSAGE();
END CATCH;
GO
--------------------------------------------------------------------------------
-- STEP 6: Invalid Test 2 – Order with an Invalid Product ID in Order_Items
-- This should fail due to the foreign key constraint on product_id.
--------------------------------------------------------------------------------
PRINT '=== Invalid Test 2: Insert order with non-existing product id in order_items ===';
BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO sales.orders (customer_id, order_status, order_date, required_date, shipped_date, store_id, staff_id)
    VALUES (NULL, 1, GETDATE(), DATEADD(day, 7, GETDATE()), NULL, 1, 1);

    DECLARE @NewOrderID INT = SCOPE_IDENTITY();

    -- Use an invalid product id (e.g. 9999) for the first order item.
    INSERT INTO sales.order_items (order_id, item_id, product_id, quantity, list_price, discount)
    VALUES (@NewOrderID, 1, 9999, 2, 100.00, 0),  -- Invalid product_id
           (@NewOrderID, 2, 2, 1, 200.00, 0);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    PRINT 'Error in Invalid Test 2: ' + ERROR_MESSAGE();
END CATCH;
GO
--------------------------------------------------------------------------------
-- STEP 7: Final Audit Table Check – Display All Audit Records
--------------------------------------------------------------------------------
SELECT *
FROM sales.bikestore_audit
ORDER BY id DESC;
GO
