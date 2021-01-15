using System;




namespace RI.Framework.Data.Repository.Entities
{
    /// <summary>
    ///     The attribute which can be used to let <see cref="EntityBase" /> ignore a property during serialization.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EntitySerializationIgnoreAttribute : Attribute
    {
    }
}
