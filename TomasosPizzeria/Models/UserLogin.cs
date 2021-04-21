using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TomasosPizzeria.Models
{
    public class UserLogin
    {
        [DisplayName("Användarnamn")]
        [Required(ErrorMessage = "Användarnamn är obligatoriskt!")]
        public string Username { get; set; }

        [DisplayName("Lösenord")]
        [Required(ErrorMessage = "Lösenord är obligatoriskt!")]
        public string Password { get; set; }
    }
}
