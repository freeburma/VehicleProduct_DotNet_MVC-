using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleProducts.Models
{
    public class VehicleModel
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto Generated primary key
        public int Id { get; set; } 

        public string Title { get; set; }
        public string ProductDescription { get; set; }
        public string FilePath { get; set; }
        public string ImageName_1 { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StoreDate { get; set; }

    }
}
