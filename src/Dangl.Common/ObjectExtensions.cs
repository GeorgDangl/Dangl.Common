using Dangl.ObjectCopy;

namespace Dangl
{
    /// <summary>
    /// Static class that holds extension methods for <see cref="object"/>s.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// This creates a copy that is value but not reference of the original object. It does
        /// copy the object and all its properties, and also recursively traverses the tree. It handles
        /// circular references.
        /// Delegate types, like Action or Func, are not copied.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T original)
        {
            return (T)ObjectCopyExtensions.DeepCopy((object)original);
        }
    }
}
