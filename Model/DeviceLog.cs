using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class DeviceLog : BaseDapper
    {
        [Key]
        public int Id { get; set; }
        public string deviceid { get; set; }

        [NotMapped]
        public DateTime CreatedDateTime
        {
            get
            {
                return createdate.AddHours(8);
            }
        }
        public DateTime createdate { get; set; }
        public string data { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string alarm { get; set; }
        public string batt { get; set; }
        public string mode { get; set; }
        public string fw_ver { get; set; }
        public string hw_ver { get; set; }
        public string wifi_rssi { get; set; }
        public string ble_rssi { get; set; }

        [NotMapped]
        public DateTime DateTime
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(date_time))
                        return DateTime.MinValue;
                    if (date_time == "NaN")
                        return DateTime.MinValue;
                    if (date_time == "undefined")
                        return DateTime.MinValue;

                    if (date_time == "Invalid date")
                        return DateTime.MinValue;
                    

                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(date_time));
                    return dateTimeOffset.DateTime;
                }
                catch (Exception ex)
                {
                }
                return DateTime.MinValue;
            }
        }
        public string date_time { get; set; }
        public string prox_id_list { get; set; }
        public string dashboard_status { get; set; }

    }
}
