using System.Collections.Generic;

namespace ChatDoMhund.Models.ViewModels.Base
{
    public abstract class BaseViewModelTabela<T> : BaseViewModel
    {
        public List<T> Itens { get; set; }

        public BaseViewModelTabela()
        {
            this.Itens = new List<T>();
        }
    }
}