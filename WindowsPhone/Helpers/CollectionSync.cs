using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Opuno.Brenn.WindowsPhone.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    public static class CollectionSync
    {
        public static void Sync<TSource, TDest>(ICollection<TSource> source, ICollection<TDest> dest, Func<TSource, TDest, bool> compare,
            Action<TSource> addToDest, Action<TDest> removeFromDest, Action<TSource, TDest> updateDest)
            where TSource : class
            where TDest : class
        {
            foreach (var d in dest)
            {
                if (!source.Any(s => compare(s, d)))
                {
                    removeFromDest(d);
                }
            }

            foreach (var s in source)
            {
                var d = dest.SingleOrDefault(x => compare(s, x));

                if (d == null)
                {
                    addToDest(s);
                }
                else
                {
                    updateDest(s, d);
                }
            }
        }
    }
}
