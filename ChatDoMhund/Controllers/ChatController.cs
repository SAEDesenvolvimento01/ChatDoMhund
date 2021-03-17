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
using ChatDoMhund.Data.Repository;
using ChatDoMhund.Models.Domain;
using ChatDoMhund.Models.Enum;
using ChatDoMhund.Models.Tratamento;
using HelperSaeCore31.Models.Infra.Cookie.Interface;

namespace ChatDoMhund.Controllers
{
	public class ChatController : AbsController
	{
		private UsuarioLogado _usuarioLogado;
		private readonly ChatDomain _chatDomain;
		private readonly GroupBuilder _groupBuilder;
		private readonly AlunosRepository _alunosRepository;
		private readonly CadforpsRepository _cadforpsRepository;
		private readonly PessoasRepository _pessoasRepository;
		private readonly ISaeHelperCookie _saeHelperCookie;
		private readonly ChatLogRepository _chatLogRepository;

		public ChatController(UsuarioLogado usuarioLogado,
			ChatDomain chatDomain,
			GroupBuilder groupBuilder,
			AlunosRepository alunosRepository,
			CadforpsRepository cadforpsRepository,
			PessoasRepository pessoasRepository,
			ISaeHelperCookie saeHelperCookie,
			ChatLogRepository chatLogRepository)
		{
			this._usuarioLogado = usuarioLogado;
			this._chatDomain = chatDomain;
			this._groupBuilder = groupBuilder;
			this._alunosRepository = alunosRepository;
			this._cadforpsRepository = cadforpsRepository;
			this._pessoasRepository = pessoasRepository;
			this._saeHelperCookie = saeHelperCookie;
			this._chatLogRepository = chatLogRepository;
		}

		public IActionResult Index()
		{
			this._usuarioLogado = this._usuarioLogado.GetUsuarioLogado();
			this._chatLogRepository.AtualizaUltimoAcesso(this._usuarioLogado.Codigo, this._usuarioLogado.TipoDeUsuario);

			string origem = this._saeHelperCookie.GetCookie(EChatCookie.OrigemDeChat.ToString());

			return this.View("Index", new ChatIndexViewModel
			{
				UsuarioLogado = this._usuarioLogado,
				GroupName = this._groupBuilder.BuildGroupName(),
				Title = $"Chat{OrigemDeChatTrata.GetOrigemExtenso(origem, " - ")}"
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

		public JsonResult GetUsuarioParaConversa(string groupName)
		{
			this._groupBuilder.DismantleGroupName(groupName, out int codigoDoCliente, out string tipoDeUsuario,
				out int codigoDoUsuario);

			PkUsuarioConversa usuarioConversa = new PkUsuarioConversa();
			if (TipoDeUsuarioDoChatTrata.EhAluno(tipoDeUsuario))
			{
				usuarioConversa = this._alunosRepository.GetAlunoParaConversa(codigoDoUsuario).Content;
			}
			else if (TipoDeUsuarioDoChatTrata.EhCoordenadorOuProfessor(tipoDeUsuario))
			{
				usuarioConversa = this._cadforpsRepository.GetProfessorOuCoordenadorParaConversa(codigoDoUsuario)
					.Content;
			}
			else if (TipoDeUsuarioDoChatTrata.EhResponsavel(tipoDeUsuario))
			{
				usuarioConversa = this._pessoasRepository.GetResponsavelParaConversa(codigoDoUsuario).Content;
			}

			int codigoDaEscola = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente).ConvertToInt32();

			List<ChatProfess> mensagens = this._chatDomain.GetMensagens(usuarioConversa);

			PkConversa conversa = new PkConversa(usuarioConversa, mensagens, codigoDaEscola, this._groupBuilder);
			return this.Json(new SaeResponse
			{
				Status = true,
				Content = conversa
			});
		}

		public JsonResult LimparMensagens()
		{
			return this.Json(this._chatDomain.LimparTodasAsMensagens());
		}

		public JsonResult LimparLogs()
		{
			return this.Json(this._chatDomain.LimparTodosOsLogs());
		}
	}
}