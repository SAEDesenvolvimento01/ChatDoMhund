using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;

namespace ChatDoMhund.Models.Tratamento
{
	public class GroupBuilder
	{
		private readonly ISaeHelperCookie _saeHelperCookie;

		public GroupBuilder(ISaeHelperCookie saeHelperCookie)
		{
			this._saeHelperCookie = saeHelperCookie;
		}

		public string GetGroupName()
		{
			string codigoDoCliente = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente);
			string tipoDeUsuario = this._saeHelperCookie.GetCookie(ECookie.TipoUsuario);
			string codigoDoUsuario = this._saeHelperCookie.GetCookie(ECookie.CodigoDoUsuario);
			return GetGroupName(codigoDoCliente, tipoDeUsuario, codigoDoUsuario);
		}

		public string GetGroupName(string codigoDoCliente, string tipoDeUsuario, string codigoDoUsuario)
		{
			return $"{codigoDoCliente}-{tipoDeUsuario}-{codigoDoUsuario}";
		}
	}
}
