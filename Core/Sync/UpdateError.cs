// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateError.cs" company="">
//   
// </copyright>
// <summary>
//   The update error.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.Sync
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The update error.
    /// </summary>
    [DataContract]
    public enum UpdateError
    {
        /// <summary>
        /// The modified on server.
        /// </summary>
        [EnumMember]
        ModifiedOnServer, 

        /// <summary>
        /// The deleted on server.
        /// </summary>
        [EnumMember]
        DeletedOnServer
    }
}