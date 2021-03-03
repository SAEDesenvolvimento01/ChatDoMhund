using ChatDoMhund.Models.ViewModels.Base;

namespace ChatDoMhund.Models.ViewModels
{
    public class LoginViewModel : BaseViewModelIndex
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
