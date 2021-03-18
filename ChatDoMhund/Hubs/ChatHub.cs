using ChatDoMhund.Data.Repository;
using ChatDoMhund.Models.Poco;
using ChatDoMhund.Models.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Handlers;
using HelperSaeStandard11.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatDoMhund.Models.Enum;

namespace ChatDoMhund.Hubs
{
	public class ChatHub : Hub
	{
		private readonly ISaeHelperCookie _saeHelperCookie;
		private readonly GroupBuilder _groupBuilder;
		private readonly ChatProfessRepository _chatProfessRepository;

		public ChatHub(ISaeHelperCookie saeHelperCookie,
			GroupBuilder groupBuilder,
			ChatProfessRepository chatProfessRepository)
		{
			this._saeHelperCookie = saeHelperCookie;
			this._groupBuilder = groupBuilder;
			this._chatProfessRepository = chatProfessRepository;
		}

		public async Task AddToGroup()
		{
			await this.Groups.AddToGroupAsync(this.Context.ConnectionId, this._groupBuilder.BuildGroupName());
		}

		//Não implementei porque não precisa, aparentemente: https://stackoverflow.com/questions/23854979/signalr-is-it-necessary-to-remove-the-connection-id-from-group-ondisconnect
		public async Task RemoveFromGroup()
		{
			await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, this._groupBuilder.BuildGroupName());
		}

		public async Task SendMessage(string groupNameDestino, string message)
		{
			string codigoDoCliente = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente);
			string tipoDeUsuarioOrigem = this._saeHelperCookie.GetCookie(ECookie.TipoUsuario);
			string codigoDoUsuarioOrigem = this._saeHelperCookie.GetCookie(ECookie.CodigoDoUsuario);
			string origem = this._saeHelperCookie.GetCookie(EChatCookie.OrigemDeChat.ToString());
			groupNameDestino.Split("-");
			this._groupBuilder.DismantleGroupName(groupNameDestino,
				out int codigoDoClienteDestino,
				out string tipoDoUsuarioDestino,
				out int codigoDoUsuarioDestino);

			ChatProfess chatProfess = new ChatProfess
			{
				DtMensagem = DateTime.Now,
				IdDestino = codigoDoUsuarioDestino,
				IdOrigem = codigoDoUsuarioOrigem.ConvertToInt32(),
				Lido = false,
				TextMens = message,
				TipoDestino = tipoDoUsuarioDestino,
				TipoOrigem = tipoDeUsuarioOrigem,
				OrigemLcto = origem
			};

			if (chatProfess.IsValid())
			{
				this._chatProfessRepository.Add(chatProfess);

				string groupNameOrigem =
					this._groupBuilder.BuildGroupName(codigoDoCliente, tipoDeUsuarioOrigem, codigoDoUsuarioOrigem);
				await this
					.Clients
					.Groups(groupNameOrigem, groupNameDestino)
					.SendAsync("ReceiveMessage",
						new PkMensagem(chatProfess, _groupBuilder, codigoDoCliente.ConvertToInt32()));
			}
		}

		public async Task EstouDigitando(string groupName)
		{
			await this
				.Clients
				.OthersInGroup(groupName)
				.SendAsync("EstaDigitando", this._groupBuilder.BuildGroupName());
		}

		public async Task AbriuConversa(string groupNameConversaAberta)
		{
			string groupNameQueAbriuAConversa = this._groupBuilder.BuildGroupName();

			this._groupBuilder.DismantleGroupName(groupNameQueAbriuAConversa,
				out int codigoDoClienteQueAbriu,
				out string tipoDeUsuarioQueAbriu,
				out int codigoDoUsuarioQueAbriu);

			this._groupBuilder.DismantleGroupName(groupNameConversaAberta,
				out int codigoDoClienteAberto,
				out string tipoDeUsuarioAberto,
				out int codigoDoUsuarioAberto);

			SaeResponseRepository<List<PkMensagemLida>> response = this
				._chatProfessRepository
				.LerMensagens(
				codigoDoUsuarioAberto,
				tipoDeUsuarioAberto,
				codigoDoUsuarioQueAbriu,
				tipoDeUsuarioQueAbriu);

			if (response.Status)
			{
				await this
					.Clients
					.Groups(groupNameQueAbriuAConversa, groupNameConversaAberta)
					.SendAsync("LeuMensagens",
						groupNameConversaAberta,
						groupNameQueAbriuAConversa,
						response.Content);
			}
		}
	}
}
