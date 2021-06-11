using System;

namespace Playlister.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ValidateTokenAttribute : Attribute
    {
    }
}
