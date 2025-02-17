﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project2.Models;

namespace Project2.Controllers
{
    public class EntriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //Success view for a successful applicaiton submission
        public ActionResult Success()
        {
            return View();
        }

        // GET: Entries
        [Authorize]
        public ActionResult Index(string search)
        {

            //Method for searhc box; data will display if matching SSN or Last Name is found in Db
            var entries = from i in db.Entries select i;

            if (!String.IsNullOrEmpty(search))
            {
                entries = entries.Where(e => e.LastName.Contains(search) || e.SSN.Contains(search));
            }

            return View(entries.ToList());
        }

        // GET: Entries/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }

        // GET: Entries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Entries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NameFirst,MiddleName,LastName,SSN,Email,HomePhone,CellPhone,Street,City,State,Zipcode,DOB,GenderID,HighSchoolName,HighSchoolCity,GradDate,GPA,Math,Verbal,MajorsInterest,EnrollSeason,EnrollYear, Decision")] Entry entry)
        {
            Entry MatchingSSN = db.Entries.Where(cm => string.Compare(cm.SSN, entry.SSN, true)==0).FirstOrDefault();
            Entry MatchingEmail = db.Entries.Where(cm => string.Compare(cm.Email, entry.Email, true) == 0).FirstOrDefault();
            var Score = entry.Math + entry.Verbal;
            var GPA = entry.GPA;
            if(Score < 1000)
            {
                ModelState.AddModelError("SAT Score", "You do not meet the minimum SAT score requirements (SAT score is less than 1000)");
                return View(entry);
            }
            
            if(GPA < 3.0)
            {
                ModelState.AddModelError("GPA", "You do not meet the minimum GPA requirements (GPA is less than 3.0)");
                return View(entry);
            }

            if (MatchingSSN != null)
            {
                ModelState.AddModelError("SSN", "SSN must be unique");
                return View(entry);
            }
            if (MatchingEmail != null)
            {
                ModelState.AddModelError("Email", "Email address must be unique");
                return View(entry);
            }

            if (ModelState.IsValid)
            {
                db.Entries.Add(entry);
                db.SaveChanges();
                return RedirectToAction("Success");

            }

            return View(entry);
        }

        // GET: Entries/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }

        // POST: Entries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,NameFirst,MiddleName,LastName,SSN,Email,HomePhone,CellPhone,Street,City,State,Zipcode,DOB,GenderID,HighSchoolName,HighSchoolCity,GradDate,GPA,Math,Verbal,MajorsInterest,EnrollSeason,EnrollYear, Decision")] Entry entry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(entry);
        }

        // GET: Entries/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }

        // POST: Entries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Entry entry = db.Entries.Find(id);
            db.Entries.Remove(entry);
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
