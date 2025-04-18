using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class ProductSpecs
    {
        public int ProductId { get; set; }
        public int SpecId { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public Specs Specs { get; set; }
    }
}
