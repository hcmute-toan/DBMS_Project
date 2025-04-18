using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class ProductCategory
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public Category Category { get; set; }
    }
}
