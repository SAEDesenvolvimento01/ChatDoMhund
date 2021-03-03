using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace ChatDoMhund.Models.Infra.Filter
{
    public class LoginFilterAttribute : ActionFilterAttribute
    {
        private readonly UsuarioLogado _usuarioLogado;
        private readonly Type FiltroAnonimo;

        public LoginFilterAttribute(UsuarioLogado usuarioLogado)
        {
            this._usuarioLogado = usuarioLogado;
            this.FiltroAnonimo = typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute);
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!this.AceitaUsuarioNaoLogado(filterContext))
            {
                this._usuarioLogado.GetUsuarioLogado();

                if (!this._usuarioLogado.EstaLogado() && !this._usuarioLogado.ConseguiuRelogar())
                {
                    filterContext.Result =
                        new RedirectToRouteResult(
                            new RouteValueDictionary(
                                new
                                {
                                    Controller = "Error", 
                                    Action = "HandleError",
                                    Area = "", 
                                    code = "403",
                                    message = "Sua sessão expirou."
                                }
                            )
                        );
                }
            }
        }

        private bool AceitaUsuarioNaoLogado(ActionExecutingContext filterContext)
        {
            return filterContext
                .ActionDescriptor
                .EndpointMetadata
                .Any(filter => filter.GetType() == FiltroAnonimo);
        }
    }
}
