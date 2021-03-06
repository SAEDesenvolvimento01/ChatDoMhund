﻿using System;
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
		public DateTime UltimaVezOnline { get; set; }
		public string TipoDeProfessor { get; set; }

		public override string ToString()
		{
			return $"{this.Codigo} - {this.Nome} - {this.Status}";
		}

		public bool MensagemEhDesteUsuario(ChatProfess chatProfess)
		{
			return this.Codigo == chatProfess.IdOrigem && this.Tipo == chatProfess.TipoOrigem ||
			       this.Codigo == chatProfess.IdDestino && this.Tipo == chatProfess.TipoDestino;
		}
	}
}