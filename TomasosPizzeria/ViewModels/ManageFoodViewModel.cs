using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TomasosPizzeria.Models;

namespace TomasosPizzeria.ViewModels
{
    public class ManageFoodViewModel
    {
        public List<Matratt> matratter { get; set; }
        public List<MatrattTyp> matrattsTyper { get; set; }

        [DisplayName("Maträttstyp")]
        public List<SelectListItem> SelectListMatrattTyper{ get; set; }

        public Matratt CurrentMatratt { get; set; }
        public List<Produkt> ingredients { get; set; }

    }
}
