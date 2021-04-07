using System.Collections.Generic;
using System.Linq;
using ChatDoMhund.Controllers.Abstract;
using ChatDoMhund.Data.Repository;
using ChatDoMhund.Models.Domain;
using ChatDoMhund.Models.Infra;
using ChatDoMhund.Models.Poco;
using ChatDoMhund.Models.ViewModels;
using ChatDoMhundStandard.Tratamento;
using HelperSaeCore31.Models.Infra.ControllerComponents.Interface;
using HelperSaeStandard11.Models;
using HelperSaeStandard11.Models.Extension;
using Microsoft.AspNetCore.Mvc;

namespace ChatDoMhund.Controllers
{
	public class PesquisarContatosController : AbsController
	{
		private readonly IViewRenderService _viewRenderService;
		private readonly PesquisarContatosDomain _pesquisarContatosDomain;
		private readonly UsuarioLogado _usuarioLogado;
		private readonly ProfHabilitaRepository _profHabilitaRepository;
		private readonly HistoricoRepository _historicoRepository;

		public PesquisarContatosController(IViewRenderService viewRenderService,
			PesquisarContatosDomain pesquisarContatosDomain,
			UsuarioLogado usuarioLogado,
			ProfHabilitaRepository profHabilitaRepository,
			HistoricoRepository historicoRepository
			)
		{
			this._viewRenderService = viewRenderService;
			this._pesquisarContatosDomain = pesquisarContatosDomain;
			this._usuarioLogado = usuarioLogado;
			this._profHabilitaRepository = profHabilitaRepository;
			this._historicoRepository = historicoRepository;
		}

		public IActionResult Index()
		{
			this._usuarioLogado.GetUsuarioLogado();

			List<PkHabilitacaoProfessor> habilitacoes = new List<PkHabilitacaoProfessor>();
			bool ehProfessorOuCoordenador = this._usuarioLogado.EhProfessorOuCoordenador();

			if (ehProfessorOuCoordenador)
			{
				string anoLetivo = this._historicoRepository.GetAnoLetivo().Content;
				habilitacoes = this._profHabilitaRepository
					.GetHabilitacoes(codigoDoProfessor:this._usuarioLogado.Codigo, anoLetivo:anoLetivo)
					.Content
					.DistinctBy(x => new { x.CodigoDoCurso, x.Fase })
					.ToList();
			}

			PesquisaContatosIndexModel model = new PesquisaContatosIndexModel(
				ehProfessorOuCoordenador: ehProfessorOuCoordenador,
				listaDeCursosHabilitados: habilitacoes,
				usuarioLogado: this._usuarioLogado);
			string view = this
				._viewRenderService
				.RenderToString(this, "Index", model);

			return this.Json(new SaeResponse(view));
		}

		public JsonResult AtualizarLista(PesquisaContatosIndexModel index)
		{
			this._usuarioLogado.GetUsuarioLogado();
			List<PkUsuarioConversa> usuarios = this._pesquisarContatosDomain.Get(index);

			bool ehProfessorOuCoordenador = this._usuarioLogado.EhProfessorOuCoordenador();

			PesquisaContatosListaModel model = new PesquisaContatosListaModel(
				usuarios: usuarios,
				ehProfessorOuCoordenador: ehProfessorOuCoordenador);
			string view = this
				._viewRenderService
				.RenderToString(this, "_Lista", model);

			return this.Json(new SaeResponse(view));
		}
	}
}