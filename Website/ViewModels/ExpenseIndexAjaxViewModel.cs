using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Opuno.Brenn.Website.ViewModels
{
    using Opuno.Brenn.Models;

    public class ExpenseIndexAjaxViewModel
    {
        public Trip Collection { get; set; }

        public List<Expense> Expenses { get; set; }
    }
}