using System.Collections.Generic;
using ChatDoMhund.Data.Repository.Abstract;
using ChatDoMhund.Models.Poco;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models;
using System.Linq;
using ChatDoMhundStandard.Tratamento;

namespace ChatDoMhund.Data.Repository
{
	public class CadforpsRepository : RepositoryBase<Cadforps>
	{
		public CadforpsRepository(MhundDbContext db) : base(db)
		{
		}

		public SaeResponseRepository<PkUsuarioLogado> GetUsuarioParaLogar(int codigoDoUsuario, string tipoDoUsuario)
		{
			PkUsuarioLogado usuario = this
				._db
				.Cadforps
				.Select(x => new PkUsuarioLogado
				{
					Codigo = x.Codigo,
					Nome = x.Nome,
					TipoDeUsuario = TipoDeUsuarioDoChatTrata.Professor//tipoDoUsuario
				})
				.FirstOrDefault(x => x.Codigo == codigoDoUsuario);

			return new SaeResponseRepository<PkUsuarioLogado>(usuario != null, usuario);
		}

		public SaeResponseRepository<byte[]> GetFoto(int codigo)
		{
			byte[] foto = this
				._db
				.Cadforps
				.Select(x => new { x.Codigo, x.Foto })
				.FirstOrDefault(x => x.Codigo == codigo)
				?.Foto;

			return new SaeResponseRepository<byte[]>(foto != null, foto);
		}

		public SaeResponseRepository<List<PkUsuarioConversa>> GetProfessoresECoordenadoresDasConversas(
			List<int> codigosDosCoordenadores, List<int> codigosDosProfessores)
		{
			List<PkUsuarioConversa> professoresECoordenadores = this._db
				.Cadforps
				.Where(x => x.Tipo == TipoDeUsuarioDoChatTrata.Professor &&
							(codigosDosCoordenadores.Contains(x.Codigo) || codigosDosProfessores.Contains(x.Codigo)))
				.Select(cadforps => new PkUsuarioConversa
				{
					Nome = cadforps.Nome,
					Foto = cadforps.Foto,
					Status = cadforps.Tipo,
					Codigo = cadforps.Codigo,
					Tipo = cadforps.Tipo
				}).ToList();

			return new SaeResponseRepository<List<PkUsuarioConversa>>(true, professoresECoordenadores);
		}

		public SaeResponseRepository<PkUsuarioConversa> GetProfessorOuCoordenadorParaConversa(int codigo)
		{
			PkUsuarioConversa professoresECoordenadores = this._db
				.Cadforps
				.Where(x => x.Tipo == TipoDeUsuarioDoChatTrata.Professor && x.Codigo == codigo)
				.Select(cadforps => new PkUsuarioConversa
				{
					Nome = cadforps.Nome,
					Foto = cadforps.Foto,
					Status = cadforps.Tipo,
					Codigo = cadforps.Codigo,
					Tipo = cadforps.Tipo
				})
				.FirstOrDefault();

			return new SaeResponseRepository<PkUsuarioConversa>(true, professoresECoordenadores);
		}
	}
}