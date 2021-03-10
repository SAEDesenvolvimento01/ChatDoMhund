using ChatDoMhund.Models.Poco;
using ChatDoMhund.Models.Tratamento;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Handlers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChatDoMhund.Data.Repository;
using HelperSaeCore31.Models.Enum;

namespace ChatDoMhund.Models.Domain
{
	public class ChatDomain
	{
		private readonly ISaeHelperCookie _helperCookie;
		private readonly GroupBuilder _groupBuilder;
		private readonly ChatProfessRepository _chatProfessRepository;
		private readonly AlunosRepository _alunosRepository;
		private readonly CadforpsRepository _cadforpsRepository;
		private readonly PessoasRepository _pessoasRepository;

		public ChatDomain(ISaeHelperCookie helperCookie,
			GroupBuilder groupBuilder,
			ChatProfessRepository chatProfessRepository,
			AlunosRepository alunosRepository,
			CadforpsRepository cadforpsRepository,
			PessoasRepository pessoasRepository)
		{
			this._helperCookie = helperCookie;
			this._groupBuilder = groupBuilder;
			this._chatProfessRepository = chatProfessRepository;
			this._alunosRepository = alunosRepository;
			this._cadforpsRepository = cadforpsRepository;
			this._pessoasRepository = pessoasRepository;
		}

		public List<PkConversa> GetMensagensDoUsuario(int codigoDoUsuario, string tipoDoUsuario)
		{
			List<ChatProfess> mensagens =
				this._chatProfessRepository.GetMensagensDoUsuario(codigoDoUsuario, tipoDoUsuario);

			PkCodigosDasPessoasDasMensagens codigosETipos = this
				.GetCodigosDosUsuariosDasMensagens(mensagens)
				.RemoverUsuarioLogado(codigoDoUsuario, tipoDoUsuario);

			int codigoDaEscola = this._helperCookie.GetCookie(ECookie.CodigoDoCliente).ConvertToInt32();

			List<PkUsuarioConversa> usuariosDasConversas = new List<PkUsuarioConversa>();
			List<PkConversa> conversas = new List<PkConversa>();

			if (codigosETipos.CodigosDosAlunos.Any())
			{
				usuariosDasConversas =
					this._alunosRepository.GetAlunosDasConversas(codigosETipos.CodigosDosAlunos).Content;
			}

			if (codigosETipos.CodigosDosCoordenadores.Any() || codigosETipos.CodigosDosProfessores.Any())
			{
				usuariosDasConversas = this._cadforpsRepository.GetProfessoresECoordenadoresDasConversas(
					codigosETipos.CodigosDosCoordenadores, codigosETipos.CodigosDosProfessores).Content;
			}

			if (codigosETipos.CodigosDosResponsaveis.Any())
			{
				usuariosDasConversas = this._pessoasRepository
					.GetResponsaveisDasConversas(codigosETipos.CodigosDosResponsaveis).Content;
			}

			if (usuariosDasConversas.Any())
			{
				foreach (PkUsuarioConversa usuario in usuariosDasConversas)
				{
					List<ChatProfess> mensagensDoUsuario =
						mensagens.Where(mensagem => usuario.MensagemEhDesteUsuario(mensagem)).ToList();

					conversas.Add(new PkConversa(usuario, mensagensDoUsuario, codigoDaEscola, _groupBuilder));
				}
			}

			return conversas;
		}

		private PkCodigosDasPessoasDasMensagens GetCodigosDosUsuariosDasMensagens(List<ChatProfess> mensagens)
		{
			PkCodigosDasPessoasDasMensagens codigosETipos = new PkCodigosDasPessoasDasMensagens();

			TipoDeUsuarioTrata.GetTipos().ForEach(tipo =>
			{
				codigosETipos
					.Add(this.GetDestinatariosDoTipo(mensagens, tipo), tipo)
					.Add(this.GetOrigensDoTipo(mensagens, tipo), tipo);
			});

			codigosETipos.CodigosDosAlunos = codigosETipos.CodigosDosAlunos.Distinct().ToList();
			codigosETipos.CodigosDosCoordenadores = codigosETipos.CodigosDosCoordenadores.Distinct().ToList();
			codigosETipos.CodigosDosProfessores = codigosETipos.CodigosDosProfessores.Distinct().ToList();
			codigosETipos.CodigosDosResponsaveis = codigosETipos.CodigosDosResponsaveis.Distinct().ToList();

			return codigosETipos;
		}

		private List<int> GetDestinatariosDoTipo(List<ChatProfess> mensagens, string tipo)
		{
			List<int> destinatarios = mensagens
				.Select(x => new { x.TipoDestino, x.IdDestino })
				.Where(x => x.TipoDestino == tipo)
				.Select(x => x.IdDestino.ConvertToInt32())
				.ToList();

			return destinatarios;
		}

		private List<int> GetOrigensDoTipo(List<ChatProfess> mensagens, string tipo)
		{
			List<int> destinatarios = mensagens
				.Select(x => new { x.TipoOrigem, x.IdOrigem })
				.Where(x => x.TipoOrigem == tipo)
				.Select(x => x.IdOrigem.ConvertToInt32())
				.ToList();

			return destinatarios;
		}
	}
}
