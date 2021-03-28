using ChatDoMhund.Data.Repository.Abstract;
using ChatDoMhund.Models.Poco;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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
			//PR 123
			//AL 456 RE 789
			var pessoas = (from mensagem in this
						._db
						.ChatProfess
						.Where(x =>
							(x.IdDestino == codigoDoUsuario && x.TipoDestino == tipoDoUsuario) ||
							(x.IdOrigem == codigoDoUsuario && x.TipoOrigem == tipoDoUsuario))
						   let usuarioDoParametroEhOrigem =
							   codigoDoUsuario == mensagem.IdOrigem && tipoDoUsuario == mensagem.TipoOrigem
						   select new
						   {
							   mensagem.Id,
							   idDaOutraPessoa = usuarioDoParametroEhOrigem
								   ? mensagem.IdDestino
								   : mensagem.IdOrigem,
							   tipoDaOutraPessoa = usuarioDoParametroEhOrigem
								   ? mensagem.TipoDestino
								   : mensagem.TipoOrigem,
							   data = mensagem.DtMensagem
						   })
				//.ToList()
				//.DistinctBy(x => new
				//{
				//	x.idDaOutraPessoa,
				//	x.tipoDaOutraPessoa
				//})
				.ToList();

			var mensagensDasPessoas = pessoas.GroupBy(x => new
			{
				x.idDaOutraPessoa,
				x.tipoDaOutraPessoa
			}, (pessoa, mensagens) => mensagens
					.OrderByDescending(x => x.data)
					.Select(x => x.Id)
					.Take(10)
					.ToList())
				.ToList();

			List<int> ids = this.JuntarMensagensEmUmaListaSo(mensagensDasPessoas).ToList();

			return this._db.ChatProfess.Where(x => ids.Contains(x.Id)).ToList();
		}

		private IEnumerable<int> JuntarMensagensEmUmaListaSo(List<List<int>> mensagensDasPessoas)
		{
			foreach (List<int> mensagensDaPessoa in mensagensDasPessoas)
			{
				foreach (int mensagemDaPessoa in mensagensDaPessoa)
				{
					yield return mensagemDaPessoa;
				}
			}
		}

		//public List<ChatProfess> GetMensagensDoUsuario(
		//	int codigoDoUsuario,
		//	string tipoDoUsuario)
		//{
		//	List<ChatProfess> mensagens = this
		//		._db
		//		.ChatProfess
		//		.Where(x =>
		//			(x.IdDestino == codigoDoUsuario && x.TipoDestino == tipoDoUsuario) ||
		//			(x.IdOrigem == codigoDoUsuario && x.TipoOrigem == tipoDoUsuario))
		//		.OrderBy(x => x.DtMensagem)
		//		.ToList();
		//	return mensagens;
		//}

		public SaeResponseRepository<List<ChatProfess>> GetMensagens(int codigoDoUsuario1,
			string tipoDoUsuario1,
			int codigoDoUsuario2,
			string tipoDoUsuario2, int codigoDaPrimeiraMensagemNoChat)
		{
			IQueryable<ChatProfess> query = this
				.GetMensagensAsQueryable(
					codigoDoUsuario1,
					tipoDoUsuario1,
					codigoDoUsuario2,
					tipoDoUsuario2);

			if (codigoDaPrimeiraMensagemNoChat > 0)
			{
				query = query
					.Where(x => x.Id < codigoDaPrimeiraMensagemNoChat)
					.OrderByDescending(x => x.DtMensagem)
					.Take(10);
			}


			List<ChatProfess> lista = query.ToList();

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