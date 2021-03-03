using ChatDoMhund.Models.Infra;

namespace ChatDoMhund.Models.ViewModels.Base
{
    public abstract class BaseViewModel
    {
        public VoceSabiaRoutes Routes { get; set; }

        public BaseViewModel()
        {
            this.Routes = new VoceSabiaRoutes();
        }
    }
}