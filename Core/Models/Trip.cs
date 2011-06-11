namespace Opuno.Brenn.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public partial class Trip
    {
        [DataMember]
        public virtual Guid RowId { get; set; }

        [DataMember]
        public int ChangeSetN { get; set; }

        [DataMember]
        public virtual string DisplayName { get; set; }

        [DataMember]
        public virtual ICollection<Expense> Expenses { get; set; }

        [DataMember]
        public int TripId { get; set; }

        public IEnumerable<KeyValuePair<Person, decimal>> ValuePerPerson
        {
            get
            {
                if (this.Expenses == null)
                {
                    return null;
                }

                var allPeople = new List<KeyValuePair<Person, decimal>>();

                this.Expenses.ToList().ForEach(e => allPeople.AddRange(e.ValuePerPerson));

                var grouped =
                    allPeople.GroupBy(p => p.Key).Select(
                        pg => new KeyValuePair<Person, decimal>(pg.Key, pg.Sum(x => x.Value))).ToList();

                return grouped;
            }
        }

        ////public override bool Equals(object obj)
        ////{
        ////    if (ReferenceEquals(null, obj))
        ////    {
        ////        return false;
        ////    }
        ////    if (ReferenceEquals(this, obj))
        ////    {
        ////        return true;
        ////    }
        ////    if (obj.GetType() != typeof(Trip))
        ////    {
        ////        return false;
        ////    }
        ////    return this.Equals((Trip)obj);
        ////}

        ////public bool Equals(Trip other)
        ////{
        ////    if (ReferenceEquals(null, other))
        ////    {
        ////        return false;
        ////    }

        ////    if (ReferenceEquals(this, other))
        ////    {
        ////        return true;
        ////    }

        ////    return other.TripId == this.TripId;
        ////}

        /////*public override int GetHashCode()
        ////{
        ////    return this.TripId == 0 ? base.GetHashCode() : this.TripId;
        ////}*/

        ////public override string ToString()
        ////{
        ////    return this.DisplayName;
        ////}
    }
}