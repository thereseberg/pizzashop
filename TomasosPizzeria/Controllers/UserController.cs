using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosPizzeria.IdentityData;
using TomasosPizzeria.Models;
using TomasosPizzeria.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace TomasosPizzeria.Controllers
{
    [Authorize(Roles = "RegularUser,PremiumUser")]
    public class UserController : Controller
    {
        private TomasosContext _context;
        private UserManager<ApplicationUser> _usermanager;

        public UserController(TomasosContext context, UserManager<ApplicationUser> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }

        public IActionResult UserMain()
        {
            string userName = HttpContext.User.Identity.Name;
            AspNetUser user = _context.AspNetUsers.SingleOrDefault(u => u.UserName == userName);

            return View(user);
        }

        public IActionResult UserMenu()
        {
            List<Matratt> allFood = _context.Matratts.Include(m => m.MatrattTyp).Include(m => m.MatrattProdukts).ThenInclude(m => m.Produkt).OrderBy(m => m.Beskrivning).ToList();
            UserViewModel model = new UserViewModel();
            model.Pizzas = allFood.Where(f => f.MatrattTyp.Beskrivning == "Pizza").ToList();
            model.Pastas = allFood.Where(f => f.MatrattTyp.Beskrivning == "Pasta").ToList();
            model.Sallads = allFood.Where(f => f.MatrattTyp.Beskrivning == "Sallad").ToList();
            return View(model);
        }

        public IActionResult UserInfo()
        {
            string userName = HttpContext.User.Identity.Name;
            //AspNetUser user = _context.AspNetUsers.SingleOrDefault(u => u.UserName == userName);

            AspNetUser aspUser = _context.AspNetUsers.SingleOrDefault(u => u.UserName == userName);

            User user = new User();
            user.AnvandarNamn = aspUser.UserName;
            user.Namn = aspUser.Namn;
            user.Email = aspUser.Email;
            user.Telefon = aspUser.PhoneNumber;
            user.Gatuadress = aspUser.Gatuadress;
            user.Postnr = aspUser.Postnr;
            user.Postort = aspUser.Postort;
            user.Losenord = "hiddenvalue";
            user.Points = (int)aspUser.Points;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateUserInfo(User user)
        {
            if (ModelState.IsValid)
            {
                AspNetUser aspUser = _context.AspNetUsers.SingleOrDefault(u => u.UserName == user.AnvandarNamn);
                aspUser.UserName = user.AnvandarNamn;
                aspUser.Namn = user.Namn;
                aspUser.Email = user.Email;
                aspUser.PhoneNumber = user.Telefon;
                aspUser.Gatuadress = user.Gatuadress;
                aspUser.Postnr = user.Postnr;
                aspUser.Postort = user.Postort;
                _context.Update(aspUser);
                _context.SaveChanges();
            }

            return View("UserInfo");
        }

        public IActionResult AddProduct(int id)
        {
            List<Matratt> foodcart;
            string jsonCart;

            if (HttpContext.Session.GetString("cart") == null)
            {
                //Ny tom varukorg
                foodcart = new List<Matratt>();
            }
            else
            {
                //Hämta de som finns i varukorgen
                jsonCart = HttpContext.Session.GetString("cart");

                //Konvertera dvs göra om till en lista med produkter. Jämför med JSON.parse men med .NET-kod istället
                foodcart = JsonConvert.DeserializeObject<List<Matratt>>(jsonCart);
            }

            Matratt food = _context.Matratts.SingleOrDefault(m => m.MatrattId == id);
            foodcart.Add(food);

            //Gör om vår produkt-lista till JSON. Jämför med JSON.stringify men med .NET-kod istället
            jsonCart = JsonConvert.SerializeObject(foodcart);

            //Lägg tillbaka den uppdaterade listan i sessionsvariabeln
            HttpContext.Session.SetString("cart", jsonCart);

            //Skicka vidare till översikten
            return PartialView("_CartOverviewPartial", foodcart);
        }
        public IActionResult RemoveProduct(int id)
        {
            List<Matratt> foodcart;
            string jsonCart;

            if (HttpContext.Session.GetString("cart") == null)
            {
                //Ny tom varukorg
                foodcart = new List<Matratt>();
            }
            else
            {
                //Hämta de som finns i varukorgen
                jsonCart = HttpContext.Session.GetString("cart");

                //Konvertera dvs göra om till en lista med produkter. Jämför med JSON.parse men med .NET-kod istället
                foodcart = JsonConvert.DeserializeObject<List<Matratt>>(jsonCart);
            }

            Matratt foodToRemove = foodcart.Where(f => f.MatrattId == id).First();
            foodcart.Remove(foodToRemove);

            //Gör om vår produkt-lista till JSON. Jämför med JSON.stringify men med .NET-kod istället
            jsonCart = JsonConvert.SerializeObject(foodcart);

            //Lägg tillbaka den uppdaterade listan i sessionsvariabeln
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("UserCheckout");
        }

        public IActionResult UserCheckout()
        {
            List<Matratt> foodcart;
            string jsonCart;

            if (HttpContext.Session.GetString("cart") == null)
            {
                //Ny tom varukorg
                foodcart = new List<Matratt>();
            }
            else
            {
                //Hämta de som finns i varukorgen
                jsonCart = HttpContext.Session.GetString("cart");

                //Konvertera dvs göra om till en lista med produkter. Jämför med JSON.parse men med .NET-kod istället
                foodcart = JsonConvert.DeserializeObject<List<Matratt>>(jsonCart);
            }

            //Sorterar på pris för att kunna dra bort billigaste pizzan senare för premium
            foodcart.OrderByDescending(f => f.Pris); 

            Bestallning newOrder = new Bestallning();

            AspNetUser currentUser = _context.AspNetUsers.SingleOrDefault(u => u.UserName == HttpContext.User.Identity.Name);

            int sumAllFood;
            int originalSum = foodcart.Sum(p => p.Pris);
            //int sumAllFood = foodcart.Sum(p => p.Pris);

            if (User.IsInRole("PremiumUser"))
            {
                //Lägger till poängen från nuvarande beställning till total för premium
                currentUser.Points = (int)currentUser.Points + foodcart.Count * 10; 

                if (foodcart.Count > 2)
                {
                    if (currentUser.Points >= 100)
                    {
                        foodcart[foodcart.Count - 1].Pris = 0; // Priset på billigaste pizzan dras bort tack vare poäng
                        currentUser.Points -= 100;
                        
                    }

                    sumAllFood = CalculatePriceWithDiscount(foodcart.Sum(p => p.Pris));
                }
                else
                {
                    if (currentUser.Points > 100)
                    {
                        foodcart[foodcart.Count - 1].Pris = 0; // Priset på billigaste pizzan dras bort tack vare poäng                        
                        currentUser.Points -= 100;
                    }

                    sumAllFood = foodcart.Sum(p => p.Pris);
                }                            
            }
            else
            {
                sumAllFood = foodcart.Sum(p => p.Pris);
            }

            newOrder.Totalbelopp = sumAllFood;
            newOrder.BestallningDatum = DateTime.Now;
            newOrder.Id = currentUser.Id;
            newOrder.Levererad = false;

            UserViewModel model = new UserViewModel();
            model.CurrentOrderFood = foodcart;
            model.CurrentOrder = newOrder;
            model.CurrentUser = currentUser;
            model.Discount = originalSum - sumAllFood;

            //Gör om vår produkt-lista till JSON. Jämför med JSON.stringify men med .NET-kod istället
            jsonCart = JsonConvert.SerializeObject(foodcart);

            //Lägg tillbaka den uppdaterade listan i sessionsvariabeln
            HttpContext.Session.SetString("cart", jsonCart);

            return View(model);
        }

        [HttpPost]
        public IActionResult UserCheckout(UserViewModel model)
        {
            Bestallning newOrder = new Bestallning();
            newOrder.Totalbelopp = model.CurrentOrder.Totalbelopp;
            newOrder.BestallningDatum = DateTime.Now;
            newOrder.Id = model.CurrentOrder.Id;
            newOrder.Levererad = false;

            _context.Add(newOrder);
            _context.SaveChanges();

            int orderID = _context.Bestallnings.Where(b => b.Id == newOrder.Id).OrderByDescending(b => b.BestallningDatum).First().BestallningId;

            //Hämta de som finns i varukorgen
            var jsonCart = HttpContext.Session.GetString("cart");

            //Konvertera dvs göra om till en lista med produkter. Jämför med JSON.parse men med .NET-kod istället
            var foodcart = JsonConvert.DeserializeObject<List<Matratt>>(jsonCart);

            var orderedFoodCart = from f in foodcart
                                  group f by f.MatrattId into g
                                  select new { MatrattId = g.Key, Count = g.Count() };

            foreach (var item in orderedFoodCart)
            {
                BestallningMatratt newItem = new BestallningMatratt(orderID, item.MatrattId, item.Count);
                _context.Add(newItem);               
            }
            _context.SaveChanges();

            if (User.IsInRole("PremiumUser"))
            {
                AspNetUser user = _context.AspNetUsers.SingleOrDefault(u => u.Id == newOrder.Id);
                user.Points = model.CurrentUser.Points;
                _context.Update(user);
                _context.SaveChanges();
            }
           
            model.CurrentOrder.BestallningId = orderID;
            model.CurrentUser.Id = newOrder.Id;

            HttpContext.Session.Clear(); //För att inte gammal beställnings-data ska ligga kvar i session om man gör en ny beställning

            return View("UserNewOrderMessage", model);
        }

        private int CalculatePriceWithDiscount(int sum)
        {
            double calculation = sum * 0.8;
            int returnSum = (int)calculation;
            return returnSum;
        }

        public IActionResult UserOrders()
        {
            return View();
        }


    }
}
