using ChatDoMhund.Data.Repository.Abstract;
using ChatDoMhund.Models.Poco;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models;
using System.Linq;
using ChatDoMhundStandard.Tratamento;

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
    }
}