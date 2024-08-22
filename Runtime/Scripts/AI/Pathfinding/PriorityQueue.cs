using System.Collections.Generic;
using System;

namespace m039.Common.Pathfindig
{
    /// <summary>
    /// The implementation is based on <see href="https://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx">this article</see> 
    /// </summary>
    public class PriorityQueue<T> where T : IComparable<T>
    {
        /// <see href="https://en.wikipedia.org/wiki/D-ary_heap"/>
        const int D = 4;

        readonly List<T> _data;

        public int Count => _data.Count;

        public PriorityQueue()
        {
            _data = new List<T>(128);
        }

        public void Enqueue(T item)
        {
            _data.Add(item);

            int childIndex = _data.Count - 1;

            while (childIndex > 0)
            {
                int parentIndex = (childIndex - 1) / D;

                if (_data[childIndex].CompareTo(_data[parentIndex]) >= 0)
                {
                    break;
                }

                T tmp = _data[childIndex];
                _data[childIndex] = _data[parentIndex];
                _data[parentIndex] = tmp;

                childIndex = parentIndex;
            }
        }

        public T Dequeue()
        {
            int lastIndex = _data.Count - 1;

            T frontItem = _data[0];

            _data[0] = _data[lastIndex];
            _data.RemoveAt(lastIndex);
            lastIndex--;

            int parentIndex = 0;

            while (true)
            {
                int leftChild = parentIndex * D + 1;

                if (leftChild > lastIndex)
                {
                    break;
                }

                for (int i = 2; i <= D; i++)
                {
                    var rightChild = parentIndex * D + i;
                    if (rightChild > lastIndex)
                        break;

                    if (_data[rightChild].CompareTo(_data[leftChild]) < 0)
                    {
                        leftChild = rightChild;
                    }
                }

                if (_data[parentIndex].CompareTo(_data[leftChild]) <= 0)
                {
                    break;
                }

                T tmp = _data[parentIndex];
                _data[parentIndex] = _data[leftChild];
                _data[leftChild] = tmp;

                parentIndex = leftChild;
            }

            return frontItem;
        }

        public T Peek()
        {
            T frontItem = _data[0];
            return frontItem;
        }

        public bool Contains(T item)
        {
            return _data.Contains(item);
        }

        public List<T> ToList()
        {
            return _data;
        }

        public void Clear()
        {
            _data.Clear();
        }
    }
}
