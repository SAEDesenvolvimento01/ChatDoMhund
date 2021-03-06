﻿using ChatDoMhund.Data.Repository.Abstract;
using ChatDoMhund.Models.Poco;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models;
using System.Collections.Generic;
using System.Linq;

namespace ChatDoMhund.Data.Repository
{
	public class PessoasRepository : RepositoryBase<Alunos>
	{
		public PessoasRepository(MhundDbContext db) : base(db)
		{
		}

		public SaeResponseRepository<PkUsuarioLogado> GetUsuarioParaLogar(int codigoDoUsuario)
		{
			PkUsuarioLogado usuario = this
				._db
				.Pessoas
				.Select(x => new PkUsuarioLogado
				{
					Codigo = x.Codigo,
					Nome = x.Nome,
					TipoDeUsuario = TipoDeUsuarioDoChatTrata.Responsavel
				})
				.FirstOrDefault(x => x.Codigo == codigoDoUsuario);

			return new SaeResponseRepository<PkUsuarioLogado>(usuario != null, usuario);
		}

		public SaeResponseRepository<byte[]> GetFoto(int codigo)
		{
			byte[] foto = this
				._db
				.Pessoas
				.Select(x => new { x.Codigo, x.Foto })
				.FirstOrDefault(x => x.Codigo == codigo)
				?.Foto;

			return new SaeResponseRepository<byte[]>(foto != null, foto);
		}

		public SaeResponseRepository<List<PkUsuarioConversa>> GetResponsaveisDasConversas(List<int> codigosDosResponsaveis)
		{
			List<PkUsuarioConversa> responsaveis = this._db
				.Pessoas
				.Where(x => codigosDosResponsaveis.Contains(x.Codigo))
				.Select(aluno => new PkUsuarioConversa
				{
					Nome = aluno.Nome,
					Foto = aluno.Foto,
					Status = TipoDeUsuarioDoChatTrata.ResponsavelExtenso,
					Codigo = aluno.Codigo,
					Tipo = TipoDeUsuarioDoChatTrata.Responsavel
				}).ToList();

			return new SaeResponseRepository<List<PkUsuarioConversa>>(true, responsaveis);
		}

		public SaeResponseRepository<PkUsuarioConversa> GetResponsavelParaConversa(int codigo)
		{
			PkUsuarioConversa responsaveis = this._db
				.Pessoas
				.Where(x => x.Codigo == codigo)
				.Select(aluno => new PkUsuarioConversa
				{
					Nome = aluno.Nome,
					Foto = aluno.Foto,
					Status = TipoDeUsuarioDoChatTrata.ResponsavelExtenso,
					Codigo = aluno.Codigo,
					Tipo = TipoDeUsuarioDoChatTrata.Responsavel
				})
				.FirstOrDefault();

			return new SaeResponseRepository<PkUsuarioConversa>(true, responsaveis);
		}
	}
}