using ChatDoMhund.Controllers.Abstract;
using ChatDoMhund.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChatDoMhund.Controllers
{
    public class ChatController : AbsController
    {
        public IActionResult Index()
        {
            return this.View("Index", new ChatIndexViewModel());
        }
    }
}