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

        public float growthFactor = 2;

        public int Count => _length;

        T[] _data;

        int _length = 0;

        public PriorityQueue(int capacity = 128)
        {
            _data = new T[capacity];
        }

        public void Enqueue(T item)
        {
            if (_length == _data.Length)
            {
                Expand();
            }
            _data[_length++] = item;

            int childIndex = _length - 1;

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

        void Expand()
        {
            int newSize = Math.Max(_data.Length + D, Math.Min(1 << 16, (int)Math.Round(_data.Length * growthFactor)));
            if (newSize > (1 << 16))
            {
                throw new Exception("Binary Heap Size really large (>65536).");
            }

            var newData = new T[newSize];
            _data.CopyTo(newData, 0);
            _data = newData;
        }

        public T Dequeue()
        {
            int lastIndex = _length - 1;

            T frontItem = _data[0];

            _data[0] = _data[lastIndex];
            _length--;
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

        public IEnumerable<T> GetEnumerable()
        {
            for (int i = 0; i < _length;i++)
            {
                yield return _data[i];
            }
        }

        public void Clear()
        {
            _length = 0;
        }
    }
}
