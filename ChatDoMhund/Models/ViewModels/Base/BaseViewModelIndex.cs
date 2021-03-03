using ChatDoMhund.Models.Infra;
using ChatDoMhund.Models.Poco;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace ChatDoMhund.Models.ViewModels.Base
{
    public abstract class BaseViewModelIndex : BaseViewModel
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public string CorPrimaria { get; set; }

        public List<PkItemMenuPrincipal> ItensDoMenuPrincipal { get; set; }

        public BaseViewModelIndex()
        {
            this.Title = "Chat";
            this.CorPrimaria = "#64b5f6 blue lighten-2";
        }

        public void SetItensDoMenuPrincipal(HttpContext httpContext, UsuarioLogado usuarioLogado)
        {
            this.ItensDoMenuPrincipal = new List<PkItemMenuPrincipal>();
            if (httpContext != null && usuarioLogado.EstaLogado())
            {
                List<PkItemMenuPrincipal> list = new List<PkItemMenuPrincipal>
                    {
                        new PkItemMenuPrincipal
                        {
                            Texto = "Cadastros",
                            Icone = "add_to_queue",
                            PossuiPermissaoParaAcessar = true,
                            SubItens = new List<PkSubItemMenuPrincipal>
                            {
                                new PkSubItemMenuPrincipal("Index", "Novidades")
                                {
                                    Texto = "Novidades",
                                    PossuiPermissaoParaAcessar = true
                                }
                            }
                        }
                    };

                this.ItensDoMenuPrincipal.AddRange(list);

                this.SetClassesDosItensDoMenu(httpContext);

                this.ItensDoMenuPrincipal = this.ItensDoMenuPrincipal
                    .OrderBy(item => item.Texto)
                    .ToList();

                this.ItensDoMenuPrincipal
                   .ForEach(item =>
                   {
                       item.SubItens = item.SubItens.OrderBy(subItem => subItem.Texto).ToList();
                   });
            }
        }

        private void SetClassesDosItensDoMenu(HttpContext httpContext)
        {
            this.ItensDoMenuPrincipal.ForEach(item =>
            {
                item.SubItens.ForEach(subItem =>
                {
                    subItem.EhActive = this.EhActive(subItem.Controller, subItem.Action, httpContext);
                });

                item.EhActive = item.SubItens.Any(subItem => subItem.EhActive) ||
                                this.EhActive(item.Controller, item.Action, httpContext);
            });
        }

        private bool EhActive(string controller, string action, HttpContext httpContext)
        {
            if (!string.IsNullOrEmpty(controller) &&
                !string.IsNullOrEmpty(action) &&
                httpContext != null &&
                (httpContext.Request.RouteValues?.Any() ?? false) &&
                httpContext.Request.RouteValues.TryGetValue("controller", out object requestController) &&
                httpContext.Request.RouteValues.TryGetValue("action", out object requestAction))
            {
                if (requestController?.ToString() == controller && requestAction?.ToString() == action)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
