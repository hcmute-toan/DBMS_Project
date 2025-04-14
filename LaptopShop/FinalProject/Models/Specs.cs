using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class Specs
    {
        public int SpecId { get; set; }
        public string SpecName { get; set; }

        // Navigation properties
        public List<ProductSpecs> ProductSpecs { get; set; }
    }
}
