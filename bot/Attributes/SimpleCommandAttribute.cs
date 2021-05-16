using System;

namespace Shinobu.Attributes
{
    /// <summary>
    /// This attribute marks a command as one for simple groups for help to be simplified
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SimpleCommandAttribute : Attribute
    {
    }
}