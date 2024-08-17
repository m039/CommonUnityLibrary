using System;

namespace m039.Common.Pathfindig
{
    public class FastPriorityQueue<T> where T : IComparable<T>
    {
        T[] _data;

        int _length = 0;

        const int IncreaseCoeff = 10;

        public int Count { get { return _length; } }

        void Add(T item)
        {
            if (_data == null)
            {
                _data = new T[1];
                _data[0] = item;
                _length = 1;
            } else
            {
                if (_length < _data.Length) {
                    _data[_length] = item;
                    _length++;
                } else
                {
                    var newArray = new T[_length + IncreaseCoeff];
                    Array.Copy(_data, newArray, _length);
                    newArray[_length] = item;

                    _data = newArray;
                    _length++;
                }
            }
        }

        void RemoveLast()
        {
            _length--;
        }

        public void Enqueue(T item)
        {
            Add(item);

            int childIndex = _length - 1;

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
            int lastIndex = _length - 1;

            T frontItem = _data[0];

            _data[0] = _data[lastIndex];
            RemoveLast();
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
            if (item == null || _data == null)
                return false;

            return Array.IndexOf(_data, item, 0, _length) != -1;
        }

        public void Clear()
        {
            _length = 0;
        }
    }
}
