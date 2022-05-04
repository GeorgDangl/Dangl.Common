using System;

namespace Dangl.ObjectCopy
{
    internal static class ArrayExtensions
    {
        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.Length == 0)
            {
                return;
            }
            
            ArrayTraverse walker = new ArrayTraverse(array);
            do
            {
                action(array, walker.Position);
            } while (walker.Step());
        }
    }
}
