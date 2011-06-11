using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Opuno.Brenn.WindowsPhone.UserControls
{
    using Opuno.Brenn.Models;

    using ViewModels;

    public partial class TripViewExpenses : UserControl
    {
        public EventHandler<ExpenseEventArgs> ExpenseClicked { get; set; }

        public TripViewExpenses()
        {
            InitializeComponent();
        }

        private void navigationControl_Navigation(object sender, NavigationListControl.NavigationEventArgs e)
        {
            if (this.ExpenseClicked != null)
            {
                this.ExpenseClicked(this, new ExpenseEventArgs { Expense = (Expense)e.Item });
            }
        }
    }

    public class ExpenseEventArgs : EventArgs
    {
        public Expense Expense { get; set; }
    }
}
