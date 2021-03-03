using System.Collections.Generic;
using JetBrains.Annotations;

namespace ChatDoMhund.Models.Poco
{
    public class PkItemMenuPrincipal
    {
        public string Texto { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool EhActive { get; set; }
        public bool PossuiPermissaoParaAcessar { get; set; }
        public string Icone { get; set; }
        public List<PkSubItemMenuPrincipal> SubItens { get; set; }

        public PkItemMenuPrincipal()
        {
            this.SubItens = new List<PkSubItemMenuPrincipal>();
        }

        public PkItemMenuPrincipal([AspMvcAction] string action, [AspMvcController] string controller)
        {
            this.SubItens = new List<PkSubItemMenuPrincipal>();
            this.Action = action;
            this.Controller = controller;
        }

        public string GetClasseActive()
        {
            if (this.EhActive)
            {
                return "active";
            }

            return string.Empty;
        }
    }
}