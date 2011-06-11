// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Opuno">
//   Opuno
// </copyright>
// <summary>
//   The base view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Opuno.Brenn.ViewModels
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// The base view model.
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanged
    {
        /// <summary>
        ///   Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Loads from the model.
        /// </summary>
        public abstract void LoadFromModel();

        /// <summary>
        /// Saves the view model to the model.
        /// </summary>
        public abstract bool SaveToModel();

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property.
        /// </param>
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}