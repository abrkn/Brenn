// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateResult.cs" company="">
//   
// </copyright>
// <summary>
//   The update result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.Sync
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The update result.
    /// </summary>
    [DataContract]
    public class UpdateResult
    {
        /// <summary>
        /// Gets or sets ClientRowId.
        /// </summary>
        [DataMember]
        public Guid ClientRowId { get; set; }

        /// <summary>
        /// Gets or sets Error.
        /// </summary>
        [DataMember]
        public UpdateError? Error { get; set; }
    }
}