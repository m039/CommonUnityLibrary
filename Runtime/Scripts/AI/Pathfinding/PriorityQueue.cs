using System.Collections.Generic;
using System;

namespace m039.Common.Pathfindig
{
    public class PriorityQueue<T> where T : IComparable<T>
    {

        readonly List<T> _data;

        public int Count { get { return _data.Count; } }

        public PriorityQueue()
        {
            _data = new List<T>();
        }

        public void Enqueue(T item)
        {
            _data.Add(item);

            int childIndex = _data.Count - 1;

            while (childIndex > 0)
            {
                int parentIndex = (childIndex - 1) / 2;

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
                int childIndex = parentIndex * 2 + 1;

                if (childIndex > lastIndex)
                {
                    break;
                }

                int rightChild = childIndex + 1;

                if (rightChild <= lastIndex && _data[rightChild].CompareTo(_data[childIndex]) < 0) {
                    childIndex = rightChild;
                }

                if (_data[parentIndex].CompareTo(_data[childIndex]) <= 0)
                {
                    break;
                }

                T tmp = _data[parentIndex];
                _data[parentIndex] = _data[childIndex];
                _data[childIndex] = tmp;

                parentIndex = childIndex;
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
