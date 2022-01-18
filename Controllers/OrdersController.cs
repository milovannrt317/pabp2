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
    public class OrdersController : Controller
    {
        private readonly MyDbContext db;

        public OrdersController(MyDbContext context)
        {
            db = context;
        }
        // GET: OrdersController
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var orders = db.Orders.Include(e => e.Employee).Include(e => e.Customer).Include(e => e.ShipViaNavigation).AsNoTracking();

            int pageSize = 30;
            if (TempData["alert-show"]?.ToString() == "true")
            {
                ViewData["alert-show"] = TempData["alert-show"];
                ViewData["alert-class"] = TempData["alert-class"];
                ViewData["alert-message"] = TempData["alert-message"];
            }
            ViewData["total-results"] = orders.ToList().Count;
            return View(await PaginatedList<Order>.CreateAsync(orders.OrderByDescending(x=>x.OrderId), pageNumber ?? 1, pageSize));
        }
        // GET: OrdersController/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom prikaza detalja, ID nije unet!";
                return RedirectToAction(nameof(Index));
            }

            var order = db.Orders.Include(e => e.Employee).Include(e => e.Customer).Include(e => e.ShipViaNavigation).FirstOrDefault(m => m.OrderId == id);

            if (order == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom prikaza detalja, ne postoji podatak za dati ID!";
                return RedirectToAction(nameof(Index));
            }
            return PartialView(order);
        }

        // GET: OrdersController/Create
        public ActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(db.Customers.AsNoTracking(), "CustomerId", "CompanyName");
            ViewData["EmployeeId"] = new SelectList(db.Employees.AsNoTracking(), "EmployeeId", "FirstLastName");
            ViewData["ShipVia"] = new SelectList(db.Shippers.AsNoTracking(), "ShipperId", "CompanyName");
            return PartialView();
        }

        // POST: OrdersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Order order)
        {
            try
            {
                order.ShipName = db.Customers.Where(x=>x.CustomerId==order.CustomerId).FirstOrDefault().CompanyName;
                db.Add(order);
                db.SaveChanges();
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "success";
                TempData["alert-message"] = "Uspesno kreiran novi podatak!";
            }
            catch
            {
                //ViewData["CustomerId"] = new SelectList(db.Customers.AsNoTracking(), "CustomerId", "CompanyName", order.CustomerId);
                //ViewData["EmployeeId"] = new SelectList(db.Employees.AsNoTracking(), "EmployeeId", "FirstLastName", order.EmployeeId);
                //ViewData["ShipVia"] = new SelectList(db.Shippers.AsNoTracking(), "ShipperId", "CompanyName", order.ShipVia);
                //return View(order);
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom kreiranja podataka, problem sa bazom podataka!";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: OrdersController/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom zahteva za izmenu podataka, ID nije unet!";
                return RedirectToAction(nameof(Index));
            }
            var order = db.Orders.Include(e => e.Employee).Include(e => e.Customer).Include(e => e.ShipViaNavigation).FirstOrDefault(m => m.OrderId == id);
            if (order == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom zahteva za izmenu podataka, ne postoji podatak za dati ID!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(db.Customers.AsNoTracking(), "CustomerId", "CompanyName", order.CustomerId);
            ViewData["EmployeeId"] = new SelectList(db.Employees.AsNoTracking(), "EmployeeId", "FirstLastName", order.EmployeeId);
            ViewData["ShipVia"] = new SelectList(db.Shippers.AsNoTracking(), "ShipperId", "CompanyName", order.ShipVia);
            return PartialView(order);
        }

        // POST: OrdersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Order order)
        {
            if (id != order.OrderId)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom zahteva za izmenu podataka, ID nije isti kao kod podatka koji se menja!";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                order.ShipName = db.Customers.Where(x => x.CustomerId == order.CustomerId).FirstOrDefault().CompanyName;
                db.Update(order);
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

        // GET: OrdersController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom zahteva za brisanje podataka, ID nije unet!";
                return RedirectToAction(nameof(Index));
            }

            var order = db.Orders.Include(e => e.Employee).Include(e => e.Customer).Include(e => e.ShipViaNavigation).FirstOrDefault(m => m.OrderId == id);

            if (order == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom zahteva za brisanje podataka, ne postoji podatak za dati ID!";
                return RedirectToAction(nameof(Index));
            }
            return PartialView(order);
        }

        // POST: OrdersController/Delete/5
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
            var order = db.Orders.FirstOrDefault(m => m.OrderId == id);
            try
            {
                db.Orders.Remove(order);
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
    }
}
