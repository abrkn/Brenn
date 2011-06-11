// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientModel.cs" company="">
//   
// </copyright>
// <summary>
//   The i client model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.Models
{
    using System;

    /// <summary>
    /// The i client model.
    /// </summary>
    public interface IClientModel
    {
        /// <summary>
        ///   Gets or sets the client identifier.
        /// </summary>
        /// <value>
        ///   The client identifier.
        /// </value>
        int Key { get; set; }

        /// <summary>
        /// Gets or sets the row identifier.
        /// </summary>
        /// <value>
        /// The row identifier.
        /// </value>
        Guid RowId { get; set; }

        int StoreId { get; }

        int ChangeSetN { get; set; }

        void Update(IClientModel model);
    }
}