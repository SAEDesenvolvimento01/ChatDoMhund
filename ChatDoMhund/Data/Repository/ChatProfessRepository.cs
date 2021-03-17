using System;
using ChatDoMhund.Data.Repository.Abstract;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using ChatDoMhund.Models.Poco;

namespace ChatDoMhund.Data.Repository
{
	public class ChatProfessRepository : RepositoryBase<ChatProfess>
	{
		public ChatProfessRepository(MhundDbContext db) : base(db)
		{
		}

		public List<ChatProfess> GetMensagensDoUsuario(
			int codigoDoUsuario,
			string tipoDoUsuario)
		{
			List<ChatProfess> mensagens = this
				._db
				.ChatProfess
				.Where(x =>
					(x.IdDestino == codigoDoUsuario && x.TipoDestino == tipoDoUsuario) ||
					(x.IdOrigem == codigoDoUsuario && x.TipoOrigem == tipoDoUsuario))
				.OrderBy(x => x.DtMensagem)
				.ToList();
			return mensagens;
		}

		public SaeResponseRepository<List<ChatProfess>> GetMensagens(
			int codigoDoUsuario1,
			string tipoDoUsuario1,
			int codigoDoUsuario2,
			string tipoDoUsuario2)
		{
			List<ChatProfess> lista = this
				.GetMensagensAsQueryable(
					codigoDoUsuario1,
					tipoDoUsuario1,
					codigoDoUsuario2,
					tipoDoUsuario2).ToList();

			return new SaeResponseRepository<List<ChatProfess>>(lista?.Any() ?? false, lista);
		}

		private IQueryable<ChatProfess> GetMensagensAsQueryable(int codigoDoUsuario1, string tipoDoUsuario1, int codigoDoUsuario2, string tipoDoUsuario2)
		{
			return (
				from mensagem in this._db.ChatProfess.Where(x =>
					(x.IdOrigem == codigoDoUsuario2 &&
					 x.TipoOrigem == tipoDoUsuario2 &&
					 x.IdDestino == codigoDoUsuario1 &&
					 x.TipoDestino == tipoDoUsuario1) ||
					(x.IdOrigem == codigoDoUsuario1 &&
					 x.TipoOrigem == tipoDoUsuario1 &&
					 x.IdDestino == codigoDoUsuario2 &&
					 x.TipoDestino == tipoDoUsuario2))
				select mensagem
			).AsQueryable();
		}

		public SaeResponseRepository<List<PkMensagemLida>> LerMensagens(
			int codigoDoUsuarioOrigem,
			string tipoDeUsuarioOrigem,
			int codigoDoUsuarioDestino,
			string tipoDeUsuarioDestino)
		{
			List<ChatProfess> mensagens = this
				._db
				.ChatProfess
				.Where(mensagem => mensagem.IdOrigem == codigoDoUsuarioOrigem &&
								 mensagem.TipoOrigem == tipoDeUsuarioOrigem &&
								 mensagem.IdDestino == codigoDoUsuarioDestino &&
								 mensagem.TipoDestino == tipoDeUsuarioDestino &&
								 (!mensagem.Lido ?? false)).ToList();


			mensagens.ForEach(mensagem =>
			{
				mensagem.Lido = true;
				mensagem.DtQleu = DateTime.Now;
				this._db.Entry(mensagem).State = EntityState.Modified;
			});

			bool status = this._db.SaveChanges() > 0;

			return new SaeResponseRepository<List<PkMensagemLida>>(/*status*/true, mensagens
				.Select(x => new PkMensagemLida
				{
					Id = x.Id,
					DataDeLeitura = x.DtQleu ?? DateTime.MinValue
				})
				.ToList());
		}
	}
}