using ChatDoMhund.Data.Repository.Abstract;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using System.Collections.Generic;
using System.Linq;

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
				.ToList();
			return mensagens;
		}
	}
}