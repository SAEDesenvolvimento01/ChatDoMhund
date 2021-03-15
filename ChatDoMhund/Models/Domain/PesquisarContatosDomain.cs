using ChatDoMhund.Data.Repository;
using ChatDoMhund.Models.Infra;
using ChatDoMhund.Models.Poco;
using ChatDoMhund.Models.ViewModels;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Partials;
using HelperMhundStandard.Models.Dominio;
using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Handlers;
using HelperSaeStandard11.Models.Extension;
using HelperSaeStandard11.Models.Infra;
using HelperSaeStandard11.Models.Tratamento;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
			List<PkUsuarioConversa> listaParaRetorno = new List<PkUsuarioConversa>();
			this._usuarioLogado.GetUsuarioLogado();
			int codigoDoUsuarioLogado = this._usuarioLogado.Codigo;
			string tipoDeUsuarioLogado = this._usuarioLogado.TipoDeUsuario;
			int codigoDoCliente = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente).ConvertToInt32();
			this.ExtraiCodigoECursoSelecionado(index, out int codigoDoCurso, out string fase);
			List<PkUsuarioConversa> professores = new List<PkUsuarioConversa>();
			List<PkUsuarioConversa> coordenadores = new List<PkUsuarioConversa>();
			List<PkUsuarioConversa> alunos = new List<PkUsuarioConversa>();
			List<PkUsuarioConversa> responsaveis = new List<PkUsuarioConversa>();
			bool ehAluno = TipoDeUsuarioDoChatTrata.EhAluno(tipoDeUsuarioLogado);
			bool ehResponsavel = TipoDeUsuarioDoChatTrata.EhResponsavel(tipoDeUsuarioLogado);
			bool ehProfessor = TipoDeUsuarioDoChatTrata.EhProfessor(tipoDeUsuarioLogado);
			bool ehCoordenador = TipoDeUsuarioDoChatTrata.EhCoordenador(tipoDeUsuarioLogado);
			bool ehCoordenadorOuProfessor = TipoDeUsuarioDoChatTrata.EhCoordenadorOuProfessor(tipoDeUsuarioLogado);

			if (ehAluno)
			{
				if (this._usuarioLogado.Permissoes.ConversaComAluno &&
				    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Aluno))
				{
					PkHistoricoDoAluno ultimoHistoricoDoAluno = this.GetUltimoHistoricoDoAluno(codigoDoUsuarioLogado);
					if (ultimoHistoricoDoAluno != null)
					{
						alunos =
							this.GetAlunos(
									ultimoHistoricoDoAluno.CodigoDoCurso,
									ultimoHistoricoDoAluno.Fase,
									codigoDoCliente)
								.Where(x => x.Codigo != codigoDoUsuarioLogado)
								.ToList();
					}
				}
				if (this._usuarioLogado.Permissoes.ConversaComCoordenador &&
				    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Coordenador))
				{
					PkHistoricoDoAluno ultimoHistoricoDoAluno = this.GetUltimoHistoricoDoAluno(codigoDoUsuarioLogado);
					coordenadores = this.GetProfessoresOuCoordenadoresPeloHistorico(ultimoHistoricoDoAluno,
						TipoDeUsuarioDoChatTrata.Coordenador, codigoDoCliente);
				}
				if (this._usuarioLogado.Permissoes.ConversaComProfessor &&
				    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Professor))
				{
					PkHistoricoDoAluno ultimoHistoricoDoAluno = this.GetUltimoHistoricoDoAluno(codigoDoUsuarioLogado);
					professores = this.GetProfessoresOuCoordenadoresPeloHistorico(ultimoHistoricoDoAluno,
						TipoDeUsuarioDoChatTrata.Professor, codigoDoCliente);
				}
				if (this._usuarioLogado.Permissoes.ConversaComResponsavel &&
				    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Responsavel))
				{

				}
			}
			else if (ehResponsavel)
			{
				if (this._usuarioLogado.Permissoes.ConversaComAluno &&
				    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Aluno))
				{

				}
				if (this._usuarioLogado.Permissoes.ConversaComCoordenador &&
				    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Coordenador))
				{
					PkHistoricoDoAluno ultimoHistoricoDoAluno =
						this.GetUltimoHistoricoDoAluno(this._usuarioLogado.RelacaoComAluno.CodigoDoAluno);
					coordenadores = this.GetProfessoresOuCoordenadoresPeloHistorico(ultimoHistoricoDoAluno, TipoDeUsuarioDoChatTrata.Coordenador, codigoDoCliente);
				}
				if (this._usuarioLogado.Permissoes.ConversaComProfessor &&
				    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Professor))
				{
					PkHistoricoDoAluno ultimoHistoricoDoAluno =
						this.GetUltimoHistoricoDoAluno(this._usuarioLogado.RelacaoComAluno.CodigoDoAluno);
					professores = this.GetProfessoresOuCoordenadoresPeloHistorico(ultimoHistoricoDoAluno, TipoDeUsuarioDoChatTrata.Professor, codigoDoCliente);
				}
				if (this._usuarioLogado.Permissoes.ConversaComResponsavel &&
				    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Responsavel))
				{

				}
			}
			else if (ehCoordenadorOuProfessor)
			{
				if (this._usuarioLogado.Permissoes.ConversaComAluno &&
				    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Aluno))
				{
					alunos = this.GetAlunos(codigoDoCurso, fase, codigoDoCliente).ToList();
				}
				if (this._usuarioLogado.Permissoes.ConversaComCoordenador &&
				    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Coordenador))
				{

				}
				if (this._usuarioLogado.Permissoes.ConversaComProfessor &&
				    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Professor))
				{

				}
				if (this._usuarioLogado.Permissoes.ConversaComResponsavel &&
				    index.TiposSelecionados.Contains(TipoDeUsuarioDoChatTrata.Responsavel))
				{
					responsaveis = this.GetResponsaveisParaProfessorOuCoordenador(codigoDoCurso, fase, codigoDoCliente);
				}
			}

			listaParaRetorno.AddRange(professores);
			listaParaRetorno.AddRange(coordenadores);
			listaParaRetorno.AddRange(alunos);
			listaParaRetorno.AddRange(responsaveis);

			listaParaRetorno = listaParaRetorno
				.OrderBy(x => x.Nome)
				.ThenBy(x => x.Tipo)
				.ToList();

			return listaParaRetorno;
		}

		private void ExtraiCodigoECursoSelecionado(PesquisaContatosIndexModel index, out int codigoDoCurso, out string fase)
		{
			if (this._usuarioLogado.EhProfessorOuCoordenador())
			{
				string[] cursoEFase = index.CursoEFase.Split('-');
				codigoDoCurso = cursoEFase.FirstOrDefault().ConvertToInt32();
				fase = cursoEFase.LastOrDefault();
			}
			else
			{
				codigoDoCurso = 0;
				fase = string.Empty;
			}
		}

		private IQueryable<PkUsuarioConversa> GetAlunos(int codigoDoCurso, string fase, int codigoDoCliente)
		{
			return (from historicoDeOutroAluno in this._db.Histalu.Where(x =>
					x.Resultado == ResultadoCursos.Cursando &&
					x.Nseqc == codigoDoCurso &&
					x.Fase == fase)
					join outroAluno in this._db.Alunos
						on historicoDeOutroAluno.CodAluh equals outroAluno.Codigo
					join curso in this._db.Cursos.Where(x => x.Situacao == SaeSituacao.Ativo)
						on historicoDeOutroAluno.Nseqc equals curso.Nseq
					select new PkUsuarioConversa
					{
						Codigo = outroAluno.Codigo,
						Foto = outroAluno.Foto,
						Tipo = TipoDeUsuarioDoChatTrata.Aluno,
						Nome = outroAluno.Nome,
						Status = $"{TipoDeUsuarioDoChatTrata.AlunoExtenso} do curso: {curso.Descricao}",
						CodigoDoCliente = codigoDoCliente
					}).AsQueryable();
		}

		private List<PkUsuarioConversa> GetResponsaveisParaProfessorOuCoordenador(int codigoDoCurso, string fase, int codigoDoCliente)
		{
			List<PkUsuarioConversa> responsaveisParaRetorno = new List<PkUsuarioConversa>();
			var alunoComResponsaveis =
				(from historicoDoAluno in this._db.Histalu.Where(x =>
						x.Nseqc == codigoDoCurso && x.Fase == fase && x.Resultado == ResultadoCursos.Cursando)
				 join aluno in this._db.Alunos
					 on historicoDoAluno.CodAluh equals aluno.Codigo
				 join rf in this._db.Pessoas
					 on historicoDoAluno.CodRespfi equals rf.Codigo
					 into rfJoin
				 from responsavelFinanceiro in rfJoin.DefaultIfEmpty()
				 join p in this._db.Pessoas
					 on aluno.CodPai equals p.Codigo
					 into pJoin
				 from pai in pJoin.DefaultIfEmpty()
				 join m in this._db.Pessoas
					 on aluno.CodMae equals m.Codigo
					 into mJoin
				 from mae in mJoin.DefaultIfEmpty()
				 join rp in this._db.Pessoas
					 on aluno.CodResppe equals rp.Codigo
					 into rpJoin
				 from responsavelPedagogico in rpJoin.DefaultIfEmpty()
				 select new
				 {
					 codigoDoAluno = aluno.Codigo,
					 nomeDoAluno = aluno.Nome,
					 codigoDoResponsavelFinanceiro = (int?)responsavelFinanceiro.Codigo,
					 nomeDoResponsavelFinanceiro = responsavelFinanceiro.Nome,
					 fotoDoResponsavelFinanceiro = responsavelFinanceiro.Foto,
					 codigoDoPai = (int?)pai.Codigo,
					 nomeDoPai = pai.Nome,
					 fotoDoPai = pai.Foto,
					 codigoDaMae = (int?)mae.Codigo,
					 nomeDaMae = mae.Nome,
					 fotoDaMae = mae.Foto,
					 codigoDoResponsavelPedagogico = (int?)responsavelPedagogico.Codigo,
					 nomeDoResponsavelPedagogico = responsavelPedagogico.Nome,
					 fotoDoResponsavelPedagogico = responsavelPedagogico.Foto,
				 }).ToList();

			foreach (var aluno in alunoComResponsaveis)
			{
				string codigoEPrimeiroEUltimoNomeDoAluno = $"{aluno.codigoDoAluno} - {aluno.nomeDoAluno.GetPrimeiroEUltimoNome()}";

				List<PkUsuarioConversa> responsaveisDoAluno = new List<PkUsuarioConversa>();
				if (!SaeUtil.IsNullOrZero(aluno.codigoDoResponsavelFinanceiro))
				{
					PkUsuarioConversa responsavelFinanceiro = new PkUsuarioConversa
					{
						Codigo = aluno.codigoDoResponsavelFinanceiro ?? 0,
						Tipo = TipoDeUsuarioDoChatTrata.Responsavel,
						CodigoDoCliente = codigoDoCliente,
						Foto = aluno.fotoDoResponsavelFinanceiro,
						Nome = aluno.nomeDoResponsavelFinanceiro,
						Status = $"Resp. Fin. do(a) {codigoEPrimeiroEUltimoNomeDoAluno}"
					};
					responsaveisDoAluno.Add(responsavelFinanceiro);
				}

				if (!SaeUtil.IsNullOrZero(aluno.codigoDoPai))
				{
					PkUsuarioConversa pai = new PkUsuarioConversa
					{
						Codigo = aluno.codigoDoPai ?? 0,
						Tipo = TipoDeUsuarioDoChatTrata.Responsavel,
						CodigoDoCliente = codigoDoCliente,
						Foto = aluno.fotoDoPai,
						Nome = aluno.nomeDoPai,
						Status = $"Pai do(a) {codigoEPrimeiroEUltimoNomeDoAluno}"
					};
					responsaveisDoAluno.Add(pai);
				}

				if (!SaeUtil.IsNullOrZero(aluno.codigoDaMae))
				{
					PkUsuarioConversa mae = new PkUsuarioConversa
					{
						Codigo = aluno.codigoDaMae ?? 0,
						Tipo = TipoDeUsuarioDoChatTrata.Responsavel,
						CodigoDoCliente = codigoDoCliente,
						Foto = aluno.fotoDaMae,
						Nome = aluno.nomeDaMae,
						Status = $"Mãe do(a) {codigoEPrimeiroEUltimoNomeDoAluno}"
					};
					responsaveisDoAluno.Add(mae);
				}

				if (!SaeUtil.IsNullOrZero(aluno.codigoDoResponsavelPedagogico))
				{
					PkUsuarioConversa responsavelPedagogico = new PkUsuarioConversa
					{
						Codigo = aluno.codigoDoResponsavelPedagogico ?? 0,
						Tipo = TipoDeUsuarioDoChatTrata.Responsavel,
						CodigoDoCliente = codigoDoCliente,
						Foto = aluno.fotoDoResponsavelPedagogico,
						Nome = aluno.nomeDoResponsavelPedagogico,
						Status = $"Resp. Ped. do(a) {codigoEPrimeiroEUltimoNomeDoAluno}"
					};
					responsaveisDoAluno.Add(responsavelPedagogico);
				}

				if (responsaveisDoAluno.Any())
				{
					var responsaveisAgrupados = responsaveisDoAluno.GroupBy(x => x.Codigo,
						(codigo, responsaveisIguais) =>
						{
							//Guardei numa lista para evitar múltipla enumeração
							List<PkUsuarioConversa> responsaveisIguaisList = responsaveisIguais.ToList();
							var relacoes = responsaveisIguaisList
								.Select(x => x.Status.Split(" do(a)").FirstOrDefault());

							string relacoesJuntasESeparadasPorVirgula =
								string.Join(", ", relacoes);
							var primeiroIndice = responsaveisIguaisList.FirstOrDefault();
							string nomeDoAluno = primeiroIndice.Status.Split("do(a)").LastOrDefault();
							var responsavelTemporario = new PkUsuarioConversa
							{
								Codigo = codigo,
								Tipo = TipoDeUsuarioDoChatTrata.Responsavel,
								CodigoDoCliente = codigoDoCliente,
								Foto = primeiroIndice.Foto,
								Nome = primeiroIndice.Nome,
								Status = $"{relacoesJuntasESeparadasPorVirgula} do(a) {nomeDoAluno}"
							};

							return responsavelTemporario;
						}).ToList();

					responsaveisParaRetorno.AddRange(responsaveisAgrupados);
				}
			}

			return responsaveisParaRetorno;
		}

		private List<PkUsuarioConversa> GetProfessoresOuCoordenadoresPeloHistorico(PkHistoricoDoAluno ultimoHistoricoDoAluno, string tipo,
			int codigoDoCliente)
		{
			List<PkUsuarioConversa> coordenadores = new List<PkUsuarioConversa>();

			if (ultimoHistoricoDoAluno != null)
			{
				List<PkHabilitacaoProfessor> habilitacoes = this._habilitaRepository
					.GetHabilitacoesPeloHistorico(ultimoHistoricoDoAluno)
					.DistinctBy(x => x.CodigoDoProfessor)
					.ToList();

				string cargo = TipoDeUsuarioDoChatTrata.TipoExtenso(tipo);

				coordenadores = (from profHabilita in habilitacoes
								 join cadforps in this._db.Cadforps.Where(x =>
										 x.ProfNivel == tipo && x.Situacao == SaeSituacao.Ativo)
									 on profHabilita.CodigoDoProfessor equals cadforps.Codigo
								 join curso in this._db.Cursos.Where(x => x.Situacao == SaeSituacao.Ativo)
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
			}

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
