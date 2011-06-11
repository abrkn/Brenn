// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Trip.cs" company="">
//   
// </copyright>
// <summary>
//   The trip.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.Models
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// The trip.
    /// </summary>
    public partial class Trip : IClientModel
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
                return this.TripId;
            }
        }

        public void Update(IClientModel model)
        {
            var typedModel = (Trip)model;

            this.DisplayName = typedModel.DisplayName;

            // Copy the expenses.
            this.Expenses = typedModel.Expenses.ToList();
        }
    }
}