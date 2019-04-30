using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiplomaThesis.Common
{
    public static class ISetExtensions
    {
        public static void AddRange<T>(this ISet<T> set, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                set.Add(item);
            }
        }

        public static bool TryGetValue<T>(this ISet<T> set, T equalValue, out T actualValue)
        {
            actualValue = default(T);
            if (set is HashSet<T>)
            {
                var hashSet = (HashSet<T>)set;
                return hashSet.TryGetValue(equalValue, out actualValue);
            }
            var intersect = set.Intersect(new[] { equalValue });
            if (intersect.Count() == 1)
            {
                actualValue = intersect.FirstOrDefault();
                return true;
            }
            return false;
        }

        public static int RemoveWhere<T>(this ISet<T> set, Predicate<T> match)
        {
            if (set is HashSet<T>)
            {
                var hashSet = (HashSet<T>)set;
                return hashSet.RemoveWhere(match);
            }
            var valuesToRemove = new List<T>();
            foreach (var item in set)
            {
                if (match(item))
                {
                    valuesToRemove.Add(item);
                }
            }
            valuesToRemove.ForEach(x => set.Remove(x));
            return valuesToRemove.Count;
        }
    }
}
