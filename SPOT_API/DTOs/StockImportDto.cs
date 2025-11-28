using System;

namespace SPOT_API.DTOs
{
    public class StockImportDto
    {
        // Mandatory Fields
        public string StockNo { get; set; }
        public string ChasisNo { get; set; }

        // Core Vehicle Fields
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string VehicleTypeName { get; set; }
        public string EngineNo { get; set; }
        public string EngineCapacity { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Color { get; set; }

        // Pricing Fields
        public decimal RecommendedSalePrice { get; set; }
        public decimal MinimumSalePrice { get; set; }

        // Purchase Fields
        public string SupplierName { get; set; }
        public decimal VehiclePriceSupplierCurrency { get; set; }
        public decimal VehiclePriceLocalCurrency { get; set; }
        public string SupplierCurrency { get; set; }

        // Optional Fields
        public string ShowRoomLotNo { get; set; }
        public string LocationCode { get; set; }
        public string ArrivalState { get; set; } // "Incoming" or "Received"

        // Internal tracking
        public int RowNumber { get; set; }
    }
}
