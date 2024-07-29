using System;
using System.ComponentModel.DataAnnotations;

namespace Dashboard.Models
{
    public class ProductsDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Color { get; set; }

        [Required]
        public int Qty { get; set; }

        [Required]
        [StringLength(30)]
        public string Images { get; set; }

        [Required]

        public double Price { get; set; }
        public int ProductId { get; set; }




    }
}
