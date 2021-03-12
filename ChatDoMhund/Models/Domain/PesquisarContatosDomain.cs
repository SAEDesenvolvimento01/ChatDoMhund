using ChatDoMhund.Models.Infra;
using ChatDoMhund.Models.Poco;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Partials;
using HelperMhundStandard.Models.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Handlers;

namespace ChatDoMhund.Models.Domain
{
	public class PesquisarContatosDomain
	{
		private readonly MhundDbContext _db;
		private readonly UsuarioLogado _usuarioLogado;
		private readonly ISaeHelperCookie _saeHelperCookie;

		public PesquisarContatosDomain(MhundDbContext db,
			UsuarioLogado usuarioLogado,
			ISaeHelperCookie saeHelperCookie)
		{
			this._db = db;
			this._usuarioLogado = usuarioLogado;
			this._saeHelperCookie = saeHelperCookie;
		}

		public List<PkUsuarioConversa> Get()
		{
			List<PkUsuarioConversa> lista = new List<PkUsuarioConversa>();
			this._usuarioLogado.GetUsuarioLogado();
			int codigoDoUsuarioLogado = this._usuarioLogado.Codigo;
			string tipoDeUsuarioLogado = this._usuarioLogado.TipoDeUsuario;
			int codigoDoCliente = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente).ConvertToInt32();
			if (this._usuarioLogado.Permissoes.ConversaComAluno)
			{
				//todo fluxo responsavel
				//todo fluxo professor e coordenador (usam mesma regra)
				//todo fluxo aluno

				if (tipoDeUsuarioLogado == TipoDeUsuarioTrata.Aluno)
				{
					PkHistoricoDoAluno ultimoHistoricoDoAluno = this.GetUltimoHistoricoDoAluno(codigoDoUsuarioLogado);
					if (ultimoHistoricoDoAluno != null)
					{
						List<PkUsuarioConversa> alunos =
							(from historicoDeOutroAluno in this._db.Histalu.Where(x =>
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
								 Tipo = TipoDeUsuarioTrata.Aluno,
								 Nome = outroAluno.Nome,
								 Status = $"Curso: {curso.Descricao}",
								 CodigoDoCliente = codigoDoCliente
							 }).ToList();

						lista.AddRange(alunos);
					}
				}
			}

			if (this._usuarioLogado.Permissoes.ConversaComCoordenador)
			{
				//todo fluxo responsavel
				//todo fluxo professor e coordenador (usam mesma regra)
				//todo fluxo aluno
			}

			if (this._usuarioLogado.Permissoes.ConversaComProfessor)
			{
				//todo fluxo responsavel
				//todo fluxo professor e coordenador (usam mesma regra)
				//todo fluxo aluno
			}

			if (this._usuarioLogado.Permissoes.ConversaComResponsavel)
			{
				//todo fluxo responsavel
				//todo fluxo professor e coordenador (usam mesma regra)
				//todo fluxo aluno
			}

			return lista;
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
