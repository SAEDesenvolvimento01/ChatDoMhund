using System.Collections.Generic;
using ChatDoMhund.Controllers.Abstract;
using ChatDoMhund.Models.Domain;
using ChatDoMhund.Models.Poco;
using ChatDoMhund.Models.ViewModels;
using HelperSaeCore31.Models.Infra.ControllerComponents.Interface;
using HelperSaeStandard11.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatDoMhund.Controllers
{
	public class PesquisarContatosController : AbsController
	{
		private readonly IViewRenderService _viewRenderService;
		private readonly PesquisarContatosDomain _pesquisarContatosDomain;

		public PesquisarContatosController(IViewRenderService viewRenderService,
			PesquisarContatosDomain pesquisarContatosDomain)
		{
			this._viewRenderService = viewRenderService;
			this._pesquisarContatosDomain = pesquisarContatosDomain;
		}

		public IActionResult Index()
		{
			List<PkUsuarioConversa> usuarios = this._pesquisarContatosDomain.Get();
			PesquisaContatosIndexModel model = new PesquisaContatosIndexModel(usuarios);
			string view = this
				._viewRenderService
				.RenderToString(this, "Index", model);

			return this.Json(new SaeResponse(view));
		}
	}
}