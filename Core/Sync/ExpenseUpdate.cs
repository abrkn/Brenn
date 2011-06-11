// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpenseUpdate.cs" company="">
//   
// </copyright>
// <summary>
//   The expense update.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.Sync
{
    using System;
    using System.Runtime.Serialization;

    using Opuno.Brenn.Models;

    /// <summary>
    /// The expense update.
    /// </summary>
    [DataContract]
    public class ExpenseUpdate
    {
        /// <summary>
        /// Gets or sets the client row id.
        /// </summary>
        /// <value>
        /// The client row id.
        /// </value>
        [DataMember]
        public Guid ClientRowId { get; set; }


        /// <summary>
        /// Gets or sets the expense.
        /// </summary>
        /// <value>
        /// The expense.
        /// </value>
        [DataMember]
        public Expense Expense { get; set; }
    }
}