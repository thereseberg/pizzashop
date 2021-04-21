using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosPizzeria.Models;

namespace TomasosPizzeria.ViewModels
{
    public class UserViewModel
    {
        public AspNetUser CurrentUser { get; set; }
        public List<Bestallning> UserOrders { get; set; }
        public Bestallning CurrentOrder { get; set; }
        public List<Matratt> Pizzas { get; set; }
        public List<Matratt> Pastas { get; set; }
        public List<Matratt> Sallads { get; set; }
        public List<Matratt> CurrentOrderFood { get; set; }
        public int Discount { get; set; }

    }
}
