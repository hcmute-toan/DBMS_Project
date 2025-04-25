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
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } // For vw_ImportDetails
        public DateTime ImportDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
