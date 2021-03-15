using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperSaeStandard11.Models.Tratamento;

namespace ChatDoMhund.Models.Poco
{
	public class PkUsuarioLogadoPermissoes
	{
		public bool ConversaComProfessor { get; set; }
		public bool ConversaComCoordenador { get; set; }
		public bool ConversaComResponsavel { get; set; }
		public bool ConversaComAluno { get; set; }

		public PkUsuarioLogadoPermissoes()
		{

		}

		public PkUsuarioLogadoPermissoes(string tipoDeUsuario, AppCfg configuracoesDeApp)
		{
			if (tipoDeUsuario == TipoDeUsuarioDoChatTrata.Aluno)
			{
				this.SetPermissoes(
					conversaComProfessor: configuracoesDeApp.CAluXPro == SaeSituacao.Sim,
					conversaComCoordenador: configuracoesDeApp.CAluXCoo == SaeSituacao.Sim,
					conversaComResponsavel: false,
					conversaComAluno: configuracoesDeApp.CAluXAlu == SaeSituacao.Sim);
			}
			else if (tipoDeUsuario == TipoDeUsuarioDoChatTrata.Responsavel)
			{
				this.SetPermissoes(
					conversaComProfessor: configuracoesDeApp.CRespXProf == SaeSituacao.Sim,
					conversaComCoordenador: configuracoesDeApp.CRespXCoo == SaeSituacao.Sim,
					conversaComResponsavel: configuracoesDeApp.CRespXResp == SaeSituacao.Sim,
					conversaComAluno: false);
			}
			else if (tipoDeUsuario == TipoDeUsuarioDoChatTrata.Coordenador || tipoDeUsuario == TipoDeUsuarioDoChatTrata.Professor)
			{
				this.SetPermissoes(
					conversaComProfessor: true,
					conversaComCoordenador: true,
					conversaComResponsavel: true,
					conversaComAluno: true);
			}
		}

		private void SetPermissoes(bool conversaComProfessor, bool conversaComCoordenador, bool conversaComResponsavel, bool conversaComAluno)
		{
			this.ConversaComProfessor = conversaComProfessor;
			this.ConversaComCoordenador = conversaComCoordenador;
			this.ConversaComResponsavel = conversaComResponsavel;
			this.ConversaComAluno = conversaComAluno;
		}
	}
}