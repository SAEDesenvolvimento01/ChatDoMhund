using HelperMhundCore31.Data.Entity.Models;

namespace ChatDoMhund.Models.Poco
{
	public class PkUsuarioConversa
	{
		public int Codigo { get; set; }
		public string Nome { get; set; }
		public byte[] Foto { get; set; }
		public string Status { get; set; }
		public string Tipo { get; set; }
		public int CodigoDoCliente { get; set; }
		public string TipoParaExibicao { get; set; }

		public bool MensagemEhDesteUsuario(ChatProfess chatProfess)
		{
			return this.Codigo == chatProfess.IdOrigem && this.Tipo == chatProfess.TipoOrigem ||
			       this.Codigo == chatProfess.IdDestino && this.Tipo == chatProfess.TipoDestino;
		}
	}
}