using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CarManagment.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base() { }

        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Полное имя")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Дата регистрации")]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    }
}