using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projekat2.Models;
using System.Linq;
using System.Threading.Tasks;

namespace projekat2.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly MyDbContext db;

        public EmployeesController(MyDbContext context)
        {
            db = context;
        }
        // GET: EmployeesController
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var employees = db.Employees.Include(e => e.ReportsToNavigation).AsNoTracking();
            int pageSize = 5;
            if(TempData["alert-show"]?.ToString()=="true")
            {
                ViewData["alert-show"] = TempData["alert-show"];
                ViewData["alert-class"] = TempData["alert-class"];
                ViewData["alert-message"] = TempData["alert-message"];
            }
            ViewData["total-results"] = employees.ToList().Count;
            return View(await PaginatedList<Employee>.CreateAsync(employees.OrderByDescending(x => x.EmployeeId), pageNumber ?? 1, pageSize));
        }

        // GET: EmployeesController/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom prikaza detalja, ID nije unet!";
                return RedirectToAction(nameof(Index));
            }
            
            var employee = db.Employees.Include(e => e.ReportsToNavigation).FirstOrDefault(m => m.EmployeeId == id);

            if (employee == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom prikaza detalja, ne postoji podatak za dati ID!";
                return RedirectToAction(nameof(Index));
            }
            
            return PartialView(employee);
        }

        // GET: EmployeesController/Create
        public ActionResult Create()
        {
            var employees = db.Employees.AsNoTracking().ToList();
            employees.Add(new Employee { EmployeeId = null, FirstLastName = "----------------------------" });
            employees=employees.OrderBy(x => x.EmployeeId??int.MinValue).ToList();
            ViewData["ReportsTo"] = new SelectList(employees, "EmployeeId", "FirstLastName");
            return PartialView();
        }

        // POST: EmployeesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                db.Add(employee);
                db.SaveChanges();
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "success";
                TempData["alert-message"] = "Uspesno kreiran novi podatak!";
            }
            catch
            {
                //var employees = db.Employees.AsNoTracking().ToList();
                //employees.Add(new Employee { EmployeeId = null, FirstLastName = "----------------------------" });
                //employees = employees.OrderBy(x => x.EmployeeId ?? int.MinValue).ToList();
                //ViewData["ReportsTo"] = new SelectList(employees, "EmployeeId", "FirstLastName", employee.ReportsTo);
                //return PartialView(employee);
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom kreiranja podataka, problem sa bazom podataka!";
            }
            return RedirectToAction(nameof(Index));
            
        }

        // GET: EmployeesController/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom zahteva za izmenu podataka, ID nije unet!";
                return RedirectToAction(nameof(Index));
            }
            var employee = db.Employees.Include(e => e.ReportsToNavigation).FirstOrDefault(m => m.EmployeeId == id);

            if (employee == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom zahteva za izmenu podataka, ne postoji podatak za dati ID!";
                return RedirectToAction(nameof(Index));
            }
            var employees = db.Employees.Where(x => x.EmployeeId != employee.EmployeeId).AsNoTracking().ToList();
            employees.Add(new Employee { EmployeeId = null, FirstLastName = "----------------------------" });
            employees = employees.OrderBy(x => x.EmployeeId ?? int.MinValue).ToList();
            ViewData["ReportsTo"] = new SelectList(employees, "EmployeeId", "FirstLastName", employee.ReportsTo);
            return PartialView(employee);
        }

        // POST: EmployeesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom zahteva za izmenu podataka, ID nije isti kao kod podatka koji se menja!";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                db.Update(employee);
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

        // GET: EmployeesController/Delete/5
        public ActionResult Delete(int ?id)
        {
            if (id == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom zahteva za brisanje podataka, ID nije unet!";
                return RedirectToAction(nameof(Index));
            }

            var employee = db.Employees.Include(e => e.ReportsToNavigation).FirstOrDefault(m => m.EmployeeId == id);
            if (employee == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "danger";
                TempData["alert-message"] = "Greska prilikom zahteva za brisanje podataka, ne postoji podatak za dati ID!";
                return RedirectToAction(nameof(Index));
            }
            return PartialView(employee);
        }

        // POST: EmployeesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int ?id)
        {
            if (id == null)
            {
                TempData["alert-show"] = "true";
                TempData["alert-class"] = "warning";
                TempData["alert-message"] = "Greska prilikom zahteva za brisanje podataka, ID nije unet!";
                return RedirectToAction(nameof(Index));
            }
            var employee = db.Employees.Include(e => e.ReportsToNavigation).FirstOrDefault(m => m.EmployeeId == id);
            try
            {
                db.Employees.Remove(employee);
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
