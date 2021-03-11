using ChatDoMhund.Models.Poco;
using System.Collections.Generic;

namespace ChatDoMhund.Models.ViewModels
{
	public class PesquisaContatosIndexModel
	{
		public List<PkUsuarioConversa> Usuarios { get; }
		public string TituloDaModal { get; set; }

		public PesquisaContatosIndexModel(List<PkUsuarioConversa> usuarios)
		{
			this.Usuarios = usuarios;
			this.TituloDaModal = "Contatos";
		}
	}
}
