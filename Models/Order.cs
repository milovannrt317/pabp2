using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace projekat2.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }
        [NotMapped]
        private string showorder;
        public int? OrderId { get; set; }
        public string CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? OrderDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? RequiredDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ShippedDate { get; set; }
        public int? ShipVia { get; set; }
        public decimal? Freight { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }
        [NotMapped]
        public string ShowOrder { get
           {
                return ((showorder==null || showorder=="")?("Customer: " + (Customer?.CompanyName??"") + " | Employee: " +
                    (Employee?.FirstLastName??"") + " | Order date: " + String.Format("{0:dd.MM.yyyy.}", OrderDate) +
                    " | Required date: " + String.Format("{0:dd.MM.yyyy.}", RequiredDate) +
                    " | Shipped date: " + String.Format("{0:dd.MM.yyyy.}", ShippedDate) +
                    " | Shipper: " + (ShipViaNavigation?.CompanyName??"")):showorder);
            } set { showorder = value; }
        }

        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Shipper ShipViaNavigation { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
