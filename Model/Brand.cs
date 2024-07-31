using System.Collections.Generic;

namespace SPOT_API.Models
{
    public class Brand : BaseModel
    {
        public string Name { get; set; }

        // Navigation property to represent the collection of related Models
        public ICollection<Model> Models { get; set; }
    }

}
