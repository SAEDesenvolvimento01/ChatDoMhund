﻿using ChatDoMhundStandard.Tratamento;

namespace ChatDoMhund.Models.Tratamento
{
	public class CoresDosTiposDeUsuarios
	{
		public static string GetCor(string tipo)
		{
			string cor = string.Empty;
			if (tipo == TipoDeUsuarioDoChatTrata.Aluno)
			{
				cor = "#f1f8e9 light-green lighten-5";
			}
			else if (tipo == TipoDeUsuarioDoChatTrata.Professor)
			{
				cor = "gradient-45deg-indigo-blue white-text";
			}
			else if (tipo == TipoDeUsuarioDoChatTrata.Coordenador)
			{
				cor = "gradient-45deg-indigo-purple white-text";
			}
			else if (tipo == TipoDeUsuarioDoChatTrata.Responsavel)
			{
				cor = "gradient-45deg-light-blue-teal";
			}

			return cor;
		}
	}
}
