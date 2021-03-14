using ChatDoMhund.Data.Repository.Abstract;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using System.Collections.Generic;
using System.Linq;
using HelperSaeStandard11.Models;

namespace ChatDoMhund.Data.Repository
{
	public class ChatProfessRepository : RepositoryBase<ChatProfess>
	{
		public ChatProfessRepository(MhundDbContext db) : base(db)
		{
		}

		public List<ChatProfess> GetMensagensDoUsuario(int codigoDoUsuario, string tipoDoUsuario)
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

		public SaeResponseRepository<List<ChatProfess>> GetMensagens(int codigoDoUsuarioLogado, string tipoDoUsuarioLogado, int codigoDoUsuarioDaConversa, string tipoDoUsuarioDaConversa)
		{
			List<ChatProfess> lista = (
				from mensagem in this._db.ChatProfess.Where(x =>
					(x.IdOrigem == codigoDoUsuarioDaConversa &&
					 x.TipoOrigem == tipoDoUsuarioDaConversa &&
					 x.IdDestino == codigoDoUsuarioLogado &&
					 x.TipoDestino == tipoDoUsuarioLogado) ||
					(x.IdOrigem == codigoDoUsuarioLogado &&
					 x.TipoOrigem == tipoDoUsuarioLogado &&
					 x.IdDestino == codigoDoUsuarioDaConversa &&
					 x.TipoDestino == tipoDoUsuarioDaConversa))
				select mensagem
				).ToList();

			return new SaeResponseRepository<List<ChatProfess>>(lista?.Any() ?? false, lista);
		}
	}
}