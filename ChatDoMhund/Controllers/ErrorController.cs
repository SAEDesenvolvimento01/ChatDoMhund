using ChatDoMhund.Controllers.Abstract;
using ChatDoMhund.Models.Enum;
using ChatDoMhund.Models.ViewModels;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatDoMhund.Controllers
{
    [AllowAnonymous]
    public class ErrorController : AbsController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISaeHelperCookie _saeHelperCookie;

        public ErrorController(IHttpContextAccessor httpContextAccessor,
	        ISaeHelperCookie saeHelperCookie)
        {
	        this._httpContextAccessor = httpContextAccessor;
	        this._saeHelperCookie = saeHelperCookie;
        }

        public IActionResult HandleError(int code = 500, string error = "", string message = "")
        {
	        string origem = this._saeHelperCookie.GetCookie(EChatCookie.OrigemDeChat.ToString());

            ErrorViewModel viewModel = new ErrorViewModel(code, error, message, this._httpContextAccessor, origem);

            if (this.Response != null)
            {
                this.Response.StatusCode = viewModel.StatusCode;
            }

            return this.View("Index", viewModel);
        }
    }
}