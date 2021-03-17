using ChatDoMhund.Data.Repository.Abstract;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ChatDoMhund.Data.Repository
{
	public class ChatLogRepository : RepositoryBase<ChatLog>
	{
		public ChatLogRepository(MhundDbContext db) : base(db)
		{
		}

		public ChatLog AtualizaUltimoAcesso(int codigoDoUsuario, string tipoDeUsuario)
		{
			ChatLog chatLog = this._db.ChatLog.FirstOrDefault(x =>
								  x.CodPess == codigoDoUsuario && x.TipoPess == tipoDeUsuario) ??
							  new ChatLog(codigoDoUsuario, tipoDeUsuario);

			chatLog.DataLog = DateTime.Now;

			if (chatLog.EhNovo())
			{
				chatLog = this._db.ChatLog.Add(chatLog).Entity;
			}
			else
			{
				this._db.Entry(chatLog).State = EntityState.Modified;
			}

			this._db.SaveChanges();

			return chatLog;
		}
	}
}