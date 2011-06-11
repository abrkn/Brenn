using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Opuno.Brenn.Website.ViewModels
{
    using Opuno.Brenn.Models;

    public class ExpenseViewModel
    {
        public Expense Expense { get; set; }

        public List<Person> PossiblePayers { get; set; }

        public List<Tuple<Person, bool>> PossibleUsers { get; set; }
    }
}