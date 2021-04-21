using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosPizzeria.IdentityData;

namespace TomasosPizzeria.ViewModels
{
    public class ManageCustomerPartialViewModel
    {
        public ApplicationUser EditUser { get; set; }

        public IList<string> Roles { get; set; }
    }
}
