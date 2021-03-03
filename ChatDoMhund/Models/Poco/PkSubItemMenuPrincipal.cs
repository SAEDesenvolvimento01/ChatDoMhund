using JetBrains.Annotations;

namespace ChatDoMhund.Models.Poco
{
    public class PkSubItemMenuPrincipal
    {
        public string Texto { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool PossuiPermissaoParaAcessar { get; set; }
        public bool EhActive { get; set; }

        public PkSubItemMenuPrincipal()
        {

        }

        public PkSubItemMenuPrincipal([AspMvcAction] string action, [AspMvcController] string controller)
        {
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