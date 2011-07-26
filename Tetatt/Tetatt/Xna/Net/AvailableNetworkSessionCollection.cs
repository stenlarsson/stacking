using System;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.Net
{
	public sealed class AvailableNetworkSessionCollection
        : ReadOnlyCollection<AvailableNetworkSession>, IDisposable
    {
        internal AvailableNetworkSessionCollection() : base(null)
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException ();
        }
	}
}

