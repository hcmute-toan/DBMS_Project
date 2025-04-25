using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopShopProject.Models
{
    public class ImportDetail
    {
        public int ImportId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } // For vw_ImportDetails
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
