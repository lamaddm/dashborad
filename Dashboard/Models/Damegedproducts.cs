using System;
using System.ComponentModel.DataAnnotations;

namespace Dashboard.Models
{
    public class Damegedproducts
    {
        [Key]
        public int Id { get; set; }
        public int Qty { get; set; }
        public int ProductId { get; set; }
    }
}
