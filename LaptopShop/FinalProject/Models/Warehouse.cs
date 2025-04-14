using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class Warehouse
    {
        public int WarehouseId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Slot { get; set; }

        // Navigation properties
        public List<Product> Products { get; set; }
        public List<Import> Imports { get; set; }
        public List<Export> Exports { get; set; }
    }
}
