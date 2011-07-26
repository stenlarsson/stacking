using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.GamerServices
{
    public class GamerCollection<T>
        : ReadOnlyCollection<T>, IEnumerable<T/* Gamer in documentation */>, System.Collections.IEnumerable
            where T : Gamer
    {
        public GamerCollection() : base(null)
        {
            throw new NotImplementedException();
        }

        public struct GamerCollectionEnumerator
            : IEnumerator<T>, IDisposable, System.Collections.IEnumerator
        {
            private IEnumerator<T> wrapped;

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
