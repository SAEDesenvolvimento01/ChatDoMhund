using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;

namespace ChatDoMhund.Models.Poco
{
    public class PkUsuarioLogado
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string TipoDeUsuario { get; set; }
        public PkUsuarioLogadoPermissoes Permissoes { get; set; }
        public PkUsuarioLogadoRelacaoComAluno RelacaoComAluno { get; set; }
        public string OrigemDeChat { get; set; }
        public string TipoDeProfessor { get; set; }

        public override string ToString()
        {
            return $"{this.Codigo} - {this.Nome}";
        }

        public void SetPermissoes(AppCfg configuracoesDeApp)
        {
            this.Permissoes = new PkUsuarioLogadoPermissoes(this.TipoDeUsuario, configuracoesDeApp);
        }

        public void SetOrigemDeChat(string origemDeChat)
        {
            this.OrigemDeChat = origemDeChat;
        }

        public void SetRelacaoComAluno(int codigoDoAluno, string nomeDoAluno, string tipoDeRelacao)
        {
            if (this.TipoDeUsuario == TipoDeUsuarioDoChatTrata.Responsavel)
            {
                this.RelacaoComAluno = new PkUsuarioLogadoRelacaoComAluno
                {
                    CodigoDoAluno = codigoDoAluno,
                    NomeDoAluno = nomeDoAluno,
                    TipoDeRelacao = tipoDeRelacao
                };
            }
        }
    }
}
