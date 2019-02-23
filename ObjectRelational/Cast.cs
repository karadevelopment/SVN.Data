using System;

namespace SVN.Data.ObjectRelational
{
    internal interface ICast
    {
    }

    internal class Cast<T1, T2> : ICast
    {
        public T1 Type1 { get; set; }
        public T2 Type2 { get; set; }
        public Func<T1, T2> Handle { get; set; }

        public T2 Invoke(T1 param)
        {
            return this.Handle(param);
        }
    }
}