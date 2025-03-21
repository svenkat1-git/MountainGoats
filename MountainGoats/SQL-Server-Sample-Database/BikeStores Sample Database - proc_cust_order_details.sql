USE BikeStores;
GO

DROP PROCEDURE IF EXISTS sales.proc_cust_order_details;
GO

CREATE PROCEDURE sales.proc_cust_order_details
    @CustomerID INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        o.order_id,
        (SELECT SUM(oi.quantity * oi.list_price)
         FROM sales.order_items oi
         WHERE oi.order_id = o.order_id) AS order_total,
        o.order_date,
        oi.item_id,
        p.product_name,
        oi.quantity,
        oi.list_price
    FROM sales.orders o
    INNER JOIN sales.order_items oi ON o.order_id = oi.order_id
    INNER JOIN production.products p ON p.product_id = oi.product_id
    WHERE o.customer_id = @CustomerID
      AND o.order_date BETWEEN @StartDate AND @EndDate;
END;
GO
