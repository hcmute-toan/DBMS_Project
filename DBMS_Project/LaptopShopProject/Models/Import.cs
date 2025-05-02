using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopShopProject.Models
{
    public class Import
    {
        public int ImportId { get; set; }
        public int? SupplierId { get; set; } // Nullable to match database (ON DELETE SET NULL)
        public string? SupplierName { get; set; } // Nullable for vw_ImportDetails
        public DateTime ImportDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
