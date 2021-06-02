using System;
using JetBrains.Annotations;

namespace CoreLibrary.Components.CQRS
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DtoForAttribute : Attribute
    {
        public Type EntityType { get; }

        public DtoForAttribute([NotNull] Type entityType)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        }
    }
}
