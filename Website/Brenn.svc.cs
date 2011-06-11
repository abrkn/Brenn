namespace Opuno.Brenn.Website
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Threading;
    using System.Transactions;

    using Opuno.Brenn.DataAccess;
    using Opuno.Brenn.Models;
    using Opuno.Brenn.Sync;

    [ServiceContract(Namespace = "")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Brenn
    {
        private readonly object changeSetLock = new object();

        private int sharedChangeSetN;

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public SyncResponse Sync(SyncRequest request)
        {
            lock (this.changeSetLock)
            {
                if (this.sharedChangeSetN == 0)
                {
                    using (var db = new BrennContext())
                    {
                        this.sharedChangeSetN =
                            new[]
                                {
                                    db.Expenses.Max<Expense, int?>(e => e.ChangeSetN),
                                    db.Trips.Max<Trip, int?>(e => e.ChangeSetN),
                                    db.People.Max<Person, int?>(e => e.ChangeSetN)
                                }.Max().GetValueOrDefault(1);
                    }
                }
            }

            var anyUpdates = (request.ExpenseUpdates == null ? 0 : request.ExpenseUpdates.Count) +
                             (request.TripUpdates == null ? 0 : request.TripUpdates.Count) +
                             (request.PersonUpdates == null ? 0 : request.PersonUpdates.Count) > 0;

            var changeSetN = anyUpdates ? Interlocked.Increment(ref this.sharedChangeSetN) : this.sharedChangeSetN;

            var response = new SyncResponse
                {
                    ExpenseUpdateResults = new List<UpdateResult>(),
                    TripUpdateResults = new List<UpdateResult>(),
                    PersonUpdateResults = new List<UpdateResult>(),
                    ServerChangeSetN = changeSetN
                };

            var updatedExpenseIds = new List<int>();
            var updatedTripIds = new List<int>();
            var updatedPersonIds = new List<int>();

            PerformExpenseUpdates(request, response, updatedExpenseIds);
            PerformTripUpdates(request, response, updatedTripIds);
            PerformPersonUpdates(request, response, updatedPersonIds);

            using (var db = new BrennContext())
            {
                ((IObjectContextAdapter)db).ObjectContext.ContextOptions.ProxyCreationEnabled = false;

                response.Expenses =
                    new List<Expense>(
                        db.Expenses.Include("Receivers").Where(
                            e => e.ChangeSetN > request.ClientChangeSetN && !updatedExpenseIds.Contains(e.ExpenseId)));

                response.Trips =
                    new List<Trip>(
                        db.Trips
                        .Where(
                            e => e.ChangeSetN > request.ClientChangeSetN && !updatedTripIds.Contains(e.TripId)));

                response.Trips.ForEach(t => t.Expenses = null);

                response.People =
                    new List<Person>(
                        db.People
                        .Where(e => e.ChangeSetN > request.ClientChangeSetN && !updatedPersonIds.Contains(e.PersonId)));
            }

            return response;
        }

        private static void PerformPersonUpdates(
            SyncRequest request, SyncResponse response, ICollection<int> updatedPersonIds)
        {
            if (request.PersonUpdates == null)
            {
                return;
            }

            foreach (var personUpdate in request.PersonUpdates)
            {
                using (var db = new BrennContext())
                {
                    using (var scope = new TransactionScope())
                    {
                        var person = db.People.Find(personUpdate.Person.PersonId);

                        if (person == null)
                        {
                            response.PersonUpdateResults.Add(
                                new UpdateResult { Error = UpdateError.DeletedOnServer, ClientRowId = personUpdate.Person.RowId });
                            continue;
                        }

                        if (person.RowId != personUpdate.ClientRowId)
                        {
                            response.PersonUpdateResults.Add(
                                new UpdateResult { Error = UpdateError.ModifiedOnServer, ClientRowId = personUpdate.Person.RowId });
                            continue;
                        }

                        var entry = db.Entry(person);
                        entry.CurrentValues.SetValues(personUpdate.Person);

                        db.SaveChanges();

                        scope.Complete();
                    }

                    response.PersonUpdateResults.Add(
                        new UpdateResult { Error = null, ClientRowId = personUpdate.Person.RowId });

                    updatedPersonIds.Add(personUpdate.Person.PersonId);
                }
            }
        }

        private static void PerformTripUpdates(
            SyncRequest request, SyncResponse response, ICollection<int> updatedTripIds)
        {
            if (request.TripUpdates == null)
            {
                return;
            }

            foreach (var tripUpdate in request.TripUpdates)
            {
                if (tripUpdate.ClientRowId == default(Guid))
                {
                    throw new Exception("The client row id must be set.");
                }

                using (var db = new BrennContext())
                {
                    using (var scope = new TransactionScope())
                    {
                        if (tripUpdate.Trip.TripId == default(int))
                        {
                            db.Trips.Add(tripUpdate.Trip);
                        }
                        else
                        {
                            var trip = db.Trips.Find(tripUpdate.Trip.TripId);

                            if (trip == null)
                            {
                                response.TripUpdateResults.Add(
                                    new UpdateResult
                                        {
                                            Error = UpdateError.DeletedOnServer, ClientRowId = tripUpdate.Trip.RowId 
                                        });
                                continue;
                            }

                            if (trip.RowId != tripUpdate.ClientRowId)
                            {
                                response.TripUpdateResults.Add(
                                    new UpdateResult
                                        {
                                            Error = UpdateError.ModifiedOnServer, ClientRowId = tripUpdate.Trip.RowId 
                                        });
                                continue;
                            }

                            var entry = db.Entry(trip);
                            entry.CurrentValues.SetValues(tripUpdate.Trip);
                        }

                        db.SaveChanges();

                        scope.Complete();
                    }

                    response.TripUpdateResults.Add(
                        new UpdateResult { Error = null, ClientRowId = tripUpdate.Trip.RowId });

                    updatedTripIds.Add(tripUpdate.Trip.TripId);
                }
            }
        }

        private static void PerformExpenseUpdates(
            SyncRequest request, SyncResponse response, List<int> updatedExpenseIds)
        {
            if (request.ExpenseUpdates == null)
            {
                return;
            }

            foreach (ExpenseUpdate expenseUpdate in request.ExpenseUpdates)
            {
                using (var db = new BrennContext())
                {
                    using (var scope = new TransactionScope())
                    {
                        var expense = db.Expenses.Find(expenseUpdate.Expense.ExpenseId);

                        if (expense == null)
                        {
                            response.ExpenseUpdateResults.Add(
                                new UpdateResult
                                    { Error = UpdateError.DeletedOnServer, ClientRowId = expenseUpdate.Expense.RowId });
                            continue;
                        }

                        if (expense.RowId != expenseUpdate.ClientRowId)
                        {
                            response.ExpenseUpdateResults.Add(
                                new UpdateResult
                                    { Error = UpdateError.ModifiedOnServer, ClientRowId = expenseUpdate.Expense.RowId });
                            continue;
                        }

                        var entry = db.Entry(expense);
                        entry.CurrentValues.SetValues(expenseUpdate.Expense);

                        db.SaveChanges();

                        scope.Complete();
                    }

                    response.ExpenseUpdateResults.Add(
                        new UpdateResult { Error = null, ClientRowId = expenseUpdate.Expense.RowId });

                    updatedExpenseIds.Add(expenseUpdate.Expense.ExpenseId);
                }
            }
        }
    }
}