namespace SPOT_API.Models
{
    public class ShowRoom : BaseModel
    {
        public string LotNo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPhone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string Code { get; set; }
    }
}
