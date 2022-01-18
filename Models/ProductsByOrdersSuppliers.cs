using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projekat2.Models
{
    public class ProductsByOrdersSuppliers
    {
        public int ProductCount { get; set; }
        public int OrderId { get; set; }
        public string OrderDetails { get; set; }
        public int SupplierId { get; set; }
        public string SupplierDetails { get; set; }

    }
}
