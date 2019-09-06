using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SocialMediaQuery.Models;

namespace SocialMediaQuery.Controllers
{
    public class DataSourcesController : Controller
    {
        private DEV_DB_LICEntities2 db = new DEV_DB_LICEntities2();

        // GET: DataSources
        public ActionResult Index()
        {
            return View(db.DataSources.ToList());
        }

        // GET: DataSources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSource dataSource = db.DataSources.Find(id);
            if (dataSource == null)
            {
                return HttpNotFound();
            }
            return View(dataSource);
        }

        // GET: DataSources/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DataSources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] DataSource dataSource)
        {
            if (ModelState.IsValid)
            {
                db.DataSources.Add(dataSource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dataSource);
        }

        // GET: DataSources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSource dataSource = db.DataSources.Find(id);
            if (dataSource == null)
            {
                return HttpNotFound();
            }
            return View(dataSource);
        }

        // POST: DataSources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] DataSource dataSource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dataSource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dataSource);
        }

        // GET: DataSources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSource dataSource = db.DataSources.Find(id);
            if (dataSource == null)
            {
                return HttpNotFound();
            }
            return View(dataSource);
        }

        // POST: DataSources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DataSource dataSource = db.DataSources.Find(id);
            db.DataSources.Remove(dataSource);
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
    }
}
