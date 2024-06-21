using System;
namespace SPOT_API.Models
{
    /// <summary>
    /// Response Object for API
    /// </summary>
    public class Response
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
