// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpenseViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   View model for an expense.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Opuno.Brenn.Models;

    /// <summary>
    /// View model for an expense.
    /// </summary>
    public sealed class ExpenseViewModel : ViewModel<Expense>
    {
        /// <summary>
        /// The amount.
        /// </summary>
        private decimal amount;

        /// <summary>
        /// The display name.
        /// </summary>
        private string displayName;

        /// <summary>
        /// The receivers.
        /// </summary>
        private ObservableCollection<Person> receivers = new ObservableCollection<Person>();

        /// <summary>
        /// The record date.
        /// </summary>
        private DateTime recordDate;

        /// <summary>
        /// The sender.
        /// </summary>
        private Person sender;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ExpenseViewModel" /> class.
        /// </summary>
        public ExpenseViewModel()
        {
            // Create dependencies between various propertyies.
            // TODO: Can this be done using DependencyProperty?
            this.Receivers.CollectionChanged += (s, e) => this.RaisePropertyChanged("ValuePerPerson");

            this.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "Amount" || e.PropertyName == "Sender")
                    {
                        this.RaisePropertyChanged("ValuePerPerson");
                    }
                };
        }

        /// <summary>
        ///   Gets or sets the amount.
        /// </summary>
        /// <value>
        ///   The amount.
        /// </value>
        public decimal Amount
        {
            get
            {
                return this.amount;
            }

            set
            {
                this.amount = value;
                this.RaisePropertyChanged("Amount");
            }
        }

        /// <summary>
        ///   Gets or sets the display name.
        /// </summary>
        /// <value>
        ///   The display name.
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
        ///   Gets or sets the known people.
        /// </summary>
        /// <value>
        ///   The known people.
        /// </value>
        public List<Person> KnownPeople { get; set; }

        /// <summary>
        ///   Gets or sets who the list of who used the expense.
        /// </summary>
        /// <value>
        ///   The used by.
        /// </value>
        public ObservableCollection<Person> Receivers
        {
            get
            {
                return this.receivers;
            }

            set
            {
                this.receivers = value;
                this.RaisePropertyChanged("receivers");
            }
        }

        /// <summary>
        ///   Gets or sets the record date.
        /// </summary>
        /// <value>
        ///   The record date.
        /// </value>
        public DateTime RecordDate
        {
            get
            {
                return this.recordDate;
            }

            set
            {
                this.recordDate = value;
                this.RaisePropertyChanged("RecordDate");
            }
        }

        /// <summary>
        ///   Gets or sets who the expense was paid by.
        /// </summary>
        /// <value>
        ///   The person who the expense was paid by.
        /// </value>
        public Person Sender
        {
            get
            {
                return this.sender;
            }

            set
            {
                this.sender = value;
                this.RaisePropertyChanged("sender");
            }
        }

        /// <summary>
        ///   Gets the value per person.
        /// </summary>
        public IEnumerable<KeyValuePair<Person, decimal>> ValuePerPerson
        {
            get
            {
                if (this.Receivers == null)
                {
                    return null;
                }

                if (this.Receivers.Count == 0)
                {
                    return null;
                }

                var valueFor = new Dictionary<Person, decimal>();
                var costPerUser = this.Amount / this.Receivers.Count;

                valueFor[this.Sender] = this.Amount;

                foreach (var person in this.Receivers)
                {
                    if (valueFor.ContainsKey(person))
                    {
                        valueFor[person] -= costPerUser;
                    }
                    else
                    {
                        valueFor[person] = -costPerUser;
                    }
                }

                return valueFor.ToList();
            }
        }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>The first validation error of the column or null.</returns>
        public override string this[string columnName]
        {
            get
            {
                if (columnName == "Amount")
                {
                    var error = Expense.ValidateAmount(this.Amount);

                    if (error != null)
                    {
                        return error;
                    }

                    // TODO: Max decimal places
                }
                else if (columnName == "RecordDate")
                {
                    var error = Expense.ValidateRecordDate(this.RecordDate);

                    if (error != null)
                    {
                        return error;
                    }
                }
                else if (columnName == "DisplayName")
                {
                    var error = Expense.ValidateDisplayName(this.DisplayName);

                    if (error != null)
                    {
                        return error;
                    }
                }
                else if (columnName == "Receivers")
                {
                    var error = Expense.ValidateReceivers(this.Receivers);

                    if (error != null)
                    {
                        return error;
                    }
                }
                else if (columnName == "Sender")
                {
                    var error = Expense.ValidateSender(this.Sender);

                    if (error != null)
                    {
                        return error;
                    }
                }

                // TODO: More validation, but no exceptions. Not everything has to be validated.
                return base[columnName];
            }
        }

        /// <summary>
        /// Loads from the model.
        /// </summary>
        public override void LoadFromModel()
        {
            this.DisplayName = this.Model.DisplayName;
            this.Amount = this.Model.Amount;
            this.RecordDate = this.Model.RecordDate;
            this.Sender = this.Model.Sender;

            this.Receivers.Clear();
            this.Model.Receivers.ToList().ForEach(x => this.Receivers.Add(x));
        }

        /// <summary>
        /// Saves the view model to the model.
        /// </summary>
        public override bool SaveToModel()
        {
            var error = this.GetAnyError("Amount", "RecordDate", "DisplayName", "Sender"/*, "Receivers"*/);

            if (error != null)
            {
                throw new InvalidOperationException(
                    string.Format("The view model cannot be saved without passing validation: {0}", error));
            }

            var modelChanged = false;

            // Creating a new model.
            if (this.Model == null)
            {
                this.Model = new Expense { Receivers = new List<Person>() };
                modelChanged = true;
            }

            var model = this.Model;

            if (model.DisplayName != this.DisplayName)
            {
                model.DisplayName = this.DisplayName;
                modelChanged = true;
            }

            if (model.Amount != this.Amount)
            {
                model.Amount = this.Amount;
                modelChanged = true;
            }

            if (model.Sender != this.Sender)
            {
                model.Sender = this.Sender;
                modelChanged = true;
            }

            if (model.RecordDate != this.RecordDate)
            {
                model.RecordDate = this.RecordDate;
                modelChanged = true;
            }

            if (model.Receivers.Count != this.Receivers.Count
                ||
                (model.Receivers.Any(x => !this.Receivers.Contains(x))
                 || this.Receivers.Any(x => !model.Receivers.Contains(x))))
            {
                model.Receivers = this.Receivers.ToList();
                modelChanged = true;
            }

            return modelChanged;
        }
    }
}