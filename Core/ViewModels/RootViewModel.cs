namespace Opuno.Brenn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    using Opuno.Brenn.Models;

    public class RootViewModel : ViewModel
    {
        public static RootViewModel Instance = new RootViewModel();

        private ObservableCollection<Trip> trips = new ObservableCollection<Trip>();

        public ObservableCollection<Trip> Trips
        {
            get
            {
                return this.trips;
            }

            set
            {
                this.trips = value;
                this.RaisePropertyChanged("Trips");
            }
        }

        public void LoadTrips(IEnumerable<Trip> tripsToLoad)
        {
            if (tripsToLoad == null)
            {
                throw new ArgumentNullException("tripsToLoad");
            }

            this.Trips.Clear();

            foreach (var trip in tripsToLoad)
            {
                this.Trips.Add(trip);
            }
        }

        public override void LoadFromModel()
        {
            throw new NotImplementedException();
        }

        public override bool SaveToModel()
        {
            throw new NotImplementedException();
        }
    }
}
