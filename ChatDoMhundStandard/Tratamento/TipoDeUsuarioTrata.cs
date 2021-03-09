using System.Collections.Generic;

namespace ChatDoMhundStandard.Tratamento
{
    public class TipoDeUsuarioTrata
    {
        public const string Aluno = "AL";
        public const string Professor = "PR";
        public const string Coordenador = "CO";
        public const string Responsavel = "RE";

        public static string TipoExtenso(string tipo)
        {
            if (!string.IsNullOrEmpty(tipo))
            {
                if (tipo == Aluno)
                {
                    return "Aluno(a)";
                }
                if (tipo == Professor)
                {
                    return "Professor(a)";
                }
                if (tipo == Coordenador)
                {
                    return "Coordenador(a)";
                }
                if (tipo == Responsavel)
                {
                    return "Responsável";
                }
            }

            return string.Empty;
        }

        public static List<string> GetTipos()
        {
	        return new List<string>
	        {
		        Aluno,
		        Professor,
		        Coordenador,
		        Responsavel
	        };
        }
    }
}
