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
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } // For vw_ExportDetails
        public DateTime ExportDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
