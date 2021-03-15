using ChatDoMhund.Models.Poco;
using ChatDoMhundStandard.Tratamento;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace ChatDoMhund.Models.ViewModels
{
	public class PesquisaContatosIndexModel
	{
		private readonly PkUsuarioLogado _usuarioLogado;
		public string TituloDaModal { get; set; }
		public string CursoEFase { get; set; }
		public List<PkHabilitacaoProfessor> ListaDeCursosHabilitados { get; set; }
		public List<string> TiposSelecionados { get; set; }
		public bool EhProfessorOuCoordenador { get; set; }

		public PesquisaContatosIndexModel()
		{
			this.ListaDeCursosHabilitados = new List<PkHabilitacaoProfessor>();
			this.TiposSelecionados = new List<string>();
		}

		public PesquisaContatosIndexModel(bool ehProfessorOuCoordenador, List<PkHabilitacaoProfessor> listaDeCursosHabilitados, PkUsuarioLogado usuarioLogado)
		{
			this._usuarioLogado = usuarioLogado;
			this.EhProfessorOuCoordenador = ehProfessorOuCoordenador;
			this.ListaDeCursosHabilitados = listaDeCursosHabilitados;
			this.TituloDaModal = "Contatos";
		}

		public SelectList GetSelectListListaDeCursosHabilitados()
		{
			List<SelectListItem> items = this
				.ListaDeCursosHabilitados
				.Select(x =>
					new SelectListItem(this.GetDadosDoCursoParaSelectList(x), 
						$"{x.CodigoDoCurso}-{x.Fase}"))
				.ToList();

			return new SelectList(items.ToList(), "Value", "Text");
		}

		private string GetDadosDoCursoParaSelectList(PkHabilitacaoProfessor x)
		{
			if(x.Fase != "SAE")
			{
				return $"{x.CodigoDoCurso} - {x.DescricaoDoCurso}, {x.NomeDaFase}: {x.Fase}";
			}

			return $"{x.CodigoDoCurso} - {x.DescricaoDoCurso}";
		}

		public IHtmlContent GetChipAluno()
		{
			if (this._usuarioLogado.Permissoes.ConversaComAluno)
			{
				return this.GetChip(TipoDeUsuarioDoChatTrata.Aluno, TipoDeUsuarioDoChatTrata.AlunoExtenso);
			}

			return this.HtmlStringVazio();
		}

		public IHtmlContent GetChipProfessor()
		{
			if (this._usuarioLogado.Permissoes.ConversaComProfessor)
			{
				return this.GetChip(TipoDeUsuarioDoChatTrata.Professor, TipoDeUsuarioDoChatTrata.ProfessorExtenso);
			}

			return this.HtmlStringVazio();
		}

		public IHtmlContent GetChipCoordenador()
		{
			if (this._usuarioLogado.Permissoes.ConversaComCoordenador)
			{
				return this.GetChip(TipoDeUsuarioDoChatTrata.Coordenador, TipoDeUsuarioDoChatTrata.CoordenadorExtenso);
			}

			return this.HtmlStringVazio();
		}

		public IHtmlContent GetChipResponsavel()
		{
			if (this._usuarioLogado.Permissoes.ConversaComResponsavel)
			{
				return this.GetChip(TipoDeUsuarioDoChatTrata.Responsavel, TipoDeUsuarioDoChatTrata.ResponsavelExtenso);
			}

			return this.HtmlStringVazio();
		}

		private HtmlString HtmlStringVazio()
		{
			return new HtmlString("");
		}

		private IHtmlContent GetChip(string tipo, string tipoExtenso)
		{
			return new HtmlString(
				$"<div class=\"chip pointer chip-filtro-usuario hoverable\" tipo-de-usuario-para-filtrar=\"{tipo}\">" +
				$"{tipoExtenso}" +
				"</div>");
		}
	}
}
