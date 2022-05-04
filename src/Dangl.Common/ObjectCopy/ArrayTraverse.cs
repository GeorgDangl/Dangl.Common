using System;

namespace Dangl.ObjectCopy
{
    internal class ArrayTraverse
    {
        public int[] Position => _position;

        private int[] _position;
        private int[] _maxLengths;

        public ArrayTraverse(Array array)
        {
            _maxLengths = new int[array.Rank];
            for (int i = 0; i < array.Rank; ++i)
            {
                _maxLengths[i] = array.GetLength(i) - 1;
            }

            _position = new int[array.Rank];
        }

        public bool Step()
        {
            for (int i = 0; i < _position.Length; ++i)
            {
                if (_position[i] < _maxLengths[i])
                {
                    _position[i]++;
                    for (int j = 0; j < i; j++)
                    {
                        _position[j] = 0;
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
