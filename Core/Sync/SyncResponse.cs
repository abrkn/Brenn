// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncResponse.cs" company="">
//   
// </copyright>
// <summary>
//   The sync response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.Sync
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Opuno.Brenn.Models;

    /// <summary>
    /// The sync response.
    /// </summary>
    [DataContract]
    public class SyncResponse
    {
        /// <summary>
        /// Gets or sets ExpenseUpdateResults.
        /// </summary>
        [DataMember]
        public List<UpdateResult> ExpenseUpdateResults { get; set; }

        /// <summary>
        /// Gets or sets Expenses.
        /// </summary>
        [DataMember]
        public List<Expense> Expenses { get; set; }

        /// <summary>
        /// Gets or sets People.
        /// </summary>
        [DataMember]
        public List<Person> People { get; set; }

        /// <summary>
        /// Gets or sets PersonUpdateResults.
        /// </summary>
        [DataMember]
        public List<UpdateResult> PersonUpdateResults { get; set; }

        /// <summary>
        /// Gets or sets ServerChangeSetN.
        /// </summary>
        [DataMember]
        public int ServerChangeSetN { get; set; }

        /// <summary>
        /// Gets or sets TripUpdateResults.
        /// </summary>
        [DataMember]
        public List<UpdateResult> TripUpdateResults { get; set; }

        /// <summary>
        /// Gets or sets Trips.
        /// </summary>
        [DataMember]
        public List<Trip> Trips { get; set; }
    }
}