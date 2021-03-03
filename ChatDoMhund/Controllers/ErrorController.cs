using ChatDoMhund.Controllers.Abstract;
using ChatDoMhund.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatDoMhund.Controllers
{
    [AllowAnonymous]
    public class ErrorController : AbsController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ErrorController(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public IActionResult HandleError(int code = 500, string error = "", string message = "")
        {
            ErrorViewModel viewModel = new ErrorViewModel(code, error, message, this._httpContextAccessor);

            if (this.Response != null)
            {
                this.Response.StatusCode = viewModel.StatusCode;
            }

            return this.View("Index", viewModel);
        }
    }
}