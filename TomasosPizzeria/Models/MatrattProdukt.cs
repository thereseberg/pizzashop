using System;
using System.Collections.Generic;

#nullable disable

namespace TomasosPizzeria.Models
{
    public partial class MatrattProdukt
    {
        public int MatrattId { get; set; }
        public int ProduktId { get; set; }

        public MatrattProdukt()
        {

        }

        public MatrattProdukt(int matrattId, int produktId)
        {
            MatrattId = matrattId;
            ProduktId = produktId;
        }

        public virtual Matratt Matratt { get; set; }
        public virtual Produkt Produkt { get; set; }
    }
}
