namespace Opuno.Brenn.ViewModels
{
    using System;

    using Opuno.Brenn.Models;

    public sealed class PersonViewModel : ViewModel<Person>
    {
        private string displayName;

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                this.displayName = value;
                this.RaisePropertyChanged("DisplayName");
            }
        }

        /// <summary>
        /// Loads from the model.
        /// </summary>
        public override void LoadFromModel()
        {
            this.DisplayName = this.Model.DisplayName;
        }

        /// <summary>
        /// Saves the view model to the model.
        /// </summary>
        public override bool SaveToModel()
        {
            throw new NotImplementedException();
        }
    }
}