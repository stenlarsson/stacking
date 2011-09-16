using System;
using System.Runtime.Remoting.Messaging;

namespace Microsoft.Xna.Framework.Storage
{
    public sealed class StorageDevice
    {
        delegate StorageDevice DeviceDelegate();
        static DeviceDelegate deviceDelegate;
        public static IAsyncResult BeginShowSelector(PlayerIndex player, AsyncCallback callback, object state)
        {
            if (deviceDelegate != null)
                throw new InvalidOperationException();

            deviceDelegate = () => new StorageDevice();
            return deviceDelegate.BeginInvoke(callback, state);
        }

        public static StorageDevice EndShowSelector(IAsyncResult result)
        {
            DeviceDelegate invoker = deviceDelegate;
            deviceDelegate = null;
            return invoker.EndInvoke(result);
        }

        delegate StorageContainer ContainerDelegate();
        ContainerDelegate containerDelegate;
        public IAsyncResult BeginOpenContainer(string containerName, AsyncCallback callback, object state)
        {
            if (containerDelegate != null)
                throw new InvalidOperationException();
            containerDelegate = () => new StorageContainer(containerName);
            return containerDelegate.BeginInvoke(callback, state);
        }

        public StorageContainer EndOpenContainer(IAsyncResult result)
        {
            ContainerDelegate invoker = containerDelegate;
            containerDelegate = null;
            return invoker.EndInvoke(result);
        }
    }
}

