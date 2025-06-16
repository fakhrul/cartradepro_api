using System.ComponentModel.DataAnnotations;

namespace SPOT_API.DTOs
{
    public class UpdateStockNoDto
    {
        [Required]
        [StringLength(50)] // Adjust based on your database column length
        public string NewStockNo { get; set; }
    }
}
