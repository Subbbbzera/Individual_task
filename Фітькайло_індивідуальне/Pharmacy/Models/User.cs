using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Прізвище обов'язкове")]
        public string LastName { get; set; }

        [Required, EmailAddress(ErrorMessage = "Невірний формат пошти")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обов'язковий")]
        public string Password { get; set; }

        public bool IsDoctor { get; set; }



    }

}
