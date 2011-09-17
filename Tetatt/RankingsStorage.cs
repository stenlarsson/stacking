using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Tetatt.ArtificialIntelligence;
using Tetatt.Screens;

namespace Tetatt
{
    using Rankings = Dictionary<Level, List<Result>>;
    
    struct Result : IComparable<Result>
    {
        public int Ticks;
        public string Gamertag;

        public int CompareTo(Result other)
        {
            return Ticks.CompareTo(other.Ticks);
        }
    }

    enum RankingsState
    {
        /// <summary>
        /// Only results for the current game instance are collected (and not stored on disk).
        /// </summary>
        Session,
        /// <summary>
        /// Results from file are taken into account in the rankings, but the serialized version on disk needs be updated.
        /// </summary>
        SerializationNeeded,
        /// <summary>
        /// Session results have been merged with stored rankings and the stored results are up to date.
        /// </summary>
        Storage
    }


    /// <summary>
    /// The rankings component handles storage of rankings to disk. It is implemented as a
    /// game component and can be added to the game services. The component automatically
    /// performs all the filesystem tasks when the ranking table is updated. Until the
    /// filesystem has been accessed, the class operates in session mode, where the
    /// highscores for the current session only are collected. Once the filesystem is
    /// accessed the current session scores are merged with the rankings on file, and
    /// operation in storage mode is commenced, where the rankings stored on file are
    /// simply overwritten upon update.
    /// </summary>
    class RankingsStorage : GameComponent
    {
        const string containerName = "Rankings";
        const string fileName = "Rankings.xml";

        StorageDevice device;
        // The async result is used for getting the storage device when device == null, or
        // for getting the storage container when device != null.
        IAsyncResult result; 
        
        /// <summary>
        /// Describes to what extent the rankings are synchronized with their on-disk counterpart.
        /// </summary>
        public RankingsState State { get; private set; }

        /// <summary>
        /// The in-memory rankings for this RankingsStorage.
        /// </summary>
        public Rankings Rankings { get; private set; }

        /// <summary>
        /// Create new RankingStorage component and add to game services.
        /// </summary>
        /// <param name="game"></param>
        public RankingsStorage(Game game) : base(game)
        {
            Enabled = false;
            State = RankingsState.Session;
            Rankings = new Rankings();
            foreach (var level in Levels.All)
                Rankings.Add(level, new List<Result>(20));

            game.Services.AddService(typeof(RankingsStorage), this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (result.IsCompleted)
            {
                if (device == null)
                {
                    device = StorageDevice.EndShowSelector(result);
                    result = (device == null) ? null : device.BeginOpenContainer(containerName, null, null);
                }
                else
                {
                    Enabled = false;
                    try
                    {
                        using (StorageContainer container = device.EndOpenContainer(result))
                        {
                            Stream file = container.OpenFile(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                            UpdateOnDisk(file);
                            file.Close();
                            result = null;
                        }
                        State = RankingsState.Storage;
                    }
                    catch (IOException e)
                    {
                        ScreenManager screenManager = (ScreenManager)Game.Services.GetService(typeof(ScreenManager));
                        MessageBoxScreen msg = new MessageBoxScreen(screenManager, Resources.ErrorStoreRankings + "\n" + e.Message);
                        msg.Accepted += (_,arg) => BeginDiskSync(arg.PlayerIndex);
                        screenManager.AddScreen(msg, null);
                    }
                }
            }
        }

        // Assumes that the file has been opened for reading and writing
        void UpdateOnDisk(Stream file)
        {
            if (State == RankingsState.Session)
            {
                if (file.Length == 0)
                    State = RankingsState.SerializationNeeded;
                else
                {
                    // Change stored rather than Rankings, so that we haven't changed
                    // anything when an exception occurs.
                    bool changed = false;
                    Rankings stored = Deserialize(new StreamReader(file));
                    foreach (var kvp in Rankings)
                    {
                        foreach (var result in kvp.Value)
                        {
                            if (AddIfBetter(stored[kvp.Key], result))
                                changed = true;
                            else
                                break; // The remaining should be worse
                        }
                    }
                    Rankings = stored;
                    State = changed ? RankingsState.SerializationNeeded : RankingsState.Storage;
                }
            }
            if (State != RankingsState.Storage)
            {
                file.SetLength(0);
                Serialize(new StreamWriter(file), Rankings);
            }
        }

        static void Serialize(StreamWriter file, Rankings rankings)
        {
            foreach (var ranking in rankings)
            {
                file.WriteLine(ranking.Key);
                foreach (var entry in ranking.Value)
                {
                    file.WriteLine("{0} {1}", entry.Ticks, entry.Gamertag);
                }
            }
        }

        static Rankings Deserialize(StreamReader file)
        {
            Rankings rankings = new Rankings();
            List<Result> ranking = null;
            string line;
            while ((line = file.ReadLine()) != null)
            {
                int index = line.IndexOf(' ');
                if (index < 0)
                {
                    ranking = new List<Result>();
                    rankings[(Level)Enum.Parse(typeof(Level), line, false)] = ranking;
                }
                else
                {
                    Result result = new Result();
                    result.Ticks = int.Parse(line.Substring(0, index));
                    result.Gamertag = line.Substring(index + 1);
                    ranking.Add(result);
                }
            }
            return rankings;
        }

        static bool AddIfBetter(List<Result> ranking, Result entry)
        {
            int i = 0;
            for (; i < ranking.Count; i++)
            {
                if (ranking[i].Ticks > entry.Ticks)
                    break;
            }

            bool shouldInsert = (i < 10);
            if (shouldInsert)
            {
                // Remove first to avoid buffer allocation
                if (ranking.Count >= 10)
                    ranking.RemoveAt(9);

                ranking.Insert(i, entry);
            }

            return shouldInsert;
        }

        public void AddResult(Level level, Result entry, PlayerIndex player)
        {
            if (AddIfBetter(Rankings[level], entry))
            {
                State = RankingsState.SerializationNeeded;
                BeginDiskSync(player);
            }
        }

        /// <summary>
        /// Start the process of synchronizing from and to disk.
        /// </summary>
        /// <param name="player">The controlling player that initiated this action</param>
        public void BeginDiskSync(PlayerIndex player)
        {
            if (result == null && State != RankingsState.Storage)
            {
                if (device == null)
                    result = StorageDevice.BeginShowSelector(player, null, null);
                else
                    result = device.BeginOpenContainer(containerName, null, null);
                Enabled = true;
            }
        }
    }
}
