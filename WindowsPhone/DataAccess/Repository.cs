// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="Repository.cs" company="Opuno">
// //   Copyright (c) Opuno. All rights reserved.
// // </copyright>
// // <summary>
// //   Defines the Repository type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace Opuno.Brenn.WindowsPhone.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using Models;

    public class Repository
    {
        public static readonly Repository Instance = new Repository();

        private readonly Dictionary<int, IClientModel> models = new Dictionary<int, IClientModel>();

        private int lastKey;

        /// <summary>
        /// Gets the model with the specified key.
        /// </summary>
        public IClientModel this[int key]
        {
            get
            {
                if (key <= 0)
                {
                    throw new ArgumentOutOfRangeException("key", "key must not be less than or equal to zero.");
                }

                return this.models[key];
            }
        }

        protected Repository()
        {
        }

        public EventHandler<RepositoryItemEventArgs> Added { get; set; }

        public EventHandler<RepositoryItemEventArgs> Removed { get; set; }

        public EventHandler<RepositoryItemEventArgs> Updated { get; set; }

        public IEnumerable<KeyValuePair<int, IClientModel>> All
        {
            get
            {
                // TODO: Copy?
                return this.models;
            }
        }

        /// <summary>
        /// Adds the specified model to the repository.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The client key for the model</returns>
        public virtual int Add(IClientModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            Debug.Assert(
                !this.models.Any(mkv => mkv.Value.GetType() == model.GetType() && mkv.Value.StoreId == model.StoreId),
                "A duplicate model already exists in the repository.");

            var key = Interlocked.Increment(ref this.lastKey);
            model.Key = key;

            Debug.WriteLine("Model [LocalId={0}; Type={1}; StoreId={2}] add to repository.", key, model.GetType().Name, model.StoreId);

            this.models.Add(key, model);

            // Resolve references
            var trip = model as Trip;

            if (trip != null)
            {
                foreach (var e in this.models.Values.OfType<Expense>().Where(e => e.TripId == trip.TripId))
                {
                    Debug.Assert(e.Trip == null);
                    e.Trip = trip;
                }
            }

            var expense = model as Expense;

            if (expense != null)
            {
                // Add to its trip, if the trip is in.
                var t = this.models.Values.OfType<Trip>().SingleOrDefault(x => x.TripId == expense.TripId);

                if (t != null)
                {
                    Debug.Assert(!t.Expenses.Any(x => x.ExpenseId == expense.ExpenseId));
                    t.Expenses.Add(expense);
                    // TODO: Inform of this update. Using this.Updated?
                }
            }

            if (this.Added != null)
            {
                this.Added(this, new RepositoryItemEventArgs(key, model));
            }

            return key;
        }

        public virtual void Remove(int key)
        {
            if (key <= 0)
            {
                throw new ArgumentException("The key parameter must be greater than zero.", "key");
            }

            var model = this.models[key];

            var removed = this.models.Remove(key);
            Debug.Assert(removed, "Unable to find model to be removed in the repository.");

            Debug.WriteLine("Model [LocalId={0}; Type={1}; StoreId={2}] removed from repository.", key, model.GetType().Name, model.StoreId);

            if (this.Removed != null)
            {
                this.Removed(this, new RepositoryItemEventArgs(key, model));
            }
        }

        public void AddOrUpdate(IClientModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            model.Key =
                this.models.SingleOrDefault(
                    x => x.Value.GetType() == model.GetType() && x.Value.StoreId == model.StoreId).Key;

            {
                // TODO: Move this out
                var trip = model as Trip;

                if (trip != null)
                {
                    Debug.Assert(trip.Expenses == null);
                    Debug.Assert(trip.TripId != default(int));

                    trip.Expenses =
                        new List<Expense>(
                            this.models.Select(x => x.Value).OfType<Expense>().Where(x => x.TripId == trip.TripId));
                }

                var expense = model as Expense;

                if (expense != null)
                {
                    Debug.Assert(expense.Trip == null);
                    Debug.Assert(expense.TripId != default(int));

                    expense.Trip =
                        this.models.Select(x => x.Value).OfType<Trip>().SingleOrDefault(x => x.TripId == expense.TripId);
                }

                // TODO: Person? Prob. not needed.
            }

            if (model.Key == default(int))
            {
                this.Add(model);
                return;
            }

            // Ensure that the model needs to be replaced.
            if (this[model.Key].RowId == model.RowId)
            {
                Debug.WriteLine(
                    "*** Not replacing model {0} with new model {1} because the row identifier is unchanged. Possibly wasted bandwidth?",
                    this[model.Key],
                    model);

                return;
            }

            var existingModel = this.models[model.Key];

            if (existingModel.RowId == model.RowId)
            {
                throw new Exception("The row identifier should have changed.");
            }

            if (existingModel.ChangeSetN >= model.ChangeSetN)
            {
                throw new Exception("The change set number should have increased.");
            }

            existingModel.RowId = model.RowId;
            existingModel.ChangeSetN = model.ChangeSetN;

            this.models[model.Key].Update(model);

            if (this.Updated != null)
            {
                this.Updated(this, new RepositoryItemEventArgs(model.Key, model));
            }

            Debug.WriteLine("Model [LocalId={0}; Type={1}; StoreId={2}] replaced in repository.", model.Key, model.GetType().Name, model.StoreId);
        }

        public bool Contains(int key)
        {
            return this.models.ContainsKey(key);
        }
    }
}
