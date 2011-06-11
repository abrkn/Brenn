namespace Opuno.Brenn.WindowsPhone.Views
{
    using System.Linq;
    using System.Windows.Navigation;

    using Microsoft.Phone.Controls;

    using Opuno.Brenn.Models;
    using Opuno.Brenn.ViewModels;
    using Opuno.Brenn.WindowsPhone.DataAccess;
    using Opuno.Brenn.WindowsPhone.Helpers;

    public partial class ExpenseView : PhoneApplicationPage
    {
        protected ExpenseViewModel ViewModel
        {
            get
            {
                return this.DataContext as ExpenseViewModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

        public ExpenseView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var key = int.Parse(NavigationContext.QueryString["id"]);
            this.ViewModel = new ExpenseViewModel
                {
                    Model = (Expense)Repository.Instance[key],
                    KnownPeople = Repository.Instance.All.Select(x => x.Value).OfType<Person>().ToList()
                };
            this.ViewModel.LoadFromModel();
        }

        private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.ViewModel.SaveToModel())
            {
                SyncHelper.Instance.Add(new SyncHelperItem() { Entity = this.ViewModel.Model });
            }

            NavigationService.GoBack();
        }
    }
}