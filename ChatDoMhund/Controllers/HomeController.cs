using ChatDoMhund.Controllers.Abstract;
using ChatDoMhund.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChatDoMhund.Controllers
{
    public class HomeController : AbsController
    {
        public HomeController()
        {
            
        }

        public IActionResult Index()
        {
            return this.View("Index", new HomeViewModel());
        }
    }
}
