using System.ComponentModel.DataAnnotations;

namespace CarManagment.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [Display(Name = "Имя")]
        [StringLength(50, ErrorMessage = "Имя не может быть длиннее 50 символов")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите фамилию")]
        [Display(Name = "Фамилия")]
        [StringLength(50, ErrorMessage = "Фамилия не может быть длиннее 50 символов")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите электронный адрес")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(100, ErrorMessage = "Пароль должен содержать минимум {2} символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
} 