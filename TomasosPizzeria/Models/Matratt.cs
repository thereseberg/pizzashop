using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace TomasosPizzeria.Models
{
    public partial class Matratt
    {
        public Matratt()
        {
            BestallningMatratts = new HashSet<BestallningMatratt>();
            MatrattProdukts = new HashSet<MatrattProdukt>();
        }

        public int MatrattId { get; set; }

        [DisplayName("Namn")]
        [Required(ErrorMessage = "Namn är obligatoriskt!")]
        [StringLength(50, ErrorMessage = "Namn får vara max 50 tecken långt")]
        public string MatrattNamn { get; set; }
        public string Beskrivning { get; set; }

        [Required(ErrorMessage = "Pris är obligatoriskt!")]
        public int Pris { get; set; }

        
        [Required(ErrorMessage = "Maträttstyp är obligatoriskt!")]
        public int MatrattTypId { get; set; }

        [DisplayName("Maträttstyp")]
        public virtual MatrattTyp MatrattTyp { get; set; }
        public virtual ICollection<BestallningMatratt> BestallningMatratts { get; set; }
        public virtual ICollection<MatrattProdukt> MatrattProdukts { get; set; }
    }
}
