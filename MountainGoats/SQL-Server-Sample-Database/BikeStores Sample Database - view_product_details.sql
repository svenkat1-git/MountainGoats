USE BikeStores;

GO

CREATE OR ALTER VIEW production.view_product_details
AS
SELECT 
    p.product_id,
    p.product_name,
    p.brand_id,
    b.brand_name,
    p.category_id,
    c.category_name,
    p.model_year,
    p.list_price,
    s.store_id,
    s.store_name,
    s.phone AS store_phone,
    s.zip_code AS store_zip,
    st.quantity
FROM production.products p
    INNER JOIN production.brands b 
        ON p.brand_id = b.brand_id
    INNER JOIN production.categories c 
        ON p.category_id = c.category_id
    INNER JOIN production.stocks st 
        ON p.product_id = st.product_id
    INNER JOIN sales.stores s
        ON st.store_id = s.store_id;
GO