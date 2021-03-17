using ChatDoMhund.Data.Repository.Abstract;
using ChatDoMhund.Models.Poco;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models.Tratamento;
using System.Collections.Generic;
using System.Linq;
using HelperSaeStandard11.Models;

namespace ChatDoMhund.Data.Repository
{
	public class ProfHabilitaRepository : RepositoryBase<ProfHabilita>
	{
		public ProfHabilitaRepository(MhundDbContext db) : base(db)
		{
		}

		public SaeResponseRepository<List<PkHabilitacaoProfessor>> GetHabilitacoes(int codigoDoProfessor)
		{
			List<PkHabilitacaoProfessor> habilitacoes =
				(from profHabilita in this._db.ProfHabilita.Where(x => x.CodProf == codigoDoProfessor)
				 join curso in this._db.Cursos.Where(x => x.Situacao == SaeSituacao.Ativo)
					 on profHabilita.CodCurso equals curso.Nseq
				 select new PkHabilitacaoProfessor
				 {
					 Fase = profHabilita.Fase,
					 CodigoDoCurso = profHabilita.CodCurso ?? 0,
					 CodigoDaMateria = profHabilita.CodMateria ?? 0,
					 CodigoDoProfessor = profHabilita.CodProf ?? 0,
					 DescricaoDoCurso = curso.Descricao,
					 NomeDaFase = curso.Nomedasfases
				 })
				.OrderBy(x => x.CodigoDoCurso)
				.ThenBy(x => x.Fase)
				.ThenBy(x => x.CodigoDoProfessor)
				.ToList();

			return new SaeResponseRepository<List<PkHabilitacaoProfessor>>(habilitacoes.Any(), habilitacoes);
		}

		public SaeResponseRepository<List<PkHabilitacaoProfessor>> GetHabilitacoes(PkHistoricoDoAluno historico)
		{
			return this.GetHabilitacoes(historico.CodigoDoCurso, historico.Fase);
		}

		public SaeResponseRepository<List<PkHabilitacaoProfessor>> GetHabilitacoes(int codigoDoCurso, string fase)
		{
			List<PkHabilitacaoProfessor> habilitacoes =
				(from profHabilita in this._db.ProfHabilita.Where(x =>
						x.CodCurso == codigoDoCurso && x.Fase == fase)
				 join curso in this._db.Cursos.Where(x => x.Situacao == SaeSituacao.Ativo)
					 on profHabilita.CodCurso equals curso.Nseq
				 select new PkHabilitacaoProfessor
				 {
					 Fase = profHabilita.Fase,
					 CodigoDoCurso = profHabilita.CodCurso ?? 0,
					 CodigoDaMateria = profHabilita.CodMateria ?? 0,
					 CodigoDoProfessor = profHabilita.CodProf ?? 0,
					 DescricaoDoCurso = curso.Descricao,
					 NomeDaFase = curso.Nomedasfases
				 })
				.OrderBy(x => x.CodigoDoCurso)
				.ThenBy(x => x.Fase)
				.ThenBy(x => x.CodigoDoProfessor)
				.ToList();

			return new SaeResponseRepository<List<PkHabilitacaoProfessor>>(habilitacoes.Any(), habilitacoes);
		}
	}
}
