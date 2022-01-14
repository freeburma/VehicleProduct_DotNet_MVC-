using System.ComponentModel.DataAnnotations;

namespace VehicleProducts.Models
{
    public class VehicleModel
    {
        [Key]
        [Required]
        public int Id { get; set; } 

        public string Title { get; set; }
        public string ProductDescription { get; set; }
        public string FilePath { get; set; }
        public string ImageName_1 { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StoreDate { get; set; }

    }
}
