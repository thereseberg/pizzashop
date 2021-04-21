using System;
using System.Collections.Generic;

#nullable disable

namespace TomasosPizzeria.Models
{
    public partial class Bestallning
    {
        public Bestallning()
        {
            BestallningMatratts = new HashSet<BestallningMatratt>();
        }

        public int BestallningId { get; set; }
        public DateTime BestallningDatum { get; set; }
        public int Totalbelopp { get; set; }
        public bool Levererad { get; set; }
        public string Id { get; set; }

        public virtual AspNetUser IdNavigation { get; set; }
        public virtual ICollection<BestallningMatratt> BestallningMatratts { get; set; }
    }
}
