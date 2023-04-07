using System;

namespace Playlister.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateTokenAttribute : Attribute { }
}
