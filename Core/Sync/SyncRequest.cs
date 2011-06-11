// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncRequest.cs" company="">
//   
// </copyright>
// <summary>
//   The sync request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.Sync
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The sync request.
    /// </summary>
    [DataContract]
    public class SyncRequest
    {
        /// <summary>
        /// Gets or sets ClientChangeSetN.
        /// </summary>
        [DataMember]
        public int ClientChangeSetN { get; set; }

        /// <summary>
        /// Gets or sets ExpenseUpdates.
        /// </summary>
        [DataMember]
        public List<ExpenseUpdate> ExpenseUpdates { get; set; }

        /// <summary>
        /// Gets or sets PersonUpdates.
        /// </summary>
        [DataMember]
        public List<PersonUpdate> PersonUpdates { get; set; }

        /// <summary>
        /// Gets or sets TripUpdates.
        /// </summary>
        [DataMember]
        public List<TripUpdate> TripUpdates { get; set; }
    }
}