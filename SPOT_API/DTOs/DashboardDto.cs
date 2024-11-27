using System.Collections.Generic;

namespace CarTradePro.API.DTOs
{
    public class DashboardDto
    {
        public SalesData LastYearSales { get; set; }
        public SalesData ThisYearSales { get; set; }
        public ArrivaleStatusData ArrivalStatus { get; set; }

        public List<TopSellingModelData> TopSellingModels { get; set; }
        public List<TopSellingModelData> LastYearTopSellingModels { get; set; }
        public List<UnitsByMonth> UnitsRegisteredByMonth { get; set; }
        public List<UnitsByYear> UnitsRegisteredByYear { get; set; }
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

    public class ArrivaleStatusData
    {
        public int Incoming { get; set; }
        public int Received { get; set; }
    }

    public class UnitsByMonth
    {
        public int Month { get; set; }
        public int Count { get; set; }
    }

    public class UnitsByYear
    {
        public int Year { get; set; }
        public int Count { get; set; }
    }
}
