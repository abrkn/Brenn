namespace Opuno.Brenn.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public partial class Person
    {

        [DataMember]
        public virtual Guid RowId { get; set; }

        [DataMember]
        public int ChangeSetN { get; set; }

        [DataMember]
        public virtual int PersonId { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        public virtual ICollection<Expense> ExpensesUsed { get; set; }

        public virtual ICollection<Expense> ExpensesPaid { get; set; }

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
        ////    if (obj.GetType() != typeof(Person))
        ////    {
        ////        return false;
        ////    }
        ////    return Equals((Person)obj);
        ////}

        ////public bool Equals(Person other)
        ////{
        ////    if (ReferenceEquals(null, other))
        ////    {
        ////        return false;
        ////    }
        ////    if (ReferenceEquals(this, other))
        ////    {
        ////        return true;
        ////    }
        ////    return other.PersonId == this.PersonId;
        ////}

        /////*public override int GetHashCode()
        ////{
        ////    return this.PersonId == 0 ? base.GetHashCode() : this.PersonId;
        ////}*/
        /// 

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}