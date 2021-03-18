using System;

namespace ChatDoMhund.Models.Poco
{
	public class PkHistoricoDoAluno
	{
		public int CodigoDoCurso { get; set; }
		public string Fase { get; set; }
		public int CodigoDoAluno { get; set; }
		public DateTime DataDeCadastro { get; set; }
		public string Ano { get; set; }
	}
}
