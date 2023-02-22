using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dangl.ObjectCopy
{
    // This was mostly taken from
    // https://stackoverflow.com/a/11308879/4190785
    internal static class ObjectCopyExtensions
    {
        public static object DeepCopy(this object originalObject)
        {
            return InternalCopy(originalObject,
                new Dictionary<object, object>());
        }

        private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        private static bool IsPrimitive(this Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }

#if NETSTANDARD_1_3
            return type.GetTypeInfo().IsValueType & type.GetTypeInfo().IsPrimitive;

#else
            return type.IsValueType & type.IsPrimitive;
#endif
        }

        private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
        {
            if (originalObject == null)
            {
                return null;
            }
            
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect))
            {
                return originalObject;
            }

            if (visited.TryGetValue(originalObject, out var previouslyVisitedObject))
            {
                return previouslyVisitedObject;
            }

            if (typeof(Delegate).IsAssignableFrom(typeToReflect))
            {
                return null;
            }

            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (!IsPrimitive(arrayType))
                {
                    var clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }
            }

            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
#if NETSTANDARD_1_3
            if (typeToReflect.GetTypeInfo().BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.GetTypeInfo().BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.GetTypeInfo().BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
#else
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
#endif
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && !filter(fieldInfo))
                {
                    continue;
                }
                
                if (IsPrimitive(fieldInfo.FieldType))
                {
                    continue;
                }
                
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
    }
}
