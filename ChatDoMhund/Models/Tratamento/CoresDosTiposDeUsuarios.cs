using ChatDoMhundStandard.Tratamento;

namespace ChatDoMhund.Models.Tratamento
{
	public class CoresDosTiposDeUsuarios
	{
		public static string GetCor(string tipo)
		{
			string cor = string.Empty;
			if (tipo == TipoDeUsuarioDoChatTrata.Aluno)
			{
				cor = "gradient-45deg-amber-amber";
			}
			else if (tipo == TipoDeUsuarioDoChatTrata.Professor)
			{
				cor = "gradient-45deg-indigo-blue";
			}
			else if (tipo == TipoDeUsuarioDoChatTrata.Coordenador)
			{
				cor = "gradient-45deg-indigo-purple";
			}
			else if (tipo == TipoDeUsuarioDoChatTrata.Responsavel)
			{
				cor = "gradient-45deg-deep-orange-orange";
			}

			return cor;
		}
	}
}
