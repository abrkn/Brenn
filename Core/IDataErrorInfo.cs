// -----------------------------------------------------------------------
// <copyright file="IDataErrorInfo.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

#if WINDOWS_PHONE
namespace Opuno.Brenn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IDataErrorInfo
    {
        string this[string columnName]
        {
            get;
        }

        string Error
        {
            get;
        }
    }
}
#endif
