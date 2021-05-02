using System;

namespace Shinobu.Attributes
{
    /// <summary>
    /// This attribute allows us to later identify which group the class belongs to
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SectionAttribute : Attribute
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }

        public SectionAttribute(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }
    }
}