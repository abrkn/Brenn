// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Person.cs" company="">
//   
// </copyright>
// <summary>
//   The person.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The person.
    /// </summary>
    public partial class Person : IClientModel
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
                return this.PersonId;
            }
        }

        public void Update(IClientModel model)
        {
            var typedModel = (Person)model;

            this.DisplayName = typedModel.DisplayName;
        }
    }
}
