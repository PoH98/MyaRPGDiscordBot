using Autofac;
using MyaDiscordBot.Models;
using MyaDiscordBot.Models.Antiscam;
using MyaDiscordBot.Models.MyaWebsite;
using Timer = System.Timers.Timer;

namespace MyaDiscordBot
{
    internal class Data
    {
        private static Data _instance;
        public static Data Instance
        {
            get
            {
                _instance ??= new Data();
                return _instance;
            }
        }
        public IContainer Container { get; set; }
        public ExpirableList<Tuple<ulong, Enemy>> Boss { get; set; } = new ExpirableList<Tuple<ulong, Enemy>>(604800000);
        public ExpirableList<ulong> CacheDisableResponse { get; set; } = new ExpirableList<ulong>(5000);
        public int LastRnd { get; set; }
        public ExpirableList<AntiscamData> ScamList { get; set; } = new ExpirableList<AntiscamData>(3600000);
        public ExpirableList<string> PornList { get; set; } = new ExpirableList<string>(3600000);
        public YTData Youtube { get; set; } = new YTData();
        public ExpirableList<string> BannedName { get; set; } = new ExpirableList<string>(3600000);
        public ExpirableList<string> BannedRegex { get; set; } = new ExpirableList<string>(3600000);
        public ExpirableList<string> ShortenUrl { get; set; } = new ExpirableList<string>(3600000);
        public ExpirableList<ulong> SelfBots { get; set; } = new ExpirableList<ulong>(3600000);
    }

    public class ExpirableList<T> : IList<T>
    {
        private readonly List<Tuple<DateTime, T>> collection = new();

        private readonly Timer timer;

        public double Interval
        {
            get => timer.Interval;
            set => timer.Interval = value;
        }

        public TimeSpan Expiration { get; set; }

        /// <summary>
        /// Define a list that automaticly remove expired objects.
        /// </summary>
        /// <param name="_interval"></param>
        /// The interval at which the list test for old objects.
        /// <param name="_expiration"></param>
        /// The TimeSpan an object stay valid inside the list.
        public ExpirableList(int _interval, TimeSpan? _expiration = null)
        {
            timer = new Timer
            {
                Interval = _interval
            };
            timer.Elapsed += Tick;
            timer.Start();
            if (_expiration == null)
            {
                _expiration = new TimeSpan(0, 5, 0);
            }
            Expiration = _expiration.Value;
        }

        private void Tick(object sender, EventArgs e)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if ((DateTime.Now - collection[i].Item1) >= Expiration)
                {
                    collection.RemoveAt(i);
                }
            }
        }

        #region IList Implementation
        public T this[int index]
        {
            get => collection[index].Item2;
            set => collection[index] = new Tuple<DateTime, T>(DateTime.Now, value);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return collection.Select(x => x.Item2).GetEnumerator();
        }
        public IEnumerator<T> GetEnumerator()
        {
            return collection.Select(x => x.Item2).GetEnumerator();
        }

        public void Add(T item)
        {
            collection.Add(new Tuple<DateTime, T>(DateTime.Now, item));
        }

        public int Count => collection.Count;

        public bool IsSynchronized => false;

        public bool IsReadOnly => false;

        public void CopyTo(T[] array, int index)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                array[i + index] = collection[i].Item2;
            }
        }

        public bool Remove(T item)
        {
            bool contained = Contains(item);
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if ((object)collection[i].Item2 == (object)item)
                {
                    collection.RemoveAt(i);
                }
            }
            return contained;
        }

        public void RemoveAt(int i)
        {
            collection.RemoveAt(i);
        }

        public bool Contains(T item)
        {
            return collection.Any(x => x.Item2.Equals(item));
        }

        public void Insert(int index, T item)
        {
            collection.Insert(index, new Tuple<DateTime, T>(DateTime.Now, item));
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if ((object)collection[i].Item2 == (object)item)
                {
                    return i;
                }
            }

            return -1;
        }

        public void Clear()
        {
            collection.Clear();
        }

        #endregion
    }
}
