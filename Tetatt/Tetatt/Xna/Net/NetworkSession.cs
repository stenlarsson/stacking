using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net
{
	public sealed class NetworkSession : IDisposable
	{
        public static event EventHandler<InviteAcceptedEventArgs> InviteAccepted;

        public event EventHandler<GamerJoinedEventArgs> GamerJoined;
        public event EventHandler<GamerLeftEventArgs> GamerLeft;
        public event EventHandler<GameStartedEventArgs> GameStarted;
        public event EventHandler<GameEndedEventArgs> GameEnded;
        public event EventHandler<NetworkSessionEndedEventArgs> SessionEnded;

        public bool AllowHostMigration
        {
            get; set;
        }

        public bool AllowJoinInProgress
        {
            get; set;
        }

        public NetworkSessionState SessionState
        {
            get; private set;
        }

        public NetworkSessionType SessionType
        {
            get; private set;
        }

        public bool IsHost
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsEveryoneReady
        {
            get { throw new NotImplementedException(); }
        }

        public GamerCollection<NetworkGamer> AllGamers
        {
            get { throw new NotImplementedException(); }
        }

        public GamerCollection<NetworkGamer> RemoteGamers
        {
            get { throw new NotImplementedException(); }
        }

        public GamerCollection<LocalNetworkGamer> LocalGamers
        {
            get { throw new NotImplementedException(); }
        }

        public int MaxGamers
        {
            get { throw new NotImplementedException(); }
        }

        public void StartGame()
        {
            throw new NotImplementedException();
        }

        public void EndGame()
        {
            throw new NotImplementedException();
        }

        public NetworkGamer FindGamerById(byte gamerId)
        {
            throw new NotImplementedException();
        }

        public void AddLocalGamer(SignedInGamer gamer)
        {
            throw new NotImplementedException();
        }

        public static IAsyncResult BeginCreate(
            NetworkSessionType sessionType,
            IEnumerable<SignedInGamer> localGamers,
            int maxGamers,
            int privateGamerSlots,
            NetworkSessionProperties sessionProperties,
            AsyncCallback callback,
            Object asyncState)
        {
            throw new NotImplementedException();
        }

        public static NetworkSession EndCreate(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public static IAsyncResult BeginFind(
            NetworkSessionType sessionType,
            IEnumerable<SignedInGamer> localGamers,
            NetworkSessionProperties searchProperties,
            AsyncCallback callback,
            Object asyncState)
        {
            throw new NotImplementedException();
        }

        public static AvailableNetworkSessionCollection EndFind(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public static IAsyncResult BeginJoin(
            AvailableNetworkSession availableSession,
            AsyncCallback callback,
            Object asyncState)
        {
            throw new NotImplementedException();
        }

        public static NetworkSession EndJoin(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public static IAsyncResult BeginJoinInvited(
            IEnumerable<SignedInGamer> localGamers,
            AsyncCallback callback,
            Object asyncState)
        {
            throw new NotImplementedException();
        }

        public static NetworkSession EndJoinInvited(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

