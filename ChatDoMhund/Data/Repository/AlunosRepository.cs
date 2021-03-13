using ChatDoMhund.Data.Repository.Abstract;
using ChatDoMhund.Models.Poco;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models;
using System.Collections.Generic;
using System.Linq;

namespace ChatDoMhund.Data.Repository
{
	public class AlunosRepository : RepositoryBase<Alunos>
	{
		public AlunosRepository(MhundDbContext db) : base(db)
		{
		}

		public SaeResponseRepository<PkUsuarioLogado> GetUsuarioParaLogar(int codigoDoUsuario)
		{
			PkUsuarioLogado usuario = this
				._db
				.Alunos
				.Select(x => new PkUsuarioLogado
				{
					Codigo = x.Codigo,
					Nome = x.Nome,
					TipoDeUsuario = TipoDeUsuarioDoChatTrata.Aluno
				})
				.FirstOrDefault(x => x.Codigo == codigoDoUsuario);

			return new SaeResponseRepository<PkUsuarioLogado>(usuario != null, usuario);
		}

		public SaeResponseRepository<byte[]> GetFoto(int codigo)
		{
			byte[] foto = this
				._db
				.Alunos
				.Select(x => new { x.Codigo, x.Foto })
				.FirstOrDefault(x => x.Codigo == codigo)
				?.Foto;

			return new SaeResponseRepository<byte[]>(foto != null, foto);
		}

		public SaeResponseRepository<List<PkUsuarioConversa>> GetAlunosDasConversas(List<int> codigosDosAlunos)
		{
			List<PkUsuarioConversa> alunos = this._db
				.Alunos
				.Where(x => codigosDosAlunos.Contains(x.Codigo))
				.Select(aluno => new PkUsuarioConversa
				{
					Nome = aluno.Nome,
					Foto = aluno.Foto,
					Status = TipoDeUsuarioDoChatTrata.Aluno,
					Codigo = aluno.Codigo,
					Tipo = TipoDeUsuarioDoChatTrata.Aluno
				}).ToList();

			return new SaeResponseRepository<List<PkUsuarioConversa>>(true, alunos);
		}
	}
}
