using System.Collections.Generic;
using ChatDoMhundStandard.Tratamento;

namespace ChatDoMhund.Models.Poco
{
	public class PkCodigosDasPessoasDasMensagens
	{
		public List<int> CodigosDosAlunos { get; set; }
		public List<int> CodigosDosProfessores { get; set; }
		public List<int> CodigosDosCoordenadores { get; set; }
		public List<int> CodigosDosResponsaveis { get; set; }

		public PkCodigosDasPessoasDasMensagens()
		{
			this.CodigosDosAlunos = new List<int>();
			this.CodigosDosProfessores = new List<int>();
			this.CodigosDosCoordenadores = new List<int>();
			this.CodigosDosResponsaveis = new List<int>();
		}

		public PkCodigosDasPessoasDasMensagens Add(List<int> codigos, string tipo)
		{
			if (tipo == TipoDeUsuarioDoChatTrata.Aluno)
			{
				this.CodigosDosAlunos.AddRange(codigos);
			}
			if (tipo == TipoDeUsuarioDoChatTrata.Professor)
			{
				this.CodigosDosProfessores.AddRange(codigos);
			}
			if (tipo == TipoDeUsuarioDoChatTrata.Coordenador)
			{
				this.CodigosDosCoordenadores.AddRange(codigos);
			}
			if (tipo == TipoDeUsuarioDoChatTrata.Responsavel)
			{
				this.CodigosDosResponsaveis.AddRange(codigos);
			}

			return this;
		}

		public PkCodigosDasPessoasDasMensagens RemoverUsuarioLogado(int codigoDoUsuario, string tipoDoUsuario)
		{
			if (tipoDoUsuario == TipoDeUsuarioDoChatTrata.Aluno)
			{
				this.CodigosDosAlunos.Remove(codigoDoUsuario);
			}
			if (tipoDoUsuario == TipoDeUsuarioDoChatTrata.Professor)
			{
				this.CodigosDosProfessores.Remove(codigoDoUsuario);
			}
			if (tipoDoUsuario == TipoDeUsuarioDoChatTrata.Coordenador)
			{
				this.CodigosDosCoordenadores.Remove(codigoDoUsuario);
			}
			if (tipoDoUsuario == TipoDeUsuarioDoChatTrata.Responsavel)
			{
				this.CodigosDosResponsaveis.Remove(codigoDoUsuario);
			}

			return this;
		}
	}
}
