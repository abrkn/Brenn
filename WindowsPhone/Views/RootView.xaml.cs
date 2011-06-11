// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootView.xaml.cs" company="Opuno">
//   Opuno
// </copyright>
// <summary>
//   The root view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.WindowsPhone.Views
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;

    using NavigationListControl;

    using Opuno.Brenn.Models;
    using Opuno.Brenn.ViewModels;
    using Opuno.Brenn.WindowsPhone.DataAccess;
    using Opuno.Brenn.WindowsPhone.Helpers;

    /// <summary>
    /// The root view.
    /// </summary>
    public partial class RootView
    {
        // TODO: Abstract this?

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RootView" /> class.
        /// </summary>
        public RootView()
        {
            this.InitializeComponent();

            if (this.DataContext != null)
            {
                return;
            }

            this.DataContext = new RootViewModel();

            SyncHelper.Instance.Sync();
        }

        /// <summary>
        ///   Gets the view model.
        /// </summary>
        protected RootViewModel ViewModel
        {
            get
            {
                return (RootViewModel)this.DataContext;
            }
        }

        /// <summary>
        /// Called when a page becomes the active page in a frame.
        /// </summary>
        /// <param name="e">
        /// An object that contains the event data.
        /// </param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.LoadTripsIntoViewModel();

            Repository.Instance.Added += (a, b) =>
            {
                if (!(b.Model is Trip))
                {
                    return;
                }

                Debug.WriteLine("Re-loading view because a trip was added.");
                Deployment.Current.Dispatcher.BeginInvoke(this.LoadTripsIntoViewModel);
            };
        }

        private void LoadTripsIntoViewModel()
        {
            var trips = Repository.Instance.All.Select(x => x.Value).OfType<Trip>().ToList();

            this.ViewModel.LoadTrips(trips);
        }

        /// <summary>
        /// The application bar new trip button click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ApplicationBarNewTripButtonClick(object sender, EventArgs e)
        {
            var newTripModel = new Trip
                {
                   RowId = Guid.NewGuid(), Expenses = new List<Expense>(), DisplayName = "New trip" 
                };
            var newTripModelId = Repository.Instance.Add(newTripModel);

            this.NavigationService.Navigate(
                new Uri("/Views/TripView.xaml?id=" + newTripModelId + "&PanoramaItem=Settings", UriKind.Relative));
        }

        /// <summary>
        /// The navigation list navigation.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NavigationListNavigation(object sender, NavigationEventArgs e)
        {
            var trip = e.Item as Trip;
            Debug.Assert(trip != null);
            Debug.Assert(trip.Key != 0);

            // navigate to the feed items page
            this.NavigationService.Navigate(new Uri(string.Format("/Views/TripView.xaml?id={0}", trip.Key), UriKind.Relative));
        }
    }
}