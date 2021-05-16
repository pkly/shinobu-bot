using System;

namespace Shinobu.Attributes
{
    /// <summary>
    /// If marked, a command will not appear in help
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HiddenCommandAttribute : Attribute
    {
    }
}