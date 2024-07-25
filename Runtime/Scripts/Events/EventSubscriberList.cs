using System.Collections.Generic;
using System;

namespace m039.Common
{
    internal class EventSubscriberList<T> where T : class
    {
        bool _needsCleanUp = false;

        public bool Executing;

        public readonly List<WeakReference> List = new();

        public void Add(T subscriber)
        {
            List.Add(new WeakReference(subscriber));
        }

        public void RemoveAt(int index)
        {
            if (Executing)
            {
                _needsCleanUp = true;
                List[index] = null;
            }
            else
            {
                List.RemoveAt(index);
            }
        }

        public void Remove(T subscriber)
        {
            Remove(x => x == subscriber);
        }

        public void Remove(Func<T, bool> predicate)
        {
            if (Executing)
            {
                var i = List.FindIndex(x => predicate((T)x.Target));
                if (i >= 0)
                {
                    _needsCleanUp = true;
                    List[i] = null;
                }
            }
            else
            {
                var i = List.FindIndex(x => predicate((T)x.Target));
                if (i >= 0)
                {
                    List.RemoveAt(i);
                }
            }
        }

        

        public void CleanUp()
        {
            if (!_needsCleanUp)
                return;

            List.RemoveAll(x => x == null);
            _needsCleanUp = false;
        }
    }
}
