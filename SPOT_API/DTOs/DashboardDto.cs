using System.Collections.Generic;

namespace CarTradePro.API.DTOs
{
    public class DashboardDto
    {
        public SalesData LastYearSales { get; set; }
        public SalesData ThisYearSales { get; set; }
        public StockStatusData StockStatus { get; set; }

        public List<TopSellingModelData> TopSellingModels { get; set; }
        public List<TopSellingModelData> LastYearTopSellingModels { get; set; }
        
    }
    public class TopSellingModelData
    {
        public string Name { get; set; }
        public int UnitsSold { get; set; }
    }

    public class SalesData
    {
        public int Total { get; set; }
        public float Amount { get; set; }
    }

    public class StockStatusData
    {
        public int Available { get; set; }
        public int InProgress { get; set; }
    }
}
