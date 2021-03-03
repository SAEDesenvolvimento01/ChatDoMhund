using ChatDoMhund.Data.Repository.Abstract;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models;
using HelperSaeStandard11.Models.Tratamento;
using System.Linq;

namespace ChatDoMhund.Data.Repository
{
    public class AppCfgRepository : RepositoryBase<AppCfg>
    {
        public AppCfgRepository(MhundDbContext db) : base(db)
        {
        }

        public SaeResponseRepository<AppCfg> GetFirstOrDefault()
        {
            AppCfg appCfg = this._db.AppCfg.FirstOrDefault();

            return new SaeResponseRepository<AppCfg>(appCfg != null, appCfg);
        }

        public bool EscolaUsaChat()
        {
            return this._db.AppCfg.Select(x => x.UsaConversas).FirstOrDefault() == SaeSituacao.Sim;
        }
    }
}