IF EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = 'StockApp_DB'
) DROP DATABASE StockApp_DB;

CREATE DATABASE StockApp_DB;