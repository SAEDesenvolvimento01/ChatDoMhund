using ChatDoMhund.Data.Repository.Abstract;
using ChatDoMhund.Models.Poco;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using System.Collections.Generic;
using System.Linq;

namespace ChatDoMhund.Data.Repository
{
	public class ProfHabilitaRepository : RepositoryBase<ProfHabilita>
	{
		public ProfHabilitaRepository(MhundDbContext db) : base(db)
		{
		}

		public List<PkHabilitacaoProfessor> GetHabilitacoesDoProfessorOuCoordenador(int codigoDoProfessor)
		{
			List<PkHabilitacaoProfessor> habilitacoes =
				(from profHabilita in this._db.ProfHabilita.Where(x => x.CodProf == codigoDoProfessor)
					join curso in this._db.Cursos
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
				.ToList();

			return habilitacoes;
		}

		public List<PkHabilitacaoProfessor> GetHabilitacoesPeloHistorico(PkHistoricoDoAluno historico)
		{
			List<PkHabilitacaoProfessor> habilitacoes = 
				(from profHabilita in this._db.ProfHabilita.Where(x => 
						x.CodCurso == historico.CodigoDoCurso && x.Fase == historico.Fase)
					join curso in this._db.Cursos
						on profHabilita.CodCurso equals curso.Nseq
					select  new PkHabilitacaoProfessor
					{
						Fase = profHabilita.Fase,
						CodigoDoCurso = profHabilita.CodCurso ?? 0,
						CodigoDaMateria = profHabilita.CodMateria ?? 0,
						CodigoDoProfessor = profHabilita.CodProf ?? 0,
						DescricaoDoCurso = curso.Descricao,
						NomeDaFase = curso.Nomedasfases
					})
				.ToList();

			return habilitacoes;
		}
	}
}
