using System.ComponentModel.DataAnnotations;

namespace vroom.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "ელფოსტა აუცილებელია")]
        [EmailAddress(ErrorMessage = "მოწოდებული ელფოსტა ფორმატი არასწორია")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "სახელი აუცილებელია")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "მოწოდებული ტელეფონის ნომერი ფორმატი არასწორია")]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "პაროლი აუცილებელია")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "პაროლი უნდა იყოს მინიმუმ 6 სიმბოლო")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "პაროლი და დადასტურების პაროლი არ ემთხვევა")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "ელფოსტა აუცილებელია")]
        [EmailAddress(ErrorMessage = "მოწოდებული ელფოსტა ფორმატი არასწორია")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "პაროლი აუცილებელია")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "დამიმახსოვრე")]
        public bool RememberMe { get; set; }
    }

    public class ProfileViewModel
    {
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }
    }
}

