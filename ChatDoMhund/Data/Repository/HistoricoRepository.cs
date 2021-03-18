using ChatDoMhund.Data.Repository.Abstract;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models;
using System.Linq;

namespace ChatDoMhund.Data.Repository
{
	public class HistoricoRepository : RepositoryBase<Historico>
	{
		public HistoricoRepository(MhundDbContext db) : base(db)
		{
		}

		public SaeResponseRepository<string> GetAnoLetivo()
		{
			string anoLetivo = this._db.Historico.Select(x => x.AnoLetivo).FirstOrDefault();
			return new SaeResponseRepository<string>(!string.IsNullOrEmpty(anoLetivo), anoLetivo);
		}
	}
}