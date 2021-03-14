using System;
using ChatDoMhund.Models.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperSaeStandard11.Handlers;

namespace ChatDoMhund.Models.Poco
{
	public class PkMensagem
	{
		public string GroupNameOrigem { get; set; }
		public string GroupNameDestino { get; set; }
		public string Texto { get; set; }
		public DateTime DataDaMensagem { get; set; }

		public PkMensagem()
		{

		}

		public PkMensagem(ChatProfess mensagem, GroupBuilder groupBuilder, int codigoDoCliente)
		{
			this.GroupNameOrigem = groupBuilder.BuildGroupName(codigoDoCliente, mensagem.TipoOrigem, mensagem.IdOrigem.ConvertToInt32());
			this.GroupNameDestino = groupBuilder.BuildGroupName(codigoDoCliente, mensagem.TipoDestino, mensagem.IdDestino.ConvertToInt32());
			this.Texto = mensagem.TextMens;
			this.DataDaMensagem = mensagem.DtMensagem ?? DateTime.MinValue;
		}

		public override string ToString()
		{
			return $"{this.GroupNameOrigem} => {this.GroupNameDestino}: {this.Texto}";
		}
	}
}