namespace Opuno.Brenn.WindowsPhone.Views
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Navigation;

    using Microsoft.Phone.Controls;

    using NavigationListControl;

    using Opuno.Brenn.Models;
    using Opuno.Brenn.ViewModels;
    using Opuno.Brenn.WindowsPhone.DataAccess;
    using Opuno.Brenn.WindowsPhone.Helpers;

    public partial class TripView
    {
        protected TripViewModel ViewModel
        {
            get
            {
                return this.DataContext as TripViewModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TripView"/> class.
        /// </summary>
        public TripView()
        {
            InitializeComponent();

            this.ExpensesUc.ExpenseClicked += (a, b) => NavigationService.Navigate(
                new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        "/Views/ExpenseView.xaml?id={0}", 
                        b.Expense.Key), 
                    UriKind.Relative));
        }

        /// <summary>
        /// Called when a page becomes the active page in a frame.
        /// </summary>
        /// <param name="e">An object that contains the event data.</param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Debug.Assert(this.NavigationContext.QueryString.ContainsKey("id"), "The 'id' parameter is missing from the query string.");

            var id = int.Parse(this.NavigationContext.QueryString["id"]);
            Debug.Assert(id > 0, "The id parameter must be greater than zero.");

            ////ViewModelStore.Instance.GetOrCreate<Trip, TripViewModel>(id);
            this.ViewModel = new TripViewModel { Model = (Trip)Repository.Instance[id] };
            this.ViewModel.LoadFromModel();

            // Won't work anywhere else because it'd overwrite the user's changes.
            this.ViewModel.PropertyChanged += (a, b) =>
                {
                    if (this.ViewModel.Error == null)
                    {
                        Debug.WriteLine("Saving view model and adding it for sync.");
                        this.ViewModel.SaveToModel();

                        if (SyncHelper.Instance.Add(new SyncHelperItem { Entity = this.ViewModel.Model }))
                        {
                            Debug.WriteLine("Added model for sync.");
                        }
                        else
                        {
                            Debug.WriteLine("Model was already in queue for sync.");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Not saving view model because it has validation errors.");
                    }
                };

            if (this.NavigationContext.QueryString.ContainsKey("PanoramaItem"))
            {
                if (this.NavigationContext.QueryString["PanoramaItem"] == "Settings")
                {
                    this.Panorama.DefaultItem = this.SettingsPanoramaItem;
                }
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // TODO: Can dispose the viewmodel here to save memory and lose some perf.

            base.OnNavigatedFrom(e);
        }

        private void NavigationListNavigation(object sender, NavigationListControl.NavigationEventArgs e)
        {
            var expenseViewModel = e.Item as ExpenseViewModel;

            // navigate to the feed items page
            NavigationService.Navigate(new Uri("/Views/ExpenseView.xaml?id=" + expenseViewModel.Model.Key, UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            var expense = new Expense
                {
                    RowId = Guid.NewGuid(),
                    DisplayName = "New expense",
                    RecordDate = DateTime.Now,
                    Sender = null,
                    Receivers = new List<Person>(),
                    Trip = (Trip)Repository.Instance[this.ViewModel.Model.Key],
                    Amount = 0
                };

            var key = Repository.Instance.Add(expense);

            NavigationService.Navigate(new Uri("/Views/ExpenseView.xaml?id=" + key, UriKind.Relative));
        }
    }
}