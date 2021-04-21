using System;
using System.Collections.Generic;

#nullable disable

namespace TomasosPizzeria.Models
{
    public partial class BestallningMatratt
    {
        public int MatrattId { get; set; }
        public int BestallningId { get; set; }
        public int Antal { get; set; }

        public virtual Bestallning Bestallning { get; set; }
        public virtual Matratt Matratt { get; set; }

        public BestallningMatratt()
        {

        }

        public BestallningMatratt(int bestallningId, int matrattId, int antal)
        {
            MatrattId = matrattId;
            BestallningId = bestallningId;
            Antal = antal;
        }
    }
}
