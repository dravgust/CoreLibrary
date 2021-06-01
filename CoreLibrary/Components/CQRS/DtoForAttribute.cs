using System;

namespace CoreLibrary.Components.CQRS
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DtoForAttribute : Attribute
    {
        public Type EntityType { get; }

        public DtoForAttribute(Type entityType)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        }
    }
}
