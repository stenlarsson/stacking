using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.GamerServices
{
    // TODO: Sort out the generics mess...
    public class GamerCollection<T>
        : ReadOnlyCollection<T>, IEnumerable<T/* Gamer in documentation */>, System.Collections.IEnumerable
            where T : Gamer
    {
        internal GamerCollection(IList<T> list) : base(list)
        {
        }

        public struct GamerCollectionEnumerator
            : IEnumerator<T>, IDisposable, System.Collections.IEnumerator
        {
            IEnumerator<T> wrapped;

            public GamerCollectionEnumerator (IEnumerator<T> wrapped)
            {
                this.wrapped = wrapped;
            }

            public T Current
            {
                get { return wrapped.Current; }
            }

            public bool MoveNext()
            {
                return wrapped.MoveNext();
            }

            Object System.Collections.IEnumerator.Current
            {
                get { return wrapped.Current; }
            }

            void System.Collections.IEnumerator.Reset()
            {
                wrapped.Reset();
            }

            public void Dispose()
            {
                wrapped = null;
            }
        }

        public new GamerCollectionEnumerator GetEnumerator ()
        {
            return new GamerCollectionEnumerator(base.GetEnumerator());
        }
    }
}
