using ChatDoMhund.Data.Repository;
using ChatDoMhund.Models.Enum;
using ChatDoMhund.Models.Poco;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperSaeCore31.Extensions;
using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeCore31.Models.Infra.Criptography;
using HelperSaeCore31.Models.Infra.Session.Interface;
using HelperSaeStandard11.Handlers;
using HelperSaeStandard11.Models;
using HelperSaeStandard11.Models.Infra;
using HelperSaeStandard11.Models.Tratamento;

namespace ChatDoMhund.Models.Infra
{
    public class UsuarioLogado : PkUsuarioLogado
    {
        private readonly ISaeHelperSession _saeHelperSession;
        private readonly ISaeHelperCookie _saeHelperCookie;
        private readonly SaeCriptography _saeCriptography;
        private readonly AlunosRepository _alunosRepository;
        private readonly AppCfgRepository _appCfgRepository;
        private readonly CadforpsRepository _cadforpsRepository;
        private readonly PessoasRepository _pessoasRepository;

        public UsuarioLogado(ISaeHelperSession saeHelperSession,
            ISaeHelperCookie saeHelperCookie,
            SaeCriptography saeCriptography,
            AlunosRepository alunosRepository,
            AppCfgRepository appCfgRepository,
            CadforpsRepository cadforpsRepository,
            PessoasRepository pessoasRepository)
        {
            this._saeHelperSession = saeHelperSession;
            this._saeHelperCookie = saeHelperCookie;
            this._saeCriptography = saeCriptography;
            this._alunosRepository = alunosRepository;
            this._appCfgRepository = appCfgRepository;
            this._cadforpsRepository = cadforpsRepository;
            this._pessoasRepository = pessoasRepository;
        }

        public UsuarioLogado GetUsuarioLogado()
        {
	        string logado = this._saeHelperSession.GetUsuarioLogado();
	        if (logado.TryDeserialize(out PkUsuarioLogado usuarioLogado))
            {
                this.Codigo = usuarioLogado.Codigo;
                this.Nome = usuarioLogado.Nome;
                this.OrigemDeChat = usuarioLogado.OrigemDeChat;
                this.TipoDeUsuario = usuarioLogado.TipoDeUsuario;
                this.RelacaoComAluno = usuarioLogado.RelacaoComAluno;
                this.Permissoes = usuarioLogado.Permissoes;
            }

            return this;
        }

        public bool SetUsuarioLogado(int codigoDaEscola,
            int codigoDoUsuario,
            string tipoDoUsuario,
            string origemDeChat,
            int codigoDoAluno,
            string tipoDeRelacao)
        {
            bool status = false;
            SaeResponseRepository<PkUsuarioLogado> response;

            if (tipoDoUsuario == TipoDeUsuarioDoChatTrata.Aluno)
            {
                response = this._alunosRepository.GetUsuarioParaLogar(codigoDoUsuario);
            }
            else if (tipoDoUsuario == TipoDeUsuarioDoChatTrata.Professor || tipoDoUsuario == TipoDeUsuarioDoChatTrata.Coordenador)
            {
                response = this._cadforpsRepository.GetUsuarioParaLogar(codigoDoUsuario);
            }
            else if (tipoDoUsuario == TipoDeUsuarioDoChatTrata.Responsavel)
            {
                response = this._pessoasRepository.GetUsuarioParaLogar(codigoDoUsuario);
            }
            else
            {
                response = null;
            }

            if (response?.Status ?? false)
            {
                PkUsuarioLogado usuarioLogado = response.Content;

                SaeResponseRepository<AppCfg> responseConfig = this._appCfgRepository.GetFirstOrDefault();
                usuarioLogado.SetOrigemDeChat(origemDeChat);
                usuarioLogado.SetPermissoes(responseConfig.Content);
                usuarioLogado.SetRelacaoComAluno(codigoDoAluno, tipoDeRelacao);

                this.SetDados(usuarioLogado);

                if (this.TrySerialize(out string json))
                {
                    this.SetSessions(json);
                    this.SetCookies(
                        codigoDaEscola: codigoDaEscola,
                        codigoDoUsuario: codigoDoUsuario,
                        tipoDoUsuario: tipoDoUsuario,
                        origemDeChat: origemDeChat,
                        codigoDoAlunoSelecionado: codigoDoAluno,
                        tipoDeRelacaoComAluno: tipoDeRelacao);
                    status = true;
                }
                else
                {
                    throw new SaeException("Não foi possível serializar o usuário para salvar na session.");
                }
            }

            return status;
        }

        private void SetDados(PkUsuarioLogado usuario)
        {
            this.Codigo = usuario.Codigo;
            this.Nome = usuario.Nome;
            this.TipoDeUsuario = usuario.TipoDeUsuario;
            this.Permissoes = usuario.Permissoes;
            this.RelacaoComAluno = usuario.RelacaoComAluno;
        }

        public bool EstaLogado() => this.Codigo > 0;

        public bool ConseguiuRelogar()
        {
            int codigoDaEscola = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente).ConvertToInt32();
            int codigoDoUsuario = this._saeHelperCookie.GetCookie(ECookie.CodigoDoUsuario).ConvertToInt32();
            string tipoDoUsuario = this._saeHelperCookie.GetCookie(ECookie.TipoUsuario);
            int codigoDoAluno = this._saeHelperCookie.GetCookie(EChatCookie.CodigoDoAlunoSelecionado.ToString()).ConvertToInt32();
            string origemDeChat = this._saeHelperCookie.GetCookie(EChatCookie.OrigemDeChat.ToString());
            string tipoDeRelacao = this._saeHelperCookie.GetCookie(EChatCookie.TipoDeRelacaoComAluno.ToString());
            bool ehResponsavel = tipoDeRelacao == TipoDeUsuarioDoChatTrata.Responsavel;
            bool relacaoComAlunoEstaPreenchida = codigoDoAluno > 0 && !string.IsNullOrEmpty(tipoDeRelacao);
            if (codigoDaEscola > 0 &&
                codigoDoUsuario > 0 &&
                !string.IsNullOrEmpty(tipoDoUsuario) &&
                (!ehResponsavel || relacaoComAlunoEstaPreenchida))
            {
                string criptografiaParaValidar = this.GetHashUsuarioLogado(codigoDaEscola, codigoDoUsuario, tipoDoUsuario, origemDeChat, codigoDoAluno, tipoDeRelacao);
                string criptografiaNoCookie = this._saeHelperCookie.GetCookie(ECookie.SaeSession);
                if (this._saeCriptography.Comparar(criptografiaParaValidar, criptografiaNoCookie))
                {
                    return this.SetUsuarioLogado(
                        codigoDaEscola: codigoDaEscola,
                        codigoDoUsuario: codigoDoUsuario,
                        tipoDoUsuario: tipoDoUsuario,
                        origemDeChat: origemDeChat,
                        codigoDoAluno: codigoDoAluno,
                        tipoDeRelacao: tipoDeRelacao);
                }
            }

            return false;
        }

        public void Sair()
        {
            this.Codigo = 0;
            this.Nome = string.Empty;
            this.TipoDeUsuario = string.Empty;
            this.Permissoes = null;
            this.RelacaoComAluno = null;

            if (this.TrySerialize(out string json))
            {
                this.SetSessions(json);
                this._saeHelperCookie.SetCookie(cookie: ECookie.SaeSession, valor: string.Empty, duracaoEmMinutos: 0,
                    serverOnly: true);
            }
        }

        public string GetHashUsuarioLogado(int codigoDaEscola, int codigoDoUsuario, string tipoDoUsuario,
            string origemDeChat, int codigoDoAluno, string tipoDeRelacao)
        {
            if (tipoDoUsuario == TipoDeUsuarioDoChatTrata.Responsavel)
            {
                return this._saeCriptography
                    .GerarCriptografia($"{codigoDaEscola}{codigoDoUsuario}{tipoDoUsuario}{origemDeChat}{codigoDoAluno}{tipoDeRelacao}");
            }

            return this._saeCriptography.GerarCriptografia($"{codigoDaEscola}{codigoDoUsuario}{tipoDoUsuario}{origemDeChat}");
        }

        private void SetCookies(int codigoDaEscola, int codigoDoUsuario, string tipoDoUsuario, string origemDeChat,
            int codigoDoAlunoSelecionado, string tipoDeRelacaoComAluno)
        {
            this._saeHelperCookie.SetCookie(
                cookie: ECookie.CodigoDoUsuario,
                valor: this.Codigo.ToString(),
                duracaoEmMinutos: Tempo.SeteDiasEmMinutos,
                serverOnly: true);

            this._saeHelperCookie.SetCookie(
                cookie: ECookie.CodigoDoCliente,
                valor: codigoDaEscola.ToString(),
                duracaoEmMinutos: Tempo.SeteDiasEmMinutos,
                serverOnly: true);

            this._saeHelperCookie.SetCookie(
                cookie: ECookie.TipoUsuario,
                valor: tipoDoUsuario,
                duracaoEmMinutos: Tempo.SeteDiasEmMinutos,
                serverOnly: true);

            this._saeHelperCookie.SetCookie(
                cookie: EChatCookie.OrigemDeChat.ToString(),
                valor: origemDeChat,
                duracaoEmMinutos: Tempo.SeteDiasEmMinutos,
                serverOnly: true);

            this._saeHelperCookie.SetCookie(
                cookie: EChatCookie.CodigoDoAlunoSelecionado.ToString(),
                valor: codigoDoAlunoSelecionado.ToString(),
                duracaoEmMinutos: Tempo.SeteDiasEmMinutos,
                serverOnly: true);

            this._saeHelperCookie.SetCookie(
                cookie: EChatCookie.TipoDeRelacaoComAluno.ToString(),
                valor: tipoDeRelacaoComAluno,
                duracaoEmMinutos: Tempo.SeteDiasEmMinutos,
                serverOnly: true);

            string hash = this.GetHashUsuarioLogado(codigoDaEscola, 
                codigoDoUsuario,
                tipoDoUsuario,
                origemDeChat, 
                codigoDoAlunoSelecionado,
                tipoDeRelacaoComAluno);
            this._saeHelperCookie.SetCookie(
                cookie: ECookie.SaeSession,
                valor: hash,
                duracaoEmMinutos: Tempo.OitoHorasEmMinutos,
                serverOnly: true);
        }

        private void SetSessions(string json)
        {
            this._saeHelperSession.SetUsuarioLogado(json);
        }

        public bool EhProfessorOuCoordenador()
        {
	        return this.TipoDeUsuario == TipoDeUsuarioDoChatTrata.Professor ||
	               this.TipoDeUsuario == TipoDeUsuarioDoChatTrata.Coordenador;
        }
    }
}