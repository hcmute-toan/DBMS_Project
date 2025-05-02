using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopShopProject.Models
{
    public class ExportDetail
    {
        public int ExportId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } // Non-nullable in vw_ExportDetails
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}