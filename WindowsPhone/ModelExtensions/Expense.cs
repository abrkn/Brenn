// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Expense.cs" company="">
//   
// </copyright>
// <summary>
//   The expense.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.Models
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// The expense.
    /// </summary>
    public partial class Expense : IClientModel
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        [DataMember]
        public int Key { get; set; }

        public int StoreId
        {
            get
            {
                return this.ExpenseId;
            }
        }

        public void Update(IClientModel model)
        {
            var typedModel = (Expense)model;

            this.Amount = typedModel.Amount;
            this.DisplayName = typedModel.DisplayName;
            this.RecordDate = typedModel.RecordDate;
            this.Receivers = typedModel.Receivers.ToList();
            this.Sender = typedModel.Sender;
        }
    }
}