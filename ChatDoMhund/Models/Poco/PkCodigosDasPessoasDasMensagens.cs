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
			if (tipo == TipoDeUsuarioTrata.Aluno)
			{
				this.CodigosDosAlunos.AddRange(codigos);
			}
			if (tipo == TipoDeUsuarioTrata.Professor)
			{
				this.CodigosDosProfessores.AddRange(codigos);
			}
			if (tipo == TipoDeUsuarioTrata.Coordenador)
			{
				this.CodigosDosCoordenadores.AddRange(codigos);
			}
			if (tipo == TipoDeUsuarioTrata.Responsavel)
			{
				this.CodigosDosResponsaveis.AddRange(codigos);
			}

			return this;
		}

		public PkCodigosDasPessoasDasMensagens RemoverUsuarioLogado(int codigoDoUsuario, string tipoDoUsuario)
		{
			if (tipoDoUsuario == TipoDeUsuarioTrata.Aluno)
			{
				this.CodigosDosAlunos.Remove(codigoDoUsuario);
			}
			if (tipoDoUsuario == TipoDeUsuarioTrata.Professor)
			{
				this.CodigosDosProfessores.Remove(codigoDoUsuario);
			}
			if (tipoDoUsuario == TipoDeUsuarioTrata.Coordenador)
			{
				this.CodigosDosCoordenadores.Remove(codigoDoUsuario);
			}
			if (tipoDoUsuario == TipoDeUsuarioTrata.Responsavel)
			{
				this.CodigosDosResponsaveis.Remove(codigoDoUsuario);
			}

			return this;
		}
	}
}
