using System;
using System.Data.Entity;
using System.Linq;

namespace SVN.Data.Linq
{
    public static class Extensions
    {
        public static T Extract<T>(this DbSet<T> param, Func<T, bool> predicate)
            where T : class
        {
            var item = param.FirstOrDefault(predicate);

            if (item != null)
            {
                param.Remove(item);
            }

            return item;
        }
    }
}