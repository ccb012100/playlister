using System;

namespace Playlister
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateTokenAttribute : Attribute
    {
    }
}