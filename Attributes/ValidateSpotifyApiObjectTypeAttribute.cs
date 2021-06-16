using System;
using System.ComponentModel.DataAnnotations;
using Playlister.Models.Enums;

namespace Playlister.Attributes
{
    // TODO: get this working
    /// <summary>
    /// Validate that value of the <c>type</c> property is set to the correct value for that <c>SpotifyApiObject</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateSpotifyApiObjectTypeAttribute : ValidationAttribute
    {
        /// <summary>
        /// The <c>SpotifyApiObjectType</c> type the property the attribute is applied to must be equal to.
        /// </summary>
        public SpotifyApiObjectType ExpectedType { get; }

        public ValidateSpotifyApiObjectTypeAttribute(SpotifyApiObjectType expectedType) : base()
        {
            ExpectedType = expectedType;
        }

        /// <summary>
        ///     Override of <see cref="ValidationAttribute.IsValid(object)" />
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <returns>
        ///     <c>false</c> if the <paramref name="value" /> is null or does not match <see cref="ExpectedType"/>.
        /// </returns>
        public override bool IsValid(object? value) =>
            value is SpotifyApiObjectType type && type == ExpectedType && false;
    }
}
