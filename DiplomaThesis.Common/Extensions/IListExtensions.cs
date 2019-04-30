using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Common
{
    public static class IListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
        {
            if (list is List<T>)
            {
                ((List<T>)list).AddRange(collection);
            }
            else
            {
                foreach (var item in collection)
                {
                    list.Add(item);
                }
            }
        }
    }
}
