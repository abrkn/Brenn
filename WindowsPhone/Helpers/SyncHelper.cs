// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The sync helper item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.WindowsPhone.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Opuno.Brenn.Models;
    using Opuno.Brenn.Sync;
    using Opuno.Brenn.WindowsPhone.DataAccess;

    /// <summary>
    /// The sync helper item.
    /// </summary>
    public class SyncHelperItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether Delete.
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        /// Gets or sets Entity.
        /// </summary>
        public IClientModel Entity { get; set; }
    }

    /// <summary>
    /// The sync helper.
    /// </summary>
    public class SyncHelper
    {
        /// <summary>
        ///   The singleton instance of the class.
        /// </summary>
        public static readonly SyncHelper Instance = new SyncHelper();

        /// <summary>
        ///   The items.
        /// </summary>
        private readonly List<SyncHelperItem> items = new List<SyncHelperItem>();

        /// <summary>
        ///   The change set of this client.
        /// </summary>
        private int clientChangeSet;

        /// <summary>
        ///   Prevents a default instance of the <see cref = "SyncHelper" /> class from being created.
        /// </summary>
        private SyncHelper()
        {
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public bool Add(SyncHelperItem item)
        {
            throw new NotImplementedException();

            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            lock (this.items)
            {
                this.AddWithoutLock(item);
            }
        }

        /// <summary>
        /// The sync.
        /// </summary>
        public void Sync()
        {
            lock (this.items)
            {
                this.SyncWithoutLock();
            }
        }

        /// <summary>
        /// The add without lock.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        private void AddWithoutLock(SyncHelperItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var existingItem = this.items.SingleOrDefault(x => x.Entity == item.Entity);

            if (existingItem != null)
            {
                const int DeleteAction = 2;
                const int CreateAction = 0;
                const int EditAction = 1;
                var oldAction = existingItem.Delete
                                    ? DeleteAction
                                    : existingItem.Entity.StoreId == default(int) ? CreateAction : EditAction;
                var newAction = item.Delete
                                    ? DeleteAction
                                    : item.Entity.StoreId == default(int) ? CreateAction : EditAction;

                // Delete, Edit
                if (oldAction == DeleteAction && newAction == EditAction)
                {
                    throw new InvalidOperationException("Cannot edit an entity that is pending deletion.");
                }

                // Delete, Create
                if (oldAction == DeleteAction && newAction == CreateAction)
                {
                    throw new InvalidOperationException("Cannot create an entity that is pending creation.");
                }

                // Delete, Delete
                if (oldAction == DeleteAction && newAction == DeleteAction)
                {
                    throw new InvalidOperationException("Cannot delete an entity that is pending deletion.");
                }

                // Edit, Create
                if (oldAction == EditAction && newAction == CreateAction)
                {
                    throw new InvalidOperationException("Cannot create an entity that is pending creation.");
                }

                // Edit, Edit
                // No action is required as the entity is already queued.
                if (oldAction == EditAction && newAction == EditAction)
                {
                    return;
                }

                // Edit, Delete
                if (oldAction == EditAction && newAction == DeleteAction)
                {
                    Debug.WriteLine("An entity that is marked for edit will now be marked for deletion instead.");
                    existingItem.Delete = true;
                }

                // Create, Create
                if (oldAction == CreateAction && newAction == CreateAction)
                {
                    // The case is where the user edits an item that is newly created. It just means that the entity
                    // has not yet received a store id.
                    Debug.WriteLine("An edit was performed on an entity that is marked for creation.");
                    return;
                }

                // Create, Edit
                if (oldAction == CreateAction && newAction == EditAction)
                {
                    throw new InvalidOperationException(
                        "An entity marked for creation was edited. This should not happen.");
                }

                // Create, Delete
                if (oldAction == CreateAction && newAction == DeleteAction)
                {
                    Debug.WriteLine("An entity that is queued for creation was deleted.");
                    this.items.Remove(existingItem);
                    return;
                }
            }
            else
            {
                this.items.Add(item);
            }
        }

        /// <summary>
        /// The handle sync response.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        private void HandleSyncResponse(SyncResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            lock (this.items)
            {
                this.HandleSyncResponseWithoutLock(response);
            }
        }

        /// <summary>
        /// The handle sync response without lock.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        private void HandleSyncResponseWithoutLock(SyncResponse response)
        {
            Debug.WriteLine("Handling sync response with server change set {0}.", response.ServerChangeSetN);

            if (response.ServerChangeSetN <= this.clientChangeSet)
            {
                Debug.WriteLine(
                    "Warning: The changeset number of the sync response is {0}, which is lower than or equal to the client version, {1}.", 
                    response.ServerChangeSetN, 
                    this.clientChangeSet);
            }
            else
            {
                this.clientChangeSet = response.ServerChangeSetN;
            }

            foreach (var update in response.ExpenseUpdateResults)
            {
                if (update.Error.HasValue)
                {
                    Debug.WriteLine("Failed to update expense: {0}", update.Error);
                }
                else
                {
                    Debug.WriteLine("Successful update of expense.");
                }
            }

            foreach (var update in response.TripUpdateResults)
            {
                if (update.Error.HasValue)
                {
                    Debug.WriteLine("Failed to update trip: {0}", update.Error);
                }
                else
                {
                    Debug.WriteLine("Successful update of trip.");
                }
            }

            foreach (var update in response.PersonUpdateResults)
            {
                if (update.Error.HasValue)
                {
                    Debug.WriteLine("Failed to update person: {0}", update.Error);
                }
                else
                {
                    Debug.WriteLine("Successful update of person.");
                }
            }

            if (response.People != null)
            {
                response.People.ForEach(p => Repository.Instance.AddOrUpdate(p));
            }

            if (response.Trips != null)
            {
                foreach (var t in response.Trips)
                {
                    Debug.Assert(t.Expenses == null, "The trip included its expenses.");

                    Repository.Instance.AddOrUpdate(t);
                }
            }

            if (response.Expenses != null)
            {
                foreach (var expense in response.Expenses)
                {
                    Debug.Assert(expense.Receivers != null, "Receivers was not included from the service.");
                    Debug.Assert(expense.Trip == null, "The trip should not be defined by the expense.");
                    Debug.Assert(expense.TripId > 0, "The trip id must be included.");

                    Repository.Instance.AddOrUpdate(expense);
                }
            }
        }

        /// <summary>
        /// The sync without lock.
        /// </summary>
        private void SyncWithoutLock()
        {
            var itemsToSync = this.items.ToList();
            this.items.Clear();

            var request = new SyncRequest
                {
                    ClientChangeSetN = this.clientChangeSet, 
                    ExpenseUpdates = new List<ExpenseUpdate>(), 
                    TripUpdates = new List<TripUpdate>(), 
                    PersonUpdates = new List<PersonUpdate>()
                };

            Debug.WriteLine("Syncing. Client change set is {0}.", request.ClientChangeSetN);

            foreach (var expense in itemsToSync.OfType<Expense>())
            {
                var update = new ExpenseUpdate { ClientRowId = expense.RowId, Expense = expense };
                expense.RowId = Guid.NewGuid();
                request.ExpenseUpdates.Add(update);
            }

            foreach (var trip in itemsToSync.OfType<Trip>())
            {
                var update = new TripUpdate { ClientRowId = trip.RowId, Trip = trip };
                trip.RowId = Guid.NewGuid();
                request.TripUpdates.Add(update);
            }

            foreach (var person in itemsToSync.OfType<Person>())
            {
                var update = new PersonUpdate { ClientRowId = person.RowId, Person = person };
                person.RowId = Guid.NewGuid();
                request.PersonUpdates.Add(update);
            }

            ServiceHelper.Instance.Queue<SyncRequest, SyncResponse>("Sync", this.HandleSyncResponse, request);
        }
    }
}