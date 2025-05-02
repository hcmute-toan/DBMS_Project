using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopShopProject.Models
{
    public class Export
    {
        public int ExportId { get; set; }
        public int? CustomerId { get; set; } // Nullable to match database (ON DELETE SET NULL)
        public string? CustomerName { get; set; } // Nullable for vw_ExportDetails
        public DateTime ExportDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}