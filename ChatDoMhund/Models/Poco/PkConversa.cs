using ChatDoMhund.Models.Tratamento;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using System.Collections.Generic;
using System.Linq;

namespace ChatDoMhund.Models.Poco
{
	public class PkConversa
	{
		public int Codigo { get; set; }
		public string Nome { get; set; }
		public string Foto { get; set; }
		public string Status { get; set; }
		public string Tipo { get; set; }
		public int CodigoDaEscola { get; set; }
		public string GroupName { get; set; }
		public List<PkMensagem> Mensagens { get; set; }

		public PkConversa()
		{

		}

		public PkConversa(PkUsuarioConversa usuario, List<ChatProfess> mensagensDoUsuario, int codigoDaEscola, GroupBuilder groupBuilder)
		{
			this.Codigo = usuario.Codigo;
			this.Nome = usuario.Nome;
			this.Foto = FotoTrata.ToBase64String(usuario.Foto);
			this.Status = usuario.Status;
			this.Tipo = usuario.Tipo;
			this.CodigoDaEscola = codigoDaEscola;
			this.GroupName = groupBuilder.GetGroupName(codigoDaEscola, usuario.Tipo, usuario.Codigo);
			this.Mensagens = mensagensDoUsuario.Select(mensagem => new PkMensagem(mensagem, groupBuilder, codigoDaEscola)).ToList();
		}

		public override string ToString()
		{
			return $"{this.CodigoDaEscola} - {this.Codigo} - {this.Nome}";
		}
	}
}
