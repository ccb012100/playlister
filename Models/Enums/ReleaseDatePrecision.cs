using System.Runtime.Serialization;

namespace Playlister.Models.Enums
{
    public enum ReleaseDatePrecision
    {
        [EnumMember(Value = "year")] Year,
        [EnumMember(Value = "month")] Month,
        [EnumMember(Value = "day")] Day
    }
}
