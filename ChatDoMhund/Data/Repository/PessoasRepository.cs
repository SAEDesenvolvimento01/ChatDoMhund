using System.Collections.Generic;
using ChatDoMhund.Data.Repository.Abstract;
using ChatDoMhund.Models.Poco;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models;
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
                .Cadforps
                .Select(x => new PkUsuarioLogado
                {
                    Codigo = x.Codigo,
                    Nome = x.Nome,
                    TipoDeUsuario = TipoDeUsuarioTrata.Responsavel
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
			        Status = TipoDeUsuarioTrata.Responsavel,
			        Codigo = aluno.Codigo,
			        Tipo = TipoDeUsuarioTrata.Responsavel
		        }).ToList();

	        return new SaeResponseRepository<List<PkUsuarioConversa>>(true, responsaveis);
        }
    }
}