using System;
using System.Collections.Generic;

#nullable disable

namespace TomasosPizzeria.Models
{
    public partial class MatrattTyp
    {
        public MatrattTyp()
        {
            Matratts = new HashSet<Matratt>();
        }

        public int MatrattTypId { get; set; }
        public string Beskrivning { get; set; }

        public virtual ICollection<Matratt> Matratts { get; set; }
    }
}
