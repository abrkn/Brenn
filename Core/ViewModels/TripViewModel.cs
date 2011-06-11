// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TripViewModel.cs" company="Opuno">
//   Opuno
// </copyright>
// <summary>
//   The view model for a trip.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    using Opuno.Brenn.Models;

    /// <summary>
    /// The view model for a trip.
    /// </summary>
    public sealed class TripViewModel : ViewModel<Trip>
    {
        private readonly ObservableCollection<Expense> expenses;

        private string displayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TripViewModel"/> class.
        /// </summary>
        public TripViewModel()
        {
            this.expenses = new ObservableCollection<Expense>();
            this.expenses.CollectionChanged += (s, e) => this.RaisePropertyChanged("ValuePerPerson");

            this.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "Amount")
                    {
                        this.RaisePropertyChanged("ValuePerPerson");
                    }
                };
        }

        /// <summary>
        /// Gets or sets the trip's display name.
        /// </summary>
        /// <value>
        /// The trip's display name.
        /// </value>
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                this.displayName = value;
                this.RaisePropertyChanged("DisplayName");
            }
        }

        /// <summary>
        /// Gets the expenses.
        /// </summary>
        public ObservableCollection<Expense> Expenses
        {
            get
            {
                return this.expenses;
            }
        }

        /// <summary>
        /// Gets the value per person.
        /// </summary>
        public IEnumerable<KeyValuePair<Person, decimal>> ValuePerPerson
        {
            get
            {
                if (this.Expenses == null)
                {
                    return null;
                }

                var allPeople = new List<KeyValuePair<Person, decimal>>();

                this.Expenses.Where(x => x.Receivers.Count > 0).ToList().ForEach(e => allPeople.AddRange(e.ValuePerPerson));

                var grouped =
                    allPeople.GroupBy(p => p.Key).Select(
                        pg => new KeyValuePair<Person, decimal>(pg.Key, pg.Sum(x => x.Value))).ToList();

                return grouped;
            }
        }

        /// <summary>
        /// Loads from the model.
        /// </summary>
        public override void LoadFromModel()
        {
            Debug.Assert(this.Model != null);
            Debug.Assert(this.Model.Expenses != null);

            var model = this.Model;

            this.DisplayName = model.DisplayName;

            this.Expenses.Clear();

            foreach (var expense in this.Model.Expenses)
            {
                this.Expenses.Add(expense);
            }
        }

        /// <summary>
        /// Saves the view model to the model.
        /// </summary>
        public override bool SaveToModel()
        {
            var error = this.GetAnyError("DisplayName");

            if (error != null)
            {
                throw new InvalidOperationException(string.Format("The view model cannot be saved without passing validation: {0}", error));
            }

            var modelChanged = false;

            if (this.Model == null)
            {
                this.Model = new Trip();
                modelChanged = true;
            }

            var model = this.Model;

            if (model.DisplayName != this.DisplayName)
            {
                model.DisplayName = this.DisplayName;
                modelChanged = true;
            }

            return modelChanged;
        }
    }
}