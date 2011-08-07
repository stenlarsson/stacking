using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net
{
	public sealed class NetworkSession : IDisposable
	{
#pragma warning disable 0067
        public static event EventHandler<InviteAcceptedEventArgs> InviteAccepted;

        public event EventHandler<GamerJoinedEventArgs> GamerJoined;
        public event EventHandler<GamerLeftEventArgs> GamerLeft;
        public event EventHandler<GameStartedEventArgs> GameStarted;
        public event EventHandler<GameEndedEventArgs> GameEnded;
        public event EventHandler<NetworkSessionEndedEventArgs> SessionEnded;
#pragma warning restore 0067

        public NetworkSession (
            IEnumerable<SignedInGamer> localGamers,
            int maxGamers)
        {
            SessionState = NetworkSessionState.Lobby;
            MaxGamers = maxGamers;
        }

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
            get { return NetworkSessionType.Local; }
        }

        public bool IsHost
        {
            get { return true; }
        }

        public bool IsEveryoneReady
        {
            get
            {
                return allGamers.All((gamer) => gamer.IsReady);
            }
        }

        IList<NetworkGamer> allGamers = new List<NetworkGamer>();
        public GamerCollection<NetworkGamer> AllGamers
        {
            get { return new GamerCollection<NetworkGamer>(allGamers); }
        }

        IList<NetworkGamer> remoteGamers = new List<NetworkGamer>();
        public GamerCollection<NetworkGamer> RemoteGamers
        {
            get { return new GamerCollection<NetworkGamer>(remoteGamers); }
        }

        IList<LocalNetworkGamer> localGamers = new List<LocalNetworkGamer>();
        IList<LocalNetworkGamer> waitingLocalGamers = new List<LocalNetworkGamer>();
        public GamerCollection<LocalNetworkGamer> LocalGamers
        {
            get { return new GamerCollection<LocalNetworkGamer>(localGamers); }
        }

        public int MaxGamers
        {
            get; private set;
        }

        public void StartGame()
        {
            SessionState = NetworkSessionState.Playing;
            GameStarted.Invoke(this, new GameStartedEventArgs());
        }

        public void EndGame()
        {
            SessionState = NetworkSessionState.Lobby;
            GameEnded.Invoke(this, new GameEndedEventArgs());
        }

        public NetworkGamer FindGamerById(byte gamerId)
        {
            return allGamers[(int)gamerId];
        }

        public void AddLocalGamer(SignedInGamer gamer)
        {
            if (allGamers.Count + waitingLocalGamers.Count == MaxGamers)
                throw new InvalidOperationException();

            waitingLocalGamers.Add(new LocalNetworkGamer(gamer.Gamertag, (byte)gamer.PlayerIndex));
        }


        static NetworkSession Nop(NetworkSession session)
        {
            return session;
        }
        delegate NetworkSession NopDelegate(NetworkSession session);
        static NopDelegate nopDelegate = null;

        public static IAsyncResult BeginCreate(
            NetworkSessionType sessionType,
            IEnumerable<SignedInGamer> localGamers,
            int maxGamers,
            int privateGamerSlots,
            NetworkSessionProperties sessionProperties,
            AsyncCallback callback,
            Object asyncState)
        {
            if (sessionType != NetworkSessionType.Local)
                throw new NotImplementedException();
            if (nopDelegate != null)
                throw new InvalidOperationException();

            nopDelegate = new NopDelegate(Nop);
            NetworkSession session = new NetworkSession(localGamers, maxGamers);
            return nopDelegate.BeginInvoke(session, callback, asyncState);
        }

        public static NetworkSession EndCreate(IAsyncResult result)
        {
            NetworkSession session = nopDelegate.EndInvoke(result);
            nopDelegate = null;
            return session;
        }

        public static IAsyncResult BeginFind(
            NetworkSessionType sessionType,
            IEnumerable<SignedInGamer> localGamers,
            NetworkSessionProperties searchProperties,
            AsyncCallback callback,
            Object asyncState)
        {
            // Find is not allowed on local network sessions...
            if (sessionType == NetworkSessionType.Local)
                throw new ArgumentException();
            else
                throw new NotSupportedException();
        }

        public static AvailableNetworkSessionCollection EndFind(IAsyncResult result)
        {
            throw new NotSupportedException();
        }

        public static IAsyncResult BeginJoin(
            AvailableNetworkSession availableSession,
            AsyncCallback callback,
            Object asyncState)
        {
            throw new NotSupportedException();
        }

        public static NetworkSession EndJoin(IAsyncResult result)
        {
            throw new NotSupportedException();
        }

        public static IAsyncResult BeginJoinInvited(
            IEnumerable<SignedInGamer> localGamers,
            AsyncCallback callback,
            Object asyncState)
        {
            throw new NotSupportedException();
        }

        public static NetworkSession EndJoinInvited(IAsyncResult result)
        {
            throw new NotSupportedException();
        }

        public void Update()
        {
            foreach (var gamer in waitingLocalGamers)
            {
                localGamers.Add(gamer);
                allGamers.Add(gamer);
                GamerJoined.Invoke(this, new GamerJoinedEventArgs(gamer));
            }
            waitingLocalGamers.Clear();

            foreach (var sender in localGamers)
            {
                while (sender.outgoing.Count > 0)
                {
                    var inpkt = sender.outgoing.Dequeue();
                    var outpkt = new Tuple<byte[], NetworkGamer>(inpkt.Item1, sender);
                    if (inpkt.Item2 != null)
                        ((LocalNetworkGamer)inpkt.Item2).incoming.Enqueue(outpkt);
                    else
                    {
                        foreach (var receiver in LocalGamers)
                            receiver.incoming.Enqueue(outpkt);
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}

