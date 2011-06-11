namespace Opuno.Brenn.Website.Controllers
{
    using System.Diagnostics.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using Opuno.Brenn.DataAccess;
    using Opuno.Brenn.Models;
    using Opuno.Brenn.Website.ViewModels;

    public class ExpenseController : Controller
    {
        private readonly BrennContext db = new BrennContext();

        //
        // GET: /Expense/?CollectionId=123

        public ViewResult Index(int collectionId)
        {
            ViewBag.CollectionId = collectionId;
            ViewBag.Collection = db.Trips.Find(collectionId);

            return View(db.Expenses.Where(e => e.TripId == collectionId).ToList());
        }

        //
        // GET: /Expense/Details/5

        public ViewResult IndexAjax(int collectionId)
        {
            var collection = db.Trips.Find(collectionId);

            ViewBag.Model =
                new
                    {
                        Collection = new { CollectionId = collectionId, collection.DisplayName },
                        Expenses = from e in collection.Expenses
                                   select
                                       new
                                           {
                                               CollectionId = e.TripId,
                                               e.ExpenseId,
                                               e.DisplayName,
                                               e.Amount,
                                               e.RecordDate,
                                               PaidByPersonId = e.ReceiverPersonId,
                                               PaidBy = new { e.Sender.DisplayName },
                                               UsedBy = from u in e.Receivers select new { u.PersonId, u.DisplayName }
                                           },
                        People = from p in db.People select new { p.PersonId, p.DisplayName }
                    };

            return View();
        }

        public ViewResult Details(int id)
        {
            return View(db.Expenses.Find(id));
        }

        //
        // GET: /Expense/Create

        private void CompleteViewModel(ExpenseViewModel viewModel)
        {
            viewModel.PossiblePayers = db.People.ToList();
            viewModel.PossibleUsers = db.People.ToList().Select(p => Tuple.Create(p, viewModel.Expense == null || viewModel.Expense.Receivers == null ? false : viewModel.Expense.Receivers.Any(u => u == p))).ToList();
        }

        public ActionResult Create(int collectionId)
        {
            var viewModel = new ExpenseViewModel
                {
                    Expense = new Expense() { TripId = collectionId, RecordDate = DateTime.Now, Receivers = new List<Person>() }
                };

            this.CompleteViewModel(viewModel);

            return View(viewModel);
        }

        //
        // POST: /Expense/Create

        [HttpPost]
        public ActionResult Create(int collectionId, Expense expense, FormCollection formCollection)
        {
            //throw new NotImplementedException();

            /*ar people = db.People.ToList();
            ViewBag.People = people;

            var selectedUsedBy = formCollection["usedBy"].Split(',').Select(x => int.Parse(x)).ToList();
            ViewBag.UsedByCheckList = people.ToDictionary(p => p, p => selectedUsedBy.Contains(p.PersonId));
 
            if (ModelState.IsValid)
            {
                ((Dictionary<int, bool>)ViewBag.UsedByCheckList).Where(x => x.Value)
                        .Select(x => db.People.Find(x.Key)).ToList().ForEach(x => expense.Receivers.Add(x));

                db.Expenses.Add(expense);
                db.SaveChanges();

                return RedirectToAction("Index", new { expense.CollectionId });  
            }*/

            expense.Receivers = new List<Person>(
                ParseCheckBoxes(
                    formCollection["PossibleUsers.kvp.Item1.PersonId"],
                    formCollection["PossibleUsers.kvp.Item2"]).Where(t => t.Item2).Select(t => db.People.Find(t.Item1)));
            expense.TripId = collectionId;


            if (ModelState.IsValid)
            {
                db.Expenses.Add(expense);
                db.SaveChanges();

                return RedirectToAction("Index", new { CollectionId = expense.TripId });  
            }

            var viewModel = new ExpenseViewModel { Expense = expense };

            this.CompleteViewModel(viewModel);

            return View(viewModel);
        }

        private static IEnumerable<Tuple<int, bool>> ParseCheckBoxes(string keys, string values)
        {
            var splitKeys = keys.Split(',');
            var splitValues = values.Split(',');
            var result = new List<Tuple<int, bool>>();
            var valueN = 0;

            foreach (var key in splitKeys.Select(t => Convert.ToInt32(t, CultureInfo.InvariantCulture)))
            {
                if (splitValues[valueN++] == "true")
                {
                    result.Add(Tuple.Create(key, true));
                    valueN++;
                    continue;
                }

                result.Add(Tuple.Create(key, false));
            }

            return result;
        }
        
        //
        // GET: /Expense/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View(db.Expenses.Find(id));
        }

        //
        // POST: /Expense/Edit/5

        [HttpPost]
        public ActionResult Edit(Expense expense)
        {
            if (ModelState.IsValid)
            {
                db.Entry(expense).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { collectionId = expense.TripId });
            }

            return View(expense);
        }

        //
        // GET: /Expense/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(db.Expenses.Find(id));
        }

        //
        // POST: /Expense/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Expense expense = db.Expenses.Find(id);
            db.Expenses.Remove(expense);
            db.SaveChanges();
            return RedirectToAction("Index", new { CollectionId = expense.TripId });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}