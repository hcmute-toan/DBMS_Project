using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopShopProject.Models
{
    public class ProductLog
    {
        public int LogId { get; set; }
        public int? ProductId { get; set; } // Nullable to match schema
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime DeletedDate { get; set; }
        public string? DeletedBy { get; set; } // Nullable to match schema
    }
}
