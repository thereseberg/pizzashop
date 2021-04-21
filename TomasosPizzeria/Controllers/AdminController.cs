using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosPizzeria.IdentityData;
using TomasosPizzeria.Models;
using TomasosPizzeria.ViewModels;

namespace TomasosPizzeria.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _usermanager;
        private TomasosContext _context;


        public AdminController(RoleManager<IdentityRole> rolemanager, UserManager<ApplicationUser> usermanager, TomasosContext context)
        {
            _roleManager = rolemanager;
            _usermanager = usermanager;
            _context = context;
        }

        public IActionResult AdminMain()
        {
            //var user = HttpContext.User;
            
            return View();
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.Name
                };

                IdentityResult result = await _roleManager.CreateAsync(identityRole);
                if(result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public IActionResult ManageCustomers()
        {
            var model = _usermanager.Users.Where(u => u.UserName != "Admin");

            return View(model);
        }

        public IActionResult ViewCustomer(string id)
        {
            //Eventuellt ta bort denna rad, lägg in direkt i model.EditUser
            var user = _usermanager.Users.SingleOrDefault(u => u.Id == id);

            var model = new ManageCustomerPartialViewModel();
            model.EditUser = user;
            model.Roles = _usermanager.GetRolesAsync(user).Result; //Om första raden tas bort, lägg in model.EditUser här!

            if (model.Roles[0] == "RegularUser")
            {
                ViewBag.RoleChange = "Uppgradera till PremiumUser";
            }
            else if (model.Roles[0] == "PremiumUser")
            {
                ViewBag.RoleChange = "Nedgradera till RegularUser";
            }

            return PartialView("_ManageCustomerPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(ManageCustomerPartialViewModel model)
        {
            model.EditUser = _usermanager.Users.SingleOrDefault(u => u.Id == model.EditUser.Id);
            string roleName = model.Roles[0];

            if (roleName == "RegularUser")
            {
                var result = await _usermanager.RemoveFromRoleAsync(model.EditUser, "RegularUser");
                var result2 = await _usermanager.AddToRoleAsync(model.EditUser, "PremiumUser");
            }
            else if (roleName == "PremiumUser")
            {
                var result = await _usermanager.RemoveFromRoleAsync(model.EditUser, "PremiumUser");
                var result2 = await _usermanager.AddToRoleAsync(model.EditUser, "RegularUser");
            }

            return RedirectToAction("ManageCustomers");
        }

        public IActionResult ManageFood()
        {
            ManageFoodViewModel model = new ManageFoodViewModel();
            model.matratter = _context.Matratts.Include(m => m.MatrattTyp).Include(m => m.MatrattProdukts).ThenInclude(m => m.Produkt).OrderBy(m => m.MatrattTyp.Beskrivning).ToList();
            model.matrattsTyper = _context.MatrattTyps.ToList();
            model.ingredients = _context.Produkts.ToList();
            model.SelectListMatrattTyper = new List<SelectListItem>();
            foreach (var typ in _context.MatrattTyps)
            {
                model.SelectListMatrattTyper.Add(new SelectListItem(typ.Beskrivning, typ.MatrattTypId.ToString()));
            }
            
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFood(ManageFoodViewModel model)
        {
            var foodAldreadyExist = _context.Matratts.Where(m => m.MatrattNamn == model.CurrentMatratt.MatrattNamn).ToList();

            if (ModelState.IsValid)
            {           
                if (foodAldreadyExist.Count == 0)
                {
                    _context.Add(model.CurrentMatratt);
                    _context.SaveChanges();
                    TempData["AddFood"] = "Ny maträtt tillagd";

                    var selectedIngredients = model.ingredients.Where(i => i.IsChecked == true).ToList();
                    var newFoodId = _context.Matratts.SingleOrDefault(m => m.MatrattNamn == model.CurrentMatratt.MatrattNamn).MatrattId;

                    foreach (var ingredient in selectedIngredients)
                    {
                        MatrattProdukt newItem = new MatrattProdukt();
                        newItem.MatrattId = newFoodId;
                        newItem.ProduktId = ingredient.ProduktId;
                        _context.Add(newItem);
                    }

                    _context.SaveChanges();

                    return RedirectToAction("ManageFood");
                }
                else
                {
                    TempData["AddFood"] = "Maträtt kunde inte läggas till. Namn på maträtt upptaget?";
                    return RedirectToAction("ManageFood");
                }
            }

            return RedirectToAction("ManageFood");
        }

        public IActionResult ViewFood(int id)
        {
            ManageFoodViewModel model = new ManageFoodViewModel();
            model.CurrentMatratt = _context.Matratts.Include(m => m.MatrattTyp).Include(m => m.MatrattProdukts).ThenInclude(m => m.Produkt).OrderBy(m => m.Beskrivning).Where(m => m.MatrattId == id).SingleOrDefault();
            //model.matrattsTyper = _context.MatrattTyps.ToList(); Behövs denna ens? 

            model.SelectListMatrattTyper = new List<SelectListItem>();
            foreach (var typ in _context.MatrattTyps)
            {
                model.SelectListMatrattTyper.Add(new SelectListItem(typ.Beskrivning, typ.MatrattTypId.ToString()));
            }

            model.ingredients = _context.Produkts.ToList(); //Alla ingredienser som finns 

            var ingredienser = _context.MatrattProdukts.Where(m => m.MatrattId == model.CurrentMatratt.MatrattId).Select(y => y.ProduktId).ToList();

            foreach (var item in model.ingredients)
            {
                if (ingredienser.Contains(item.ProduktId))
                {
                    item.IsChecked = true;
                }
            }

            return PartialView("_ManageFoodPartial", model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateFood(ManageFoodViewModel model)
        {
            if(ModelState.IsValid)
            {
                var original = _context.Matratts.SingleOrDefault(m => m.MatrattId == model.CurrentMatratt.MatrattId);
                _context.Entry(original).CurrentValues.SetValues(model.CurrentMatratt);
                _context.SaveChanges();

                var originalIngredients = _context.MatrattProdukts.Where(m => m.MatrattId == model.CurrentMatratt.MatrattId).Select(y => y.ProduktId).ToList();

                foreach (var item in model.ingredients)
                {
                    if (item.IsChecked && !originalIngredients.Contains(item.ProduktId))
                    {
                        MatrattProdukt newItem = new MatrattProdukt(original.MatrattId, item.ProduktId);
                        _context.Add(newItem);
                    }
                    else if (item.IsChecked == false && originalIngredients.Contains(item.ProduktId))
                    {
                        MatrattProdukt newItem = new MatrattProdukt(original.MatrattId, item.ProduktId);
                        _context.MatrattProdukts.Remove(newItem);
                    }
                }
                TempData["EditFood"] = "Maträtt uppdaterad";
                _context.SaveChanges();
            }
            else
            {
                TempData["EditFood"] = "Maträtt kunde inte uppdateras. Har du inte fyllt i alla uppgifter?";
            }

            return RedirectToAction("ManageFood");
        }

        [HttpPost]
        public IActionResult DeleteFood(int id)
        {

            var matratt = _context.Matratts.Include(m => m.BestallningMatratts).Include(m => m.MatrattProdukts).SingleOrDefault(m => m.MatrattId == id);
            if (matratt.BestallningMatratts.Count == 0)
            {
                foreach (var item in matratt.MatrattProdukts)
                {
                    //_context.MatrattProdukts.Remove(item);
                    _context.Remove(item);
                }

                _context.Remove(matratt);
                _context.SaveChanges();
                TempData["DeleteFood"] = "Maträtt borttagen";
            }
            else
            {
                TempData["DeleteFood"] = "Maträtt kunde inte tas bort. Finns den med på någon beställning?";
            }
                        
            return RedirectToAction("ManageFood");
        }

        public IActionResult ManageIngredients()
        {
            ManageIngredientsViewModel model = new ManageIngredientsViewModel();
            model.ListProducts = _context.Produkts.ToList();
            return View(model);
        }

        public IActionResult ViewIngredient(int id)
        {
            var model = _context.Produkts.SingleOrDefault(p => p.ProduktId == id);
            return PartialView("_ManageIngredientsPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddIngredient(ManageIngredientsViewModel model)
        {
            var ingredientAlreadyExist = _context.Produkts.Where(p => p.ProduktNamn == model.NewProduct.ProduktNamn).ToList();
            if (ModelState.IsValid)
            {
            
                if (ingredientAlreadyExist.Count == 0)
                {
                    _context.Produkts.Add(model.NewProduct);
                    _context.SaveChanges();
                    ViewBag.AddIngredientMessage = "Ny ingrediens tillagd";
                    return RedirectToAction("ManageIngredients");
                }
                else
                {
                    ViewBag.AddIngredientMessage = "Ingrediens kunde inte läggas till. Finns den redan?";
                    ManageIngredientsViewModel returnModel = new ManageIngredientsViewModel();
                    returnModel.ListProducts = _context.Produkts.ToList();
                    return View("ManageIngredients", returnModel);
                }
            }

            return RedirectToAction("ManageIngredients");
        }

        public IActionResult UpdateIngredient(Produkt updatedProduct)
        {
            if (ModelState.IsValid)
            {
                var original = _context.Produkts.SingleOrDefault(p => p.ProduktId == updatedProduct.ProduktId);
                _context.Entry(original).CurrentValues.SetValues(updatedProduct);
                _context.SaveChanges();
                TempData["EditIngredient"] = "Ingrediens har uppdaterats";

                return RedirectToAction("ManageIngredients");
            }
            else
            {
                TempData["EditIngredient"] = "Ingrediens kunde inte uppdateras, har du fyllt i namn på ingrediens?";
                return RedirectToAction("ManageIngredients");
            }

            

        }

        [HttpPost]
        public IActionResult DeleteIngredient(int id)
        {
            var ingredient = _context.Produkts.Include(i => i.MatrattProdukts).SingleOrDefault(p => p.ProduktId == id);
            if (ingredient.MatrattProdukts.Count == 0)
            {
                _context.Produkts.Remove(ingredient);
                _context.SaveChanges();
                TempData["Edit"] = "Ingrediens har tagits bort";
            }
            else
            {
                TempData["EditIngredient"] = "Ingrediens kunde inte tas bort. Finns den på en maträtt?";
            }

            return RedirectToAction("ManageIngredients");
        }

        public IActionResult ManageOrders()
        {
            var model = _context.AspNetUsers.Include(u => u.Bestallnings).ToList();
            return View(model);
        }

        public IActionResult UpdateOrderStatus(int id)
        {
            Bestallning currentOrder = _context.Bestallnings.SingleOrDefault(o => o.BestallningId == id);
            if (currentOrder.Levererad)
            {
                currentOrder.Levererad = false;
            }
            else
            {
                currentOrder.Levererad = true;
            }

            _context.Update(currentOrder); //Uppdaterar orderstatus i databas
            _context.SaveChanges();

            return RedirectToAction("ManageOrders");
        }

        public IActionResult DeleteOrder(int id)
        {
            var currentOrder = _context.Bestallnings.Include(o => o.BestallningMatratts).SingleOrDefault(o => o.BestallningId == id);

            foreach (var item in currentOrder.BestallningMatratts)
            {
                _context.Remove(item);
            }

            _context.Remove(currentOrder);
            _context.SaveChanges();
            TempData["RemoveOrder"] = "Beställning borttagen";

            return RedirectToAction("ManageOrders");
        }

    }
}
