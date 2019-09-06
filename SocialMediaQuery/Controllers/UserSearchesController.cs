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
    public class UserSearchesController : Controller
    {
        private DEV_DB_LICEntities2 db = new DEV_DB_LICEntities2();

        // GET: UserSearches
        public ActionResult Index()
        {
            var userSearches = db.UserSearches.Include(u => u.DataSource);
            return View(userSearches.ToList());
        }

        // GET: UserSearches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSearch userSearch = db.UserSearches.Find(id);
            if (userSearch == null)
            {
                return HttpNotFound();
            }
            return View(userSearch);
        }

        // GET: UserSearches/Create
        public ActionResult Create()
        {
            ViewBag.DataSourceId = new SelectList(db.DataSources, "Id", "Name");
            return View();
        }

        // POST: UserSearches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,QueryText,CreatedOn,DataSourceId,JsonResult")] UserSearch userSearch)
        {
            if (ModelState.IsValid)
            {
                db.UserSearches.Add(userSearch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DataSourceId = new SelectList(db.DataSources, "Id", "Name", userSearch.DataSourceId);
            return View(userSearch);
        }

        // GET: UserSearches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSearch userSearch = db.UserSearches.Find(id);
            if (userSearch == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataSourceId = new SelectList(db.DataSources, "Id", "Name", userSearch.DataSourceId);
            return View(userSearch);
        }

        // POST: UserSearches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,QueryText,CreatedOn,DataSourceId,JsonResult")] UserSearch userSearch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userSearch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DataSourceId = new SelectList(db.DataSources, "Id", "Name", userSearch.DataSourceId);
            return View(userSearch);
        }

        // GET: UserSearches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSearch userSearch = db.UserSearches.Find(id);
            if (userSearch == null)
            {
                return HttpNotFound();
            }
            return View(userSearch);
        }

        // POST: UserSearches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserSearch userSearch = db.UserSearches.Find(id);
            db.UserSearches.Remove(userSearch);
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
