using System;
using ChatDoMhund.Models.Tratamento;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using System.Collections.Generic;
using System.Linq;
using HelperSaeStandard11.Models.Extension;

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
		public string DataDaUltimaMensagem { get; set; }
		public List<PkMensagem> Mensagens { get; set; }

		public PkConversa()
		{

		}

		public PkConversa(PkUsuarioConversa usuario, List<ChatProfess> mensagensDoUsuario, int codigoDaEscola, GroupBuilder groupBuilder)
		{
			this.Codigo = usuario.Codigo;
			this.Nome = usuario.Nome.GetPrimeiroEUltimoNome();
			this.Foto = FotoTrata.ToBase64String(usuario.Foto);
			this.Status = usuario.Status;
			this.Tipo = usuario.Tipo;
			if (this.Status == this.Tipo)
			{
				this.Status = TipoDeUsuarioDoChatTrata.TipoExtenso(this.Tipo);
			}

			this.CodigoDaEscola = codigoDaEscola;
			this.GroupName = groupBuilder.BuildGroupName(codigoDaEscola, usuario.Tipo, usuario.Codigo);
			this.Mensagens = mensagensDoUsuario.Select(mensagem => new PkMensagem(mensagem, groupBuilder, codigoDaEscola)).ToList();

			this.SetDataDaUltimaMensagem();
		}

		public override string ToString()
		{
			return $"{this.CodigoDaEscola} - {this.Codigo} - {this.Nome}";
		}

		private void SetDataDaUltimaMensagem()
		{
			if (this.Mensagens.Any())
			{
				DateTime data = this.GetDataDaUltimaMensagem();
				DateTime hoje = DateTime.Today;
				DateTime ontem = DateTime.Today.AddDays(-1);
				DateTime umaSemanaAtras = DateTime.Today.AddDays(-7);
				if (data.Date == hoje)
				{
					this.DataDaUltimaMensagem = data.ToString("HH:mm");
				}
				else if (data.Date == ontem)
				{
					this.DataDaUltimaMensagem = data.ToString("'ontem', HH:mm");
				}
				else if (data > umaSemanaAtras)
				{
					this.DataDaUltimaMensagem = data.ToString("dddd");
				}
				else if (data.Year == hoje.Year)
				{
					this.DataDaUltimaMensagem = data.ToString("dd/MM");
				}
				else
				{
					this.DataDaUltimaMensagem = data.ToString("dd/MM/yyyy");
				}
			}
		}

		public DateTime GetDataDaUltimaMensagem()
		{
			return this.Mensagens.OrderByDescending(x => x.DataDaMensagem).FirstOrDefault()?.DataDaMensagem ?? DateTime.MinValue;
		}
	}
}
