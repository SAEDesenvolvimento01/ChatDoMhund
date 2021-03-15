namespace ChatDoMhundStandard.Tratamento
{
	public class OrigemDeChatTrata
	{
		public const string OrigemMhund = "MH";
		public const string OrigemProfessusMais = "PM";
		public const string OrigemProfessusPro = "PP";

		public static string GetOrigemExtenso(string origem, string textoInicio = "")
		{
			if (origem == OrigemMhund)
			{
				return $"{textoInicio}Mhund";
			}
			if (origem == OrigemProfessusMais)
			{
				return $"{textoInicio}Professus+";
			}
			if (origem == OrigemProfessusPro)
			{
				return $"{textoInicio}Professus PRO";
			}

			return string.Empty;
		}

		public static string GetUrlOrigem(string origem)
		{
			//if (origem == OrigemProfessusMais)
			//{
			//	//location.href = "intent://scan/#Intent;scheme=br.com.saeinfo.professusandroidnew;package=br.com.saeinfo.professusandroidnew;end"

			//	return "br.com.saeinfo.professusandroidnew";
			//}

			//if (origem == OrigemProfessusPro)
			//{
			//	return "com.saeinfo.professusproapp";
			//}

			//if (origem == OrigemMhund)
			//{
			//	return "https://www.mhund.com.br";
			//}

			return string.Empty;
		}
	}
}
