using System;

namespace SPOT_API.Models
{
    public class Advertisement : BaseModel
    {
        public bool MudahHadAdviertized { get; set; }
        public DateTime MudahStartDate { get; set; } = DateTime.SpecifyKind(DateTime.Now.Date, DateTimeKind.Unspecified);

        public DateTime MudahEndDate { get; set; } = DateTime.Now.ToUniversalTime();

        public bool CarListHadAdviertized { get; set; }
        public DateTime CarListStartDate { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime CarListEndDate { get; set; } = DateTime.Now.ToUniversalTime();

        public bool CariCarzHadAdviertized { get; set; }

        public DateTime CariCarzStartDate { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime CariCarzEndDate { get; set; } = DateTime.Now.ToUniversalTime();

    }
}
