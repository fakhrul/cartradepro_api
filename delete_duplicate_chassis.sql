-- Find stocks with duplicate chassis numbers
SELECT s.Id, s.StockNo, v.ChasisNo 
FROM "Stocks" s
INNER JOIN "Vehicles" v ON s."VehicleId" = v."Id"
WHERE v."ChasisNo" IN ('ABC123XYZ456', 'DEF789GHI012', 'JKL345MNO678');
