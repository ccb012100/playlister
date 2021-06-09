using System;

namespace Playlister
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ValidateTokenAttribute : Attribute
    {
    }
}
