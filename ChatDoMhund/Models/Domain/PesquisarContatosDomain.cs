using ChatDoMhund.Models.Infra;
using ChatDoMhund.Models.Poco;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Partials;
using HelperMhundStandard.Models.Dominio;
using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using ChatDoMhund.Data.Repository;
using ChatDoMhund.Models.ViewModels;
using HelperMhundCore31.Data.Entity.Models;
using HelperSaeStandard11.Models.Extension;

namespace ChatDoMhund.Models.Domain
{
	public class PesquisarContatosDomain
	{
		private readonly MhundDbContext _db;
		private readonly UsuarioLogado _usuarioLogado;
		private readonly ISaeHelperCookie _saeHelperCookie;
		private readonly ProfHabilitaRepository _habilitaRepository;

		public PesquisarContatosDomain(MhundDbContext db,
			UsuarioLogado usuarioLogado,
			ISaeHelperCookie saeHelperCookie,
			ProfHabilitaRepository habilitaRepository)
		{
			this._db = db;
			this._usuarioLogado = usuarioLogado;
			this._saeHelperCookie = saeHelperCookie;
			this._habilitaRepository = habilitaRepository;
		}

		public List<PkUsuarioConversa> Get(PesquisaContatosIndexModel index)
		{
			List<PkUsuarioConversa> lista = new List<PkUsuarioConversa>();
			this._usuarioLogado.GetUsuarioLogado();
			int codigoDoUsuarioLogado = this._usuarioLogado.Codigo;
			string tipoDeUsuarioLogado = this._usuarioLogado.TipoDeUsuario;
			int codigoDoCliente = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente).ConvertToInt32();
			List<PkUsuarioConversa> professores = new List<PkUsuarioConversa>();
			List<PkUsuarioConversa> coordenadores = new List<PkUsuarioConversa>();
			List<PkUsuarioConversa> alunos = new List<PkUsuarioConversa>();
			List<PkUsuarioConversa> responsaveis = new List<PkUsuarioConversa>();
			bool ehResponsavel = this._usuarioLogado.TipoDeUsuario == TipoDeUsuarioDoChatTrata.Responsavel;
			if (this._usuarioLogado.Permissoes.ConversaComAluno &&
			    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Aluno))
			{
				//todo fluxo responsavel
				//todo fluxo professor e coordenador (usam mesma regra)
				if (tipoDeUsuarioLogado == TipoDeUsuarioDoChatTrata.Aluno)
				{
					PkHistoricoDoAluno ultimoHistoricoDoAluno = this.GetUltimoHistoricoDoAluno(codigoDoUsuarioLogado);
					if (ultimoHistoricoDoAluno != null)
					{
						alunos = (from historicoDeOutroAluno in this._db.Histalu.Where(x =>
								x.Resultado == ResultadoCursos.Cursando &&
								x.Nseqc == ultimoHistoricoDoAluno.CodigoDoCurso &&
								x.Fase == ultimoHistoricoDoAluno.Fase &&
								x.CodAluh != codigoDoUsuarioLogado)
								  join outroAluno in this._db.Alunos
									  on historicoDeOutroAluno.CodAluh equals outroAluno.Codigo
								  join curso in this._db.Cursos
									  on historicoDeOutroAluno.Nseqc equals curso.Nseq
								  select new PkUsuarioConversa
								  {
									  Codigo = outroAluno.Codigo,
									  Foto = outroAluno.Foto,
									  Tipo = TipoDeUsuarioDoChatTrata.Aluno,
									  Nome = outroAluno.Nome,
									  Status = $"{TipoDeUsuarioDoChatTrata.Aluno} do curso: {curso.Descricao}",
									  CodigoDoCliente = codigoDoCliente
								  }).ToList();
					}
				}
			}

			if (this._usuarioLogado.Permissoes.ConversaComCoordenador &&
			    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Coordenador))
			{
				//todo fluxo responsavel
				//todo fluxo professor e coordenador (usam mesma regra)

				if (ehResponsavel)
				{
					PkHistoricoDoAluno ultimoHistoricoDoAluno =
						this.GetUltimoHistoricoDoAluno(this._usuarioLogado.RelacaoComAluno.CodigoDoAluno);
					coordenadores = this.GetProfessoresOuCoordenadoresPeloHistorico(ultimoHistoricoDoAluno, TipoDeUsuarioDoChatTrata.Coordenador, codigoDoCliente);
				}
				//todo fluxo aluno
			}

			if (this._usuarioLogado.Permissoes.ConversaComProfessor &&
			    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Professor))
			{
				//todo fluxo responsavel
				//todo fluxo professor e coordenador (usam mesma regra)
				if (ehResponsavel)
				{
					PkHistoricoDoAluno ultimoHistoricoDoAluno =
						this.GetUltimoHistoricoDoAluno(this._usuarioLogado.RelacaoComAluno.CodigoDoAluno);
					professores = this.GetProfessoresOuCoordenadoresPeloHistorico(ultimoHistoricoDoAluno, TipoDeUsuarioDoChatTrata.Professor, codigoDoCliente);
				}
				//todo fluxo aluno
			}

			if (this._usuarioLogado.Permissoes.ConversaComResponsavel &&
			    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Responsavel))
			{
				//todo fluxo responsavel
				//todo fluxo professor e coordenador (usam mesma regra)

				//List<PkHabilitacaoProfessor> habilitacoesDoProfessor = this._habilitaRepository.GetHabilitacoesDoProfessor(codigoDoUsuarioLogado);
				//(from )



				//todo fluxo aluno
			}

			lista.AddRange(professores);
			lista.AddRange(coordenadores);
			lista.AddRange(alunos);
			lista.AddRange(responsaveis);

			return lista;
		}

		private List<PkUsuarioConversa> GetProfessoresOuCoordenadoresPeloHistorico(PkHistoricoDoAluno ultimoHistoricoDoAluno, string tipo,
			int codigoDoCliente)
		{
			List<PkHabilitacaoProfessor> habilitacoes = this._habilitaRepository
				.GetHabilitacoesPeloHistorico(ultimoHistoricoDoAluno)
				.DistinctBy(x => x.CodigoDoProfessor)
				.ToList();

			string cargo = TipoDeUsuarioDoChatTrata.TipoExtenso(tipo);

			List<PkUsuarioConversa> coordenadores = (from profHabilita in habilitacoes
													 join cadforps in this._db.Cadforps.Where(x => x.ProfNivel == tipo)
														 on profHabilita.CodigoDoProfessor equals cadforps.Codigo
													 join curso in this._db.Cursos
														 on profHabilita.CodigoDoCurso equals curso.Nseq
													 select new PkUsuarioConversa
													 {
														 Codigo = cadforps.Codigo,
														 Tipo = TipoDeUsuarioDoChatTrata.Professor,
														 Foto = cadforps.Foto,
														 CodigoDoCliente = codigoDoCliente,
														 Nome = cadforps.Nome,
														 Status = $"{cargo} do curso: {curso.Descricao}"
													 }).ToList();
			return coordenadores;
		}

		private PkHistoricoDoAluno GetUltimoHistoricoDoAluno(int codigoDoAluno)
		{
			return (from aluno in this._db.Alunos.Where(x => x.Codigo == codigoDoAluno)
					join histalu in this._db.Histalu.Where(x => x.Resultado == ResultadoCursos.Cursando)
						on aluno.Codigo equals histalu.CodAluh
					select new PkHistoricoDoAluno
					{
						CodigoDoCurso = histalu.Nseqc ?? 0,
						CodigoDoAluno = histalu.CodAluh ?? 0,
						Fase = histalu.Fase,
						DataDeCadastro = histalu.DataCad ?? DateTime.MinValue
					})
				.OrderByDescending(x => x.DataDeCadastro)
				.FirstOrDefault();
		}
	}
}
