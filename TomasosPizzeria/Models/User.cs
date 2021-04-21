using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TomasosPizzeria.Models
{
    public class User
    {
        [DisplayName("Namn")]
        [Required(ErrorMessage = "Namn är obligatoriskt!")]
        [StringLength(100, ErrorMessage = "Namn får vara max 100 tecken långt")]
        public string Namn  { get; set; }

        [DisplayName("Adress")]
        [Required(ErrorMessage = "Adress är obligatoriskt!")]
        [StringLength(50, ErrorMessage = "Namn får vara max 50 tecken långt")]
        public string Gatuadress { get; set; }

        [DisplayName("Postnr")]
        [Required(ErrorMessage = "Postnr är obligatoriskt!")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Får bara innehålla siffror")]
        [StringLength(20, ErrorMessage = "Namn får vara max 20 tecken långt")]
        public string Postnr { get; set; }

        [DisplayName("Postort")]
        [Required(ErrorMessage = "Postort är obligatoriskt!")]
        [StringLength(100, ErrorMessage = "Namn får vara max 100 tecken långt")]
        
        public string Postort { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email är obligatoriskt!")]
        [StringLength(256, ErrorMessage = "Namn får vara max 256 tecken långt")]
        public string Email { get; set; }

        [DisplayName("Telefon")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Får bara innehålla siffror")]
        public string Telefon { get; set; }

        [DisplayName("Användarnamn")]
        [Required(ErrorMessage = "Användarnamn är obligatoriskt!")]
        [StringLength(256, ErrorMessage = "Namn får vara max 256 tecken långt")]
        public string AnvandarNamn { get; set; }

        [DisplayName("Lösenord")]
        [Required(ErrorMessage = "Lösenord är obligatoriskt!")]
        public string Losenord { get; set; }

        public int Points { get; set; }

        public User()
        {

        }

    }
}
