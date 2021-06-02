using System;
using JetBrains.Annotations;

namespace CoreLibrary.Components.CQRS
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ImplementationAttribute : Attribute
    {
        public ImplementationAttribute([NotNull] Type implementation)
        {
            Implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
        }

        public Type Implementation { get; }
    }
}
