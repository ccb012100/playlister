using System.ComponentModel.DataAnnotations;

namespace Playlister.ViewModels
{
    public class LoginViewModel
    {
        [Required] public required string Code { get; init; }
        [Required] public required string State { get; init; }
    }
}
