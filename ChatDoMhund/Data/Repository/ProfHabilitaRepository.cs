using ChatDoMhund.Data.Repository.Abstract;
using ChatDoMhund.Models.Poco;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models;
using HelperSaeStandard11.Models.Tratamento;
using System.Collections.Generic;
using System.Linq;

namespace ChatDoMhund.Data.Repository
{
	public class ProfHabilitaRepository : RepositoryBase<ProfHabilita>
	{
		public ProfHabilitaRepository(MhundDbContext db) : base(db)
		{
		}

		public SaeResponseRepository<List<PkHabilitacaoProfessor>> GetHabilitacoes(int codigoDoProfessor, string anoLetivo)
		{
			const string faseRegular = "SAE";
			IQueryable<PkHabilitacaoProfessor> query =
				(from profHabilita in this._db.ProfHabilita.Where(x => x.CodProf == codigoDoProfessor)
				 join curso in this._db.Cursos.Where(x => x.Situacao == SaeSituacao.Ativo)
					 on profHabilita.CodCurso equals curso.Nseq
				 join faseCurso in this._db.FaseCurso
						 .Where(x => x.Fase == faseRegular || 
						             x.Fase != faseRegular && x.Situacao == SaeSituacao.Ativo)
					 on new
					 {
						 codigoDoCurso = profHabilita.CodCurso,
						 fase = profHabilita.Fase
					 } equals new
					 {
						 codigoDoCurso =(int?) faseCurso.Nseqcfase,
						 fase = faseCurso.Fase
					 }
				 select new PkHabilitacaoProfessor
				 {
					 Fase = profHabilita.Fase,
					 CodigoDoCurso = profHabilita.CodCurso ?? 0,
					 CodigoDaMateria = profHabilita.CodMateria ?? 0,
					 CodigoDoProfessor = profHabilita.CodProf ?? 0,
					 DescricaoDoCurso = curso.Descricao,
					 NomeDaFase = curso.Nomedasfases,
					 Ano = curso.Ano
				 }).AsQueryable();

			if (!string.IsNullOrEmpty(anoLetivo))
			{
				query = query.Where(x => (x.Fase == faseRegular && x.Ano == anoLetivo) || x.Fase != faseRegular);
			}

			List<PkHabilitacaoProfessor> habilitacoes =
				query
				.OrderBy(x => x.CodigoDoCurso)
				.ThenBy(x => x.Fase)
				.ThenBy(x => x.CodigoDoProfessor)
				.ToList();

			return new SaeResponseRepository<List<PkHabilitacaoProfessor>>(habilitacoes.Any(), habilitacoes);
		}

		public SaeResponseRepository<List<PkHabilitacaoProfessor>> GetHabilitacoes(PkHistoricoDoAluno historico)
		{
			return this.GetHabilitacoes(historico.CodigoDoCurso, historico.Fase, historico.Ano);
		}

		public SaeResponseRepository<List<PkHabilitacaoProfessor>> GetHabilitacoes(int codigoDoCurso, string fase, string anoLetivo)
		{
			const string faseRegular = "SAE";
			IQueryable<PkHabilitacaoProfessor> query =
				(from profHabilita in this._db.ProfHabilita.Where(x =>
					x.CodCurso == codigoDoCurso && x.Fase == fase)
				 join curso in this._db.Cursos.Where(x => x.Situacao == SaeSituacao.Ativo)
					 on profHabilita.CodCurso equals curso.Nseq
				 join faseCurso in this._db.FaseCurso
						 .Where(x => x.Fase == faseRegular || 
						             x.Fase != faseRegular && x.Situacao == SaeSituacao.Ativo)
					 on new
					 {
						 codigoDoCurso = profHabilita.CodCurso,
						 fase = profHabilita.Fase
					 } equals new
					 {
						 codigoDoCurso =(int?) faseCurso.Nseqcfase,
						 fase = faseCurso.Fase
					 }
				 select new PkHabilitacaoProfessor
				 {
					 Fase = profHabilita.Fase,
					 CodigoDoCurso = profHabilita.CodCurso ?? 0,
					 CodigoDaMateria = profHabilita.CodMateria ?? 0,
					 CodigoDoProfessor = profHabilita.CodProf ?? 0,
					 DescricaoDoCurso = curso.Descricao,
					 NomeDaFase = curso.Nomedasfases,
					 Ano = curso.Ano
				 }).AsQueryable();

			if (!string.IsNullOrEmpty(anoLetivo))
			{
				query = query.Where(x => (x.Fase == faseRegular && x.Ano == anoLetivo) || x.Fase != anoLetivo);
			}

			List<PkHabilitacaoProfessor> habilitacoes =
				query
				.OrderBy(x => x.CodigoDoCurso)
				.ThenBy(x => x.Fase)
				.ThenBy(x => x.CodigoDoProfessor)
				.ToList();

			return new SaeResponseRepository<List<PkHabilitacaoProfessor>>(habilitacoes.Any(), habilitacoes);
		}
	}
}
