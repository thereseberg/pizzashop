using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace TomasosPizzeria.Models
{
    public partial class Produkt
    {
        public Produkt()
        {
            MatrattProdukts = new HashSet<MatrattProdukt>();
        }

        public Produkt(int produktId, string produktNamn)
        {
            ProduktId = produktId;
            ProduktNamn = produktNamn;
        }

        public int ProduktId { get; set; }

        [DisplayName("Ingrediens")]
        [Required(ErrorMessage = "Ingrediens är obligatoriskt!")]
        [StringLength(50, ErrorMessage = "Namn får vara max 50 tecken långt")]
        public string ProduktNamn { get; set; }

        [NotMapped]
        public bool IsChecked { get; set; }

        public virtual ICollection<MatrattProdukt> MatrattProdukts { get; set; }
    }
}
