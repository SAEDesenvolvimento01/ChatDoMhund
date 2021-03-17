using ChatDoMhund.Models.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperSaeStandard11.Handlers;
using System;

namespace ChatDoMhund.Models.Poco
{
	public class PkMensagem
	{
		public int Id { get; set; }
		public string GroupNameOrigem { get; set; }
		public string GroupNameDestino { get; set; }
		public string Texto { get; set; }
		public DateTime DataDaMensagem { get; set; }
		public bool Lida { get; set; }
		public DateTime? DataDeLeitura { get; set; }

		public PkMensagem()
		{

		}

		public PkMensagem(ChatProfess mensagem, GroupBuilder groupBuilder, int codigoDoCliente)
		{
			this.Id = mensagem.Id;
			this.GroupNameOrigem = groupBuilder.BuildGroupName(codigoDoCliente, mensagem.TipoOrigem, mensagem.IdOrigem.ConvertToInt32());
			this.GroupNameDestino = groupBuilder.BuildGroupName(codigoDoCliente, mensagem.TipoDestino, mensagem.IdDestino.ConvertToInt32());
			this.Texto = mensagem.TextMens;
			this.DataDaMensagem = mensagem.DtMensagem ?? DateTime.MinValue;
			this.Lida = mensagem.Lido ?? false;
			this.DataDeLeitura = mensagem.DtQleu;
		}

		public override string ToString()
		{
			return $"{this.GroupNameOrigem} => {this.GroupNameDestino}: {this.Texto}";
		}
	}
}