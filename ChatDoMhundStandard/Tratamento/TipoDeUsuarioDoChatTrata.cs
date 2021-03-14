﻿using System.Collections.Generic;

namespace ChatDoMhundStandard.Tratamento
{
    public class TipoDeUsuarioDoChatTrata
    {
        public const string Aluno = "AL";
        public const string AlunoExtenso = "Aluno(a)";
        public const string Professor = "PR";
        public const string ProfessorExtenso = "Professor(a)";
        public const string Coordenador = "CO";
        public const string CoordenadorExtenso = "Coordenador(a)";
        public const string Responsavel = "RE";
        public const string ResponsavelExtenso = "Responsável";

        public static string TipoExtenso(string tipo)
        {
            if (!string.IsNullOrEmpty(tipo))
            {
                if (tipo == Aluno)
                {
                    return AlunoExtenso;
                }
                if (tipo == Professor)
                {
                    return ProfessorExtenso;
                }
                if (tipo == Coordenador)
                {
                    return CoordenadorExtenso;
                }
                if (tipo == Responsavel)
                {
                    return ResponsavelExtenso;
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