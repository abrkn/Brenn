// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TripUpdate.cs" company="">
//   
// </copyright>
// <summary>
//   The trip update.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.Sync
{
    using System;
    using System.Runtime.Serialization;

    using Opuno.Brenn.Models;

    /// <summary>
    /// The trip update.
    /// </summary>
    [DataContract]
    public class TripUpdate
    {
        /// <summary>
        /// Gets or sets ClientRowId.
        /// </summary>
        [DataMember]
        public Guid ClientRowId { get; set; }

        /// <summary>
        /// Gets or sets Trip.
        /// </summary>
        [DataMember]
        public Trip Trip { get; set; }
    }
}