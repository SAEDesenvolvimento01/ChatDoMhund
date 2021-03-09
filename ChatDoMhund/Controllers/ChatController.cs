using ChatDoMhund.Controllers.Abstract;
using ChatDoMhund.Models.Infra;
using ChatDoMhund.Models.Poco;
using ChatDoMhund.Models.ViewModels;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperSaeCore31.Models.Enum;
using HelperSaeStandard11.Handlers;
using HelperSaeStandard11.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ChatDoMhund.Models.Domain;

namespace ChatDoMhund.Controllers
{
	public class ChatController : AbsController
	{
		private UsuarioLogado _usuarioLogado;
		private readonly ChatDomain _chatDomain;

		public ChatController(UsuarioLogado usuarioLogado,
			ChatDomain chatDomain)
		{
			this._usuarioLogado = usuarioLogado;
			this._chatDomain = chatDomain;
		}

		public IActionResult Index()
		{
			this._usuarioLogado = this._usuarioLogado.GetUsuarioLogado();
			return this.View("Index", new ChatIndexViewModel
			{
				UsuarioLogado = this._usuarioLogado
			});
		}

		public JsonResult GetConversas()
		{
			this._usuarioLogado.GetUsuarioLogado();
			List<PkConversa> lista = new List<PkConversa>(this._chatDomain.GetMensagensDoUsuario(this._usuarioLogado.Codigo, this._usuarioLogado.TipoDeUsuario));

			//lista.AddRange(lista);
			//lista.AddRange(lista);
			//lista.AddRange(lista);
			//lista.AddRange(lista);
			//lista.AddRange(lista);

			SaeResponse response = new SaeResponse
			{
				Status = true,
				Content = lista
			};

			return this.Json(response);
		}
	}
}