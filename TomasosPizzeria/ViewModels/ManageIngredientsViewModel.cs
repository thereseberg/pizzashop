using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosPizzeria.Models;

namespace TomasosPizzeria.ViewModels
{
    public class ManageIngredientsViewModel
    {
        public List<Produkt> ListProducts { get; set; }
        public Produkt NewProduct { get; set; }
    }
}
