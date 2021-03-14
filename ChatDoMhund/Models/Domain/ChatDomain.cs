using ChatDoMhund.Data.Repository;
using ChatDoMhund.Models.Poco;
using ChatDoMhund.Models.Tratamento;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Handlers;
using System.Collections.Generic;
using System.Linq;
using ChatDoMhund.Models.Infra;

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
		private readonly MhundDbContext _db;
		private readonly UsuarioLogado _usuarioLogado;

		public ChatDomain(ISaeHelperCookie helperCookie,
			GroupBuilder groupBuilder,
			ChatProfessRepository chatProfessRepository,
			AlunosRepository alunosRepository,
			CadforpsRepository cadforpsRepository,
			PessoasRepository pessoasRepository,
			MhundDbContext db,
			UsuarioLogado usuarioLogado)
		{
			this._helperCookie = helperCookie;
			this._groupBuilder = groupBuilder;
			this._chatProfessRepository = chatProfessRepository;
			this._alunosRepository = alunosRepository;
			this._cadforpsRepository = cadforpsRepository;
			this._pessoasRepository = pessoasRepository;
			this._db = db;
			this._usuarioLogado = usuarioLogado;
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
				List<PkUsuarioConversa> alunos = this._alunosRepository.GetAlunosDasConversas(codigosETipos.CodigosDosAlunos).Content;
				usuariosDasConversas.AddRange(alunos);
			}

			if (codigosETipos.CodigosDosCoordenadores.Any() || codigosETipos.CodigosDosProfessores.Any())
			{
				List<PkUsuarioConversa> coordenadoresEProfessores = this._cadforpsRepository.GetProfessoresECoordenadoresDasConversas(
					codigosETipos.CodigosDosCoordenadores, codigosETipos.CodigosDosProfessores).Content;
				usuariosDasConversas.AddRange(coordenadoresEProfessores);
			}

			if (codigosETipos.CodigosDosResponsaveis.Any())
			{
				List<PkUsuarioConversa> responsaveis = this._pessoasRepository
					.GetResponsaveisDasConversas(codigosETipos.CodigosDosResponsaveis).Content;
				usuariosDasConversas.AddRange(responsaveis);
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

			conversas = conversas.OrderByDescending(x => x.GetDataDaUltimaMensagem()).ToList();
			return conversas;
		}

		private PkCodigosDasPessoasDasMensagens GetCodigosDosUsuariosDasMensagens(List<ChatProfess> mensagens)
		{
			PkCodigosDasPessoasDasMensagens codigosETipos = new PkCodigosDasPessoasDasMensagens();

			TipoDeUsuarioDoChatTrata.GetTipos().ForEach(tipo =>
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

		public string LimparTodasAsMensagens()
		{
			int cliente = this._helperCookie.GetCookie(ECookie.CodigoDoCliente).ConvertToInt32();
			if (cliente == 99123)
			{
				List<ChatProfess> todasAsMensagens = this._db.ChatProfess.ToList();

				this._db.ChatProfess.RemoveRange(todasAsMensagens);

				int quantidadeRemovida = this._db.SaveChanges();

				return $"{quantidadeRemovida} mensagens removidas";
			}

			return $"not allowed to {cliente}";
		}

		public List<ChatProfess> GetMensagens(PkUsuarioConversa conversa)
		{
			this._usuarioLogado.GetUsuarioLogado();
			return this._chatProfessRepository.GetMensagens(
				codigoDoUsuarioLogado: this._usuarioLogado.Codigo,
				tipoDoUsuarioLogado: this._usuarioLogado.TipoDeUsuario,
				codigoDoUsuarioDaConversa: conversa.Codigo,
				tipoDoUsuarioDaConversa: conversa.Tipo).Content;
		}
	}
}
