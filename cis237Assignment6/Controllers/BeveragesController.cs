using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237Assignment6.Models;

namespace cis237Assignment6.Controllers
{
    // Set as authorize so that all of this can't be accessed without being logged in
    [Authorize]
    public class BeveragesController : Controller
    {
        private BeverageAStaffenEntities db = new BeverageAStaffenEntities();

        // GET: Beverages
        public ActionResult Index()
        {
            // Variable to hold the beverage data
            DbSet<Beverage> BeveragesToFilter = db.Beverages;

            // Variables to hold the data from the session
            string filterName = "";
            string filterPack = "";
            string filterMin = "";
            string filterMax = "";

            // Min and max for the prices
            int min = 0;
            int max = 99999;

            // If there is something that is not just spaces in the session "name," assign to the corresponding variable
            if (Session["name"] != null && !String.IsNullOrWhiteSpace((string)Session["name"]))
            {
                filterName = (string)Session["name"];
            }
            // If there is something that is not just spaces in the session "pack," assign to the corresponding variable
            if (Session["pack"] != null && !String.IsNullOrWhiteSpace((string)Session["pack"]))
            {
                filterPack = (string)Session["pack"];
            }
            // If there is something that is not just spaces in the session "min," assign to the corresponding variable
            if (Session["min"] != null && !String.IsNullOrWhiteSpace((string)Session["min"]))
            {
                filterMin = (string)Session["min"];
                Int32.TryParse(filterMin, out min);
            }
            // If there is something that is not just spaces in the session "max," assign to the corresponding variable
            if (Session["max"] != null && !String.IsNullOrWhiteSpace((string)Session["max"]))
            {
                filterMax = (string)Session["max"];
                Int32.TryParse(filterMax, out max);
            }

            // Assign to filtered any beverages that are greater than or equal to the min, less than or equal to the max,
            // contain the filterName, and contain the filterPack.
            IEnumerable<Beverage> filtered = BeveragesToFilter.Where(beverage => beverage.price >= min &&
                                                                                 beverage.price <= max &&
                                                                                 beverage.name.Contains(filterName) &&
                                                                                 beverage.pack.Contains(filterPack));
            // Assign filtered to the list finalFiltered
            IEnumerable<Beverage> finalFiltered = filtered.ToList();

            // Assign the filter info to the viewbag so they can be used in the view to display the filter info in the text boxes
            ViewBag.filterName = filterName;
            ViewBag.filterPack = filterPack;
            ViewBag.filterMin = filterMin;
            ViewBag.filterMax = filterMax;

            // Return the view of the filtered beverages
            return View(finalFiltered);
        }

        // GET: Beverages/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // GET: Beverages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Beverages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Beverages.Add(beverage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(beverage);
        }

        // GET: Beverages/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: Beverages/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // Filter method that can only redirect to the index from a form request. Sets up the info to be filtered.
        [HttpPost]
        public ActionResult Filter()
        {
            // Gets the data filter info from the form. 
            string name = Request.Form.Get("name");
            string pack = Request.Form.Get("pack");
            string min = Request.Form.Get("min");
            string max = Request.Form.Get("max");

            // Store the form data in the session to be filtered later
            Session["name"] = name;
            Session["pack"] = pack;
            Session["min"] = min;
            Session["max"] = max;

            // Redirect the user to the index page
            return RedirectToAction("Index");
        }
    }
}
