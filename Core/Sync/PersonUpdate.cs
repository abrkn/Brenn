// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonUpdate.cs" company="">
//   
// </copyright>
// <summary>
//   The person update.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.Sync
{
    using System;
    using System.Runtime.Serialization;

    using Opuno.Brenn.Models;

    /// <summary>
    /// The person update.
    /// </summary>
    [DataContract]
    public class PersonUpdate
    {
        /// <summary>
        /// Gets or sets ClientRowId.
        /// </summary>
        [DataMember]
        public Guid ClientRowId { get; set; }

        /// <summary>
        /// Gets or sets Person.
        /// </summary>
        [DataMember]
        public Person Person { get; set; }
    }
}