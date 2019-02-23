using SVN.Core.Time;
using SVN.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SVN.Data.ObjectRelational
{
    public static class AutoMapper
    {
        private static List<ICast> Casts => new List<ICast>
        {
            new Cast<long, DateTime> { Handle = (x => x.ToDateTime()) },
            new Cast<long, DateTime?> { Handle = (x => x.ToDateTime()) },
            new Cast<DateTime, long> { Handle = (x => x.ToTimeStamp()) },
            new Cast<DateTime?, long> { Handle = (x => x.ToTimeStamp()) }
        };

        public static void Register<T1, T2>(Func<T1, T2> handle)
        {
            AutoMapper.Casts.Add(new Cast<T1, T2>() { Handle = handle });
        }

        public static List<TDestination> Map<TSource, TDestination>(IQueryable<TSource> s, Action<TSource, TDestination> delegator = null)
            where TSource : class
            where TDestination : class, new()
        {
            return s.Select(x => AutoMapper.Map<TSource, TDestination>(x, null, delegator)).ToList();
        }

        public static List<TDestination> Map<TSource, TDestination>(ICollection<TSource> s, Action<TSource, TDestination> delegator = null)
            where TSource : class
            where TDestination : class, new()
        {
            return s.Select(x => AutoMapper.Map<TSource, TDestination>(x, null, delegator)).ToList();
        }

        public static List<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> s, Action<TSource, TDestination> delegator = null)
            where TSource : class
            where TDestination : class, new()
        {
            return s.Select(x => AutoMapper.Map<TSource, TDestination>(x, null, delegator)).ToList();
        }

        public static List<TDestination> Map<TSource, TDestination>(IOrderedEnumerable<TSource> s, Action<TSource, TDestination> delegator = null)
            where TSource : class
            where TDestination : class, new()
        {
            return s.Select(x => AutoMapper.Map<TSource, TDestination>(x, null, delegator)).ToList();
        }

        public static List<TDestination> Map<TSource, TDestination>(List<TSource> s, Action<TSource, TDestination> delegator = null)
            where TSource : class
            where TDestination : class, new()
        {
            return s.Select(x => AutoMapper.Map<TSource, TDestination>(x, null, delegator)).ToList();
        }

        public static TDestination Map<TSource, TDestination>(TSource s, TDestination d = null, Action<TSource, TDestination> delegator = null)
            where TSource : class
            where TDestination : class, new()
        {
            d = (d ?? new TDestination());
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
                    var value = sProperty.GetValue(s);
                    if (value != null && value != sProperty.PropertyType.GetDefault()) dProperty.SetValue(d, value);
                    continue;
                }

                foreach (var cast in AutoMapper.Casts)
                {
                    var type = cast.GetType();
                    var property1 = type.GetProperty("Type1");
                    var property2 = type.GetProperty("Type2");
                    var method = type.GetMethod("Invoke");

                    if (sProperty.PropertyType == property1.PropertyType && dProperty.PropertyType == property2.PropertyType)
                    {
                        var value = sProperty.GetValue(s);
                        value = method.Invoke(cast, new object[] { value });
                        if (value != null) dProperty.SetValue(d, value);
                        continue;
                    }
                }
            }
            delegator?.Invoke(s, d);

            return d;
        }
    }
}