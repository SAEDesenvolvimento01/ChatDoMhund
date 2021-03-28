using ChatDoMhund.Data.Repository;
using ChatDoMhundStandard.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeStandard11.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatDoMhundConsole
{
	class Program
	{
		static async Task Main(string[] args)
		{
			List<string> m = new List<string>
			{
				"oi",
				"teste",
				"testando",
				"ta ai?",
				"opa"
			};

			ChatProfessRepository chatProfessRepository = new ChatProfessRepository(new MhundDbContext(
					"User Id=user99123;Password=91921325R6D7;Host=179.127.4.38;Database=99123;Port=5433;Persist Security Info=True;"));
			for (int i = 0; i <= 10000; i++)
			{
				for (int j = 0; j <= 4; j++)
				{
					ChatProfess chatProfess = new ChatProfess
					{
						DtMensagem = DateTime.Now,
						IdDestino = 99999,
						IdOrigem = 99999,
						Lido = false,
						TextMens = m[j],
						TipoDestino = TipoDeUsuarioDoChatTrata.Aluno,
						TipoOrigem = TipoDeUsuarioDoChatTrata.Aluno,
						OrigemLcto = OrigemDeChatTrata.OrigemMhund
					};

					SaeResponseRepository<ChatProfess> saeResponseRepository = await chatProfessRepository.AddAsync(chatProfess);

					if (saeResponseRepository.Status)
					{
						Console.WriteLine(saeResponseRepository.Content);
					}
					else
					{

					}

					//m[i]
				}
			}
		}
	}
}
