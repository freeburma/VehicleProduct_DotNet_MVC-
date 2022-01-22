using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/* 
    Reference for "nullable-warnings" 
    https://docs.microsoft.com/en-us/dotnet/csharp/nullable-warnings
 */
namespace VehicleProducts.Models
{
    public class VehicleModel
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto Generated primary key
        public int Id { get; set; }

        public string Title { get; set; } = "";

        [Display(Name = "Product Description")]
        public string? ProductDescription { get; set; } = "";



        [Display(Name = "File Path")]
        public string? FilePath { get; set; } = "";

        [Display(Name = "Image Name 1")]
        public string? ImageName_1 { get; set; } = "";



        [DataType(DataType.DateTime)]
        public DateTime StoreDate { get; set; } 

    }
}
