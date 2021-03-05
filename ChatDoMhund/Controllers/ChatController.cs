using ChatDoMhund.Controllers.Abstract;
using ChatDoMhund.Data.Repository;
using ChatDoMhund.Models.Infra;
using ChatDoMhund.Models.Poco;
using ChatDoMhund.Models.Tratamento;
using ChatDoMhund.Models.ViewModels;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Handlers;
using HelperSaeStandard11.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ChatDoMhund.Controllers
{
	public class ChatController : AbsController
	{
		private readonly ISaeHelperCookie _saeHelperCookie;
		private readonly AlunosRepository _alunosRepository;
		private UsuarioLogado _usuarioLogado;
		private readonly CadforpsRepository _cadforpsRepository;
		private readonly GroupBuilder _groupBuilder;

		public ChatController(ISaeHelperCookie saeHelperCookie,
			AlunosRepository alunosRepository,
			UsuarioLogado usuarioLogado,
			CadforpsRepository cadforpsRepository,
			GroupBuilder groupBuilder)
		{
			this._saeHelperCookie = saeHelperCookie;
			this._alunosRepository = alunosRepository;
			this._usuarioLogado = usuarioLogado;
			this._cadforpsRepository = cadforpsRepository;
			this._groupBuilder = groupBuilder;
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
			var lista = new List<PkConversa>();

			string codigoDaEscola = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente);
			this._usuarioLogado = this._usuarioLogado.GetUsuarioLogado();
			if (this._usuarioLogado.TipoDeUsuario == TipoDeUsuarioTrata.Aluno)
			{
				Cadforps cadforps1 = this._cadforpsRepository.GetById(1143).Content;
				Cadforps cadforps2 = this._cadforpsRepository.GetById(6).Content;
				foreach (Cadforps cadforps in new List<Cadforps> { cadforps1, cadforps2 })
				{
					lista.Add(new PkConversa
					{
						Nome = cadforps.Nome,
						Foto = FotoTrata.ToBase64String(cadforps.Foto),
						Status = TipoDeUsuarioTrata.TipoExtenso(TipoDeUsuarioTrata.Professor),
						Codigo = cadforps.Codigo,
						Tipo = TipoDeUsuarioTrata.Professor,
						CodigoDaEscola = codigoDaEscola.ConvertToInt32(),
						GroupName = this._groupBuilder.GetGroupName(codigoDaEscola, TipoDeUsuarioTrata.Professor, cadforps.Codigo.ToString())
					});
				}
			}
			else if (this._usuarioLogado.TipoDeUsuario == TipoDeUsuarioTrata.Professor)
			{
				Alunos alunos1 = this._alunosRepository.GetById(750).Content;
				Alunos alunos2 = this._alunosRepository.GetById(861).Content;

				foreach (Alunos alunos in new List<Alunos> { alunos1, alunos2 })
				{
					lista.Add(new PkConversa
					{
						Nome = alunos.Nome,
						Foto = FotoTrata.ToBase64String(alunos.Foto),
						Status = TipoDeUsuarioTrata.TipoExtenso(TipoDeUsuarioTrata.Aluno),
						Codigo = alunos.Codigo,
						Tipo = TipoDeUsuarioTrata.Aluno,
						CodigoDaEscola = codigoDaEscola.ConvertToInt32(),
						GroupName = this._groupBuilder.GetGroupName(codigoDaEscola, TipoDeUsuarioTrata.Aluno, alunos.Codigo.ToString())
					});
				}
			}
			SaeResponse response = new SaeResponse
			{
				Status = true,
				Content = lista
			};

			return this.Json(response);
		}
	}
}