using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projekat2.Models;

namespace projekat2.Controllers
{
    public class ProductsOrdersSuppliersController : Controller
    {
        private readonly MyDbContext db;
        public ProductsOrdersSuppliersController(MyDbContext context)
        {
            db = context;
        }
        // GET: ProductsOrdersSuppliersController
        //public async Task<IActionResult> Index(int? SupplierId, int? OrderId,int? pageNumber)
        public ActionResult Index(int? SupplierId, int? OrderId)
        {
            var products = db.Products.AsNoTracking().ToList();
            
            var orders = db.Orders.Include(e => e.Employee).Include(e => e.Customer).Include(e => e.ShipViaNavigation).AsNoTracking().ToList();
            orders.Add(new Order { OrderId = null, ShowOrder = "----------------------------" });
            orders = orders.OrderBy(x => x.OrderId ?? int.MinValue).ToList();
            ViewData["Orders"] = new SelectList(orders, "OrderId", "ShowOrder");
            
            var suppliers = db.Suppliers.AsNoTracking().ToList();
            suppliers.Add(new Supplier { SupplierId = null, CompanyName = "----------------------------" });
            suppliers = suppliers.OrderBy(x => x.SupplierId ?? int.MinValue).ToList();
            ViewData["Suppliers"] = new SelectList(suppliers, "SupplierId", "CompanyName");
            var orderDetails = db.OrderDetails.Include(x => x.Product).AsNoTracking().ToList();

            //var rez = from od in orderDetails
            //          group od.ProductId by od.OrderId into g
            //          select new ProductsByOrdersSuppliers { ProductCount = g.Count(), OrderId = g.Key, OrderDetails = orders.Where(x => x.OrderId == g.Key).FirstOrDefault().ShowOrder, SupplierId = (int)products.Where(x => x.ProductId == orderDetails.Where(x => x.OrderId == g.Key).First().ProductId).First().SupplierId, SupplierDetails = suppliers.Where(x => x.SupplierId == products.Where(x=>x.ProductId==orderDetails.Where(x=>x.OrderId==g.Key).FirstOrDefault().ProductId).FirstOrDefault().SupplierId).FirstOrDefault().CompanyName };
            var rez = from od in orderDetails
                      group od.ProductId by new { od.OrderId, od.Product.SupplierId } into g
                      select new ProductsByOrdersSuppliers { ProductCount = g.Count(), OrderId = g.Key.OrderId, OrderDetails = orders.FirstOrDefault(x => x.OrderId == g.Key.OrderId).ShowOrder, SupplierId = (int)g.Key.SupplierId, SupplierDetails = suppliers.FirstOrDefault(x => x.SupplierId == g.Key.SupplierId).CompanyName };
            
            if (OrderId!=null || SupplierId != null)
            {
                rez = rez.Where(x => x.OrderId == (OrderId ?? x.OrderId) && x.SupplierId == (SupplierId ?? x.SupplierId));
            }
            
            //int pageSize = 50;
            ViewData["total-results"] = rez.ToList().Count;
            return View(rez.OrderByDescending(x => x.ProductCount));
        }

    }
}
