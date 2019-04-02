using SVN.Core.Time;
using SVN.Reflection;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace SVN.Data.EntityFramework
{
    public static class Extensions
    {
        public static void Clear<T>(this DbSet<T> param)
            where T : class
        {
            param.RemoveRange(param);
        }

        public static void Remove<T>(this DbSet<T> param, Func<T, bool> predicate)
            where T : class
        {
            param.RemoveRange(param.Where(predicate));
        }

        public static List<TDestination> Map<TSource, TDestination>(this IQueryable<TSource> s, bool includeDefaults = false)
            where TSource : class
            where TDestination : class, new()
        {
            return s.Select(x => x.Map<TSource, TDestination>(includeDefaults)).ToList();
        }

        public static List<TDestination> Map<TSource, TDestination>(this ICollection<TSource> s, bool includeDefaults = false)
            where TSource : class
            where TDestination : class, new()
        {
            return s.Select(x => x.Map<TSource, TDestination>(includeDefaults)).ToList();
        }

        public static List<TDestination> Map<TSource, TDestination>(this IEnumerable<TSource> s, bool includeDefaults = false)
            where TSource : class
            where TDestination : class, new()
        {
            return s.Select(x => x.Map<TSource, TDestination>(includeDefaults)).ToList();
        }

        public static List<TDestination> Map<TSource, TDestination>(this IOrderedEnumerable<TSource> s, bool includeDefaults = false)
            where TSource : class
            where TDestination : class, new()
        {
            return s.Select(x => x.Map<TSource, TDestination>(includeDefaults)).ToList();
        }

        public static TDestination Map<TSource, TDestination>(this TSource s, bool includeDefaults = false)
            where TSource : class
            where TDestination : new()
        {
            return s.Map(new TDestination(), includeDefaults);
        }

        public static TDestination Map<TSource, TDestination>(this TSource s, TDestination d, bool includeDefaults = false)
            where TSource : class
            where TDestination : new()
        {
            if (s is null) return d;

            var bindingFlags = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            var sType = s.GetType();
            var dType = d.GetType();

            foreach (var sProperty in sType.GetProperties(bindingFlags))
            {
                var dProperty = dType.GetProperty(sProperty.Name, bindingFlags);

                if (dProperty is null)
                {
                    continue;
                }

                if (sProperty.PropertyType == dProperty.PropertyType)
                {
                    var sValue = sProperty.GetValue(s);
                    if (includeDefaults || sValue != null && sValue != sProperty.PropertyType.GetDefault()) dProperty.SetValue(d, sValue);
                }
                else if (sProperty.PropertyType == typeof(int?) && dProperty.PropertyType == typeof(int))
                {
                    var sValue = (int?)sProperty.GetValue(s);
                    if (sValue.HasValue) dProperty.SetValue(d, sValue.Value);
                }
                else if (sProperty.PropertyType == typeof(int) && dProperty.PropertyType == typeof(int?))
                {
                    var sValue = (int)sProperty.GetValue(s);
                    dProperty.SetValue(d, sValue);
                }
                else if (sProperty.PropertyType == typeof(DateTime) && dProperty.PropertyType == typeof(long))
                {
                    var sValue = sProperty.GetValue(s);
                    if (sValue != null && sValue != typeof(DateTime).GetDefault()) dProperty.SetValue(d, ((DateTime)sValue).ToTimeStamp());
                }
                else if (sProperty.PropertyType == typeof(long) && dProperty.PropertyType == typeof(DateTime))
                {
                    var sValue = sProperty.GetValue(s);
                    if (sValue != null && sValue != typeof(long).GetDefault()) dProperty.SetValue(d, ((long)sValue).ToDateTime());
                }
            }

            return d;
        }
    }
}