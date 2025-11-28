using System.Collections.Generic;

namespace SPOT_API.DTOs
{
    public class StockImportResultDto
    {
        public int TotalRows { get; set; }
        public int ValidRows { get; set; }
        public int InvalidRows { get; set; }
        public List<StockImportErrorDto> Errors { get; set; }
        public ImportSummary Summary { get; set; }
        public string ValidationToken { get; set; } // Token for confirming import

        public StockImportResultDto()
        {
            Errors = new List<StockImportErrorDto>();
            Summary = new ImportSummary();
        }
    }

    public class ImportSummary
    {
        public int NewStocks { get; set; }
        public int DuplicateStockNo { get; set; }
        public int DuplicateChasisNo { get; set; }
        public int MissingReferenceData { get; set; }
        public int ValidationErrors { get; set; }
    }

    public class StockImportConfirmDto
    {
        public string ValidationToken { get; set; }
        public bool ImportOnlyValid { get; set; } // If true, import only valid rows
    }

    public class StockImportCompleteDto
    {
        public bool Success { get; set; }
        public int ImportedCount { get; set; }
        public int FailedCount { get; set; }
        public string Message { get; set; }
        public List<StockImportErrorDto> Errors { get; set; }

        public StockImportCompleteDto()
        {
            Errors = new List<StockImportErrorDto>();
        }
    }
}
