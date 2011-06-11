namespace Opuno.Brenn.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    public abstract class ViewModel<TModel> : ViewModel, IDataErrorInfo
        where TModel : class
    {
        public TModel Model { get; set; }

        public virtual string this[string columnName]
        {
            get
            {
                return null;
            }
        }

        protected string GetAnyError(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                var error = this[propertyName];

                if (error != default(string))
                {
                    return error;
                }
            }

            return null;
        }

        public string Error
        {
            get
            {
                return null;
            }
        }
    }
}
