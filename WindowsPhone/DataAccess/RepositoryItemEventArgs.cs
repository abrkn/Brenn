namespace Opuno.Brenn.WindowsPhone.DataAccess
{
    using System;
    using System.Diagnostics;

    using Opuno.Brenn.Models;

    public class RepositoryItemEventArgs : EventArgs
    {
        public RepositoryItemEventArgs()
        {
        }

        public RepositoryItemEventArgs(int key, IClientModel model)
        {
            Debug.Assert(key > 0);

            this.Key = key;
            this.Model = model;
        }

        public int Key { get; set; }

        public IClientModel Model { get; set; }
    }
}