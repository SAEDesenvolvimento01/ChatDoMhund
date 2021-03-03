namespace ChatDoMhund.Models.Infra
{
    public class VoceSabiaRoutes
    {
        public string Helper { get; set; }

        public VoceSabiaRoutes()
        {
            if (/*false*/Util.EhDebug())
            {
                this.Helper = "http://localhost:52243";
            }
            else
            {
                this.Helper = "http://helper.mhund.com.br";
            }
        }
    }
}
