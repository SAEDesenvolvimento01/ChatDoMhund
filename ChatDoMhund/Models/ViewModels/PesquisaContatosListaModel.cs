using ChatDoMhund.Models.Poco;
using System.Collections.Generic;

namespace ChatDoMhund.Models.ViewModels
{
	public class PesquisaContatosListaModel
	{
		public List<PkUsuarioConversa> Usuarios { get; }
		public bool EhProfessorOuCoordenador { get; set; }

		public PesquisaContatosListaModel(List<PkUsuarioConversa> usuarios, bool ehProfessorOuCoordenador)
		{
			this.Usuarios = usuarios;
			this.EhProfessorOuCoordenador = ehProfessorOuCoordenador;
		}
	}
}