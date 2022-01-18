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
    public class ProductsController : Controller
    {
        private readonly MyDbContext db;
        public ProductsController(MyDbContext context)
        {
            db = context;
        }
        // GET: ProductsController
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var orders = db.Orders.Include(e => e.Employee).Include(e => e.Customer).Include(e => e.ShipViaNavigation).AsNoTracking().ToList();
            orders.Add(new Order { OrderId = null, ShowOrder = "----------------------------" });
            orders = orders.OrderBy(x => x.CustomerId ?? string.Empty).ToList();
            ViewData["Orders"] = new SelectList(orders, "OrderId", "ShowOrder");
            var customers = db.Customers.AsNoTracking().ToList();
            customers.Add(new Customer { CustomerId = null, CompanyName = "----------------------------" });
            customers = customers.OrderBy(x => x.CustomerId ?? string.Empty).ToList();
            ViewData["Customers"] = new SelectList(customers, "CustomerId", "CompanyName");
            ViewData["SupplierId"] = new SelectList(db.Suppliers, "SupplierId", "CompanyName");
            var products = db.Products.Include(e => e.Category).Include(e => e.Supplier).AsNoTracking();
            int pageSize = 30;
            if (TempData["alert-show"]?.ToString() == "true")
            {
                ViewData["alert-show"] = TempData["alert-show"];
                ViewData["alert-class"] = TempData["alert-class"];
                ViewData["alert-message"] = TempData["alert-message"];
            }
            ViewData["total-results"] = products.ToList().Count;
            return View(await PaginatedList<Product>.CreateAsync(products.OrderByDescending(x => x.ProductId), pageNumber ?? 1, pageSize));
        }

        // GET: ProductsController/Create
        public ActionResult Create(int SupplierId)
        {
            var supplier = db.Suppliers.FirstOrDefault(m => m.SupplierId == SupplierId);
            var product = new Product { SupplierId = SupplierId, Supplier = supplier };
            ViewData["CategoryId"] = new SelectList(db.Categories.AsNoTracking(), "CategoryId", "CategoryName");
            if (TempData["alert-show"]?.ToString() == "true")
            {
                ViewData["alert-show"] = TempData["alert-show"];
                ViewData["alert-class"] = TempData["alert-class"];
                ViewData["alert-message"] = TempData["alert-message"];
            }
            return View(product);
        }

        // POST: ProductsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["CategoryId"] = new SelectList(db.Categories.AsNoTracking(), "CategoryId", "CategoryName", product.CategoryId);
                    ViewData["alert-show"] = "true";
                    ViewData["alert-class"] = "danger";
                    ViewData["alert-message"] = "Greska prilikom kreiranja podataka!";
                    return View(product);
                }
                db.Add(product);
                db.SaveChanges();
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "success";
                TempData["alert-message"] = "Uspesno kreiran novi podatak!";
                return RedirectToAction("Create", new { SupplierId = product.SupplierId });
            }
            catch
            {
                ViewData["CategoryId"] = new SelectList(db.Categories.AsNoTracking(), "CategoryId", "CategoryName", product.CategoryId);
                ViewData["alert-show"] = "true";
                ViewData["alert-class"] = "danger";
                ViewData["alert-message"] = "Greska prilikom kreiranja podataka, problem sa bazom podataka!";
                return View(product);
            }
        }

        // GET: ProductsController/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom prikaza detalja, ID nije unet!";
                return RedirectToAction(nameof(Index));
            }
            var product = db.Products.Include(e => e.Category).Include(e => e.Supplier).FirstOrDefault(m => m.ProductId == id);
            if (product == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom prikaza detalja, ne postoji podatak za dati ID!";
                return RedirectToAction(nameof(Index));
            }

            return PartialView(product);
        }

        // GET: ProductsController/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom zahteva za izmenu podataka, ID nije unet!";
                return RedirectToAction(nameof(Index));
            }
            var product = db.Products.Include(e => e.Category).Include(e => e.Supplier).FirstOrDefault(m => m.ProductId == id);

            if (product == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom zahteva za izmenu podataka, ne postoji podatak za dati ID!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(db.Suppliers.AsNoTracking(), "SupplierId", "CompanyName", product.SupplierId);
            ViewData["CategoryId"] = new SelectList(db.Categories.AsNoTracking(), "CategoryId", "CategoryName", product.CategoryId);
            return PartialView(product);
        }

        // POST: ProductsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Product product)
        {
            if (id != product.ProductId)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom zahteva za izmenu podataka, ID nije isti kao kod podatka koji se menja!";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                db.Update(product);
                db.SaveChanges();
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "success";
                TempData["alert-message"] = "Uspesno izmenjen podatak!";
            }
            catch
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom izmene podataka, problem sa bazom podataka!";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ProductsController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom zahteva za brisanje podataka, ID nije unet!";
                return RedirectToAction(nameof(Index));
            }

            var product = db.Products.Include(e => e.Category).Include(e => e.Supplier).FirstOrDefault(m => m.ProductId == id);

            if (product == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom zahteva za brisanje podataka, ne postoji podatak za dati ID!";
                return RedirectToAction(nameof(Index));
            }
            return PartialView(product);
        }

        // POST: ProductsController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom zahteva za brisanje podataka, ID nije unet!";
                return RedirectToAction(nameof(Index));
            }
            var product = db.Products.AsNoTracking().FirstOrDefault(m => m.ProductId == id);
            try
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "success";
                TempData["alert-message"] = "Uspesno obrisan podatak!";
            }
            catch
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom brisanja podataka, problem sa bazom podataka!";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Filter(string CustomerId, int? OrderId)
        {
            var orders = db.Orders.Include(e => e.Employee).Include(e => e.Customer).Include(e => e.ShipViaNavigation).AsNoTracking().ToList();
            orders.Add(new Order { OrderId = null, ShowOrder = "----------------------------" });
            orders = orders.OrderBy(x => x.CustomerId ?? string.Empty).ToList();
            ViewData["Orders"] = new SelectList(orders, "OrderId", "ShowOrder");
            var customers = db.Customers.AsNoTracking().ToList();
            customers.Add(new Customer { CustomerId = null, CompanyName = "----------------------------" });
            customers = customers.OrderBy(x => x.CustomerId ?? string.Empty).ToList();
            ViewData["Customers"] = new SelectList(customers, "CustomerId", "CompanyName");
            ViewData["SupplierId"] = new SelectList(db.Suppliers, "SupplierId", "CompanyName");
            var filter = db.OrderDetails.Include(x => x.Product).Include(x=>x.Product.Category).Include(x => x.Product.Supplier).Include(x => x.Order).Where(x => x.Order.OrderId == (OrderId ?? x.Order.OrderId) && x.Order.CustomerId.Equals(CustomerId ?? x.Order.CustomerId)).Select(x=>x.Product).Distinct();
            int br= filter.ToList().Count;
            ViewData["total-results"] = br;
            if(br!=0)
                return View("Index", await PaginatedList<Product>.CreateAsync(filter.OrderByDescending(x => x.ProductId), 1, br));
            return View("Index", new PaginatedList<Product>(new List<Product>(),0,0,1));
        }

    }
}
