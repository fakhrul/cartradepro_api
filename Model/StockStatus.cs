namespace SPOT_API.Models
{
    /// <summary>
    /// preset vehcile status
/*
Draft
Open
Booked(linked with customer)
LOU(letter of undertaking)
Pending Duty Tax
Incoming Stock
Sold
*/
    /// </summary>
    public class StockStatus : BaseModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }

}
