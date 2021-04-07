using ChatDoMhundStandard.Tratamento;
using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Handlers;

namespace ChatDoMhund.Models.Tratamento
{
	public class GroupBuilder
	{
		private readonly ISaeHelperCookie _saeHelperCookie;

		public GroupBuilder(ISaeHelperCookie saeHelperCookie)
		{
			this._saeHelperCookie = saeHelperCookie;
		}

		public string BuildGroupName()
		{
			string codigoDoCliente = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente);
			string tipoDeUsuario = this._saeHelperCookie.GetCookie(ECookie.TipoUsuario);
			string codigoDoUsuario = this._saeHelperCookie.GetCookie(ECookie.CodigoDoUsuario);
			return BuildGroupName(codigoDoCliente, tipoDeUsuario, codigoDoUsuario);
		}

		public string BuildGroupName(string codigoDoCliente, string tipoDeUsuario, string codigoDoUsuario)
		{
			if (tipoDeUsuario == TipoDeUsuarioDoChatTrata.Coordenador)
			{
				tipoDeUsuario = TipoDeUsuarioDoChatTrata.Professor;
			}

			return $"{codigoDoCliente}-{tipoDeUsuario}-{codigoDoUsuario}";
		}

		public string BuildGroupName(int codigoDoCliente, string tipoDeUsuario, int codigoDoUsuario)
		{
			if (tipoDeUsuario == TipoDeUsuarioDoChatTrata.Coordenador)
			{
				tipoDeUsuario = TipoDeUsuarioDoChatTrata.Professor;
			}

			return $"{codigoDoCliente}-{tipoDeUsuario}-{codigoDoUsuario}";
		}

		public void DismantleGroupName(string groupName, out int codigoDoCliente, out string tipoDeUsuario, out int codigoDoUsuario)
		{
			string[] split = groupName.Split("-");
			codigoDoCliente = split[0].ConvertToInt32();
			tipoDeUsuario = split[1];
			codigoDoUsuario = split[2].ConvertToInt32();
		}
	}
}
