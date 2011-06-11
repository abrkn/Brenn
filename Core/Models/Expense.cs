// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Expense.cs" company="Opuno">
//   Copyright Opuno (c) 2011
// </copyright>
// <summary>
//   Defines the Expense type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Stores information about an expense.
    /// </summary>
    [DataContract]
    public partial class Expense
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        [DataMember]
        public virtual decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the unique row identifier.
        /// </summary>
        /// <value>
        /// The unique row identifier.
        /// </value>
        [DataMember]
        public virtual Guid RowId { get; set; }

        /// <summary>
        /// Gets or sets the change set number.
        /// </summary>
        /// <value>
        /// The change set number.
        /// </value>
        [DataMember]
        public int ChangeSetN { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        [DataMember]
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the expense identifier.
        /// </summary>
        /// <value>
        /// The expense identifier.
        /// </value>
        [DataMember]
        public virtual int ExpenseId { get; set; }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        [DataMember]
        public virtual Person Sender { get; set; }

        /// <summary>
        /// Gets or sets the receiver's person id.
        /// </summary>
        /// <value>
        /// The receiver's person id.
        /// </value>
        [DataMember]
        public virtual int ReceiverPersonId { get; set; }

        /// <summary>
        /// Gets or sets the record date.
        /// </summary>
        /// <value>
        /// The record date.
        /// </value>
        [DataMember]
        public virtual DateTime RecordDate { get; set; }

        /// <summary>
        /// Gets or sets the trip.
        /// </summary>
        /// <value>
        /// The trip.
        /// </value>
        public virtual Trip Trip { get; set; }

        /// <summary>
        /// Gets or sets the trip id.
        /// </summary>
        /// <value>
        /// The trip id.
        /// </value>
        [DataMember]
        public virtual int TripId { get; set; }

        /// <summary>
        /// Gets or sets the receivers.
        /// </summary>
        /// <value>
        /// The receivers.
        /// </value>
        [DataMember]
        public virtual ICollection<Person> Receivers { get; set; }

        public const string TooFewReceiversValidationError = "TooFewReceiversValidationError";

        public const string AmountLessThanOrEqualToZeroValidationError = "AmountLessThanOrEqualToZeroValidationError";

        public const string AmountTooLargeValidationError = "AmountTooLargeValidationError";

        public static string ValidateAmount(decimal amount)
        {
            if (amount < 0)
            {
                return AmountLessThanOrEqualToZeroValidationError;
            }

            if (amount > 100 * 1000 * 1000)
            {
                return AmountTooLargeValidationError;
            }

            return null;
        }

        public const string RecordDateTooEarlyValidationError = "RecordDateTooEarlyValidationError";

        public const string RecordDateTooLateValidationError = "RecordDateTooLateValidationError";

        public const string DisplayNameEmptyValidationError = "DisplayNameEmptyValidationError";
 
        public static string ValidateDisplayName(string displayName)
        {
            if (displayName == null)
            {
                throw new ArgumentNullException("displayName");
            }

            if (displayName.Length == 0)
            {
                return DisplayNameEmptyValidationError;
            }

            return null;
        }

        public const string SenderNotSetValidationError = "SenderNotSetValidationError";

        public static string ValidateSender(Person sender)
        {
            if (sender == null)
            {
                return SenderNotSetValidationError;
            }

            return null;
        }

        public static string ValidateRecordDate(DateTime recordDate)
        {
            if (recordDate.Year < 1950)
            {
                return RecordDateTooEarlyValidationError;
            }

            if (recordDate > DateTime.UtcNow.AddYears(10))
            {
                return RecordDateTooLateValidationError;
            }

            return null;
        }

        public static string ValidateReceivers(ICollection<Person> receivers)
        {
            if (receivers == null)
            {
                throw new ArgumentNullException("receivers");
            }

            if (receivers.Count == 0)
            {
                return TooFewReceiversValidationError;
            }

            return null;
        }

        /// <summary>
        /// Gets the value per person.
        /// </summary>
        public IEnumerable<KeyValuePair<Person, decimal>> ValuePerPerson
        {
            get
            {
                if (this.Receivers == null)
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
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}