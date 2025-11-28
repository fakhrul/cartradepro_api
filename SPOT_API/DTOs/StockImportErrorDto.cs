namespace SPOT_API.DTOs
{
    public class StockImportErrorDto
    {
        public int Row { get; set; }
        public string Field { get; set; }
        public string Error { get; set; }
        public string Value { get; set; }
    }
}
