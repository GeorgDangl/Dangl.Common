/*
This implementation allows compatibility for .NET 4.0. While it doesn't support the required attributes,
modern compilers are able to pick up the attributes and generate the correct functionality.

See here for more information:
https://thomaslevesque.com/2012/06/13/using-c-5-caller-info-attributes-when-targeting-earlier-versions-of-the-net-framework/
 */
#if NET40
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This is an implementation without any behavior. The full attribute name is recognized by
    /// compilers that support C# 5 or newer to generate the desired functionality.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class CallerMemberNameAttribute : Attribute
    {
    }
    
    /// <summary>
    /// This is an implementation without any behavior. The full attribute name is recognized by
    /// compilers that support C# 5 or newer to generate the desired functionality.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class CallerFilePathAttribute : Attribute
    {
    }
    
    /// <summary>
    /// This is an implementation without any behavior. The full attribute name is recognized by
    /// compilers that support C# 5 or newer to generate the desired functionality.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class CallerLineNumberAttribute : Attribute
    {
    }
}
#endif
