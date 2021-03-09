using ChatDoMhund.Data.Repository;
using ChatDoMhund.Models.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Handlers;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

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
			await this.Groups.AddToGroupAsync(this.Context.ConnectionId, this._groupBuilder.GetGroupName());
		}

		public async Task RemoveFromGroup()
		{
			await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, this._groupBuilder.GetGroupName());
		}

		public async Task SendMessage(string groupNameDestino, string message)
		{
			string codigoDoCliente = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente);
			string tipoDeUsuarioOrigem = this._saeHelperCookie.GetCookie(ECookie.TipoUsuario);
			string codigoDoUsuarioOrigem = this._saeHelperCookie.GetCookie(ECookie.CodigoDoUsuario);
			var destino = groupNameDestino.Split("-");
			string tipoDeDestino = destino[1];
			int codigoDeDestino = destino[2].ConvertToInt32();
			var chatProfess = new ChatProfess
			{
				DtMensagem = DateTime.Now,
				IdDestino = codigoDeDestino,
				IdOrigem = codigoDoUsuarioOrigem.ConvertToInt32(),
				Lido = false,
				TextMens = message,
				TipoDestino = tipoDeDestino,
				TipoOrigem = tipoDeUsuarioOrigem
			};

			if (chatProfess.IsValid())
			{
				this._chatProfessRepository.Add(chatProfess);

				string groupNameOrigem =
					this._groupBuilder.GetGroupName(codigoDoCliente, tipoDeUsuarioOrigem, codigoDoUsuarioOrigem);
				await this
					.Clients
					.Groups(groupNameOrigem, groupNameDestino)
					.SendAsync("ReceiveMessage", groupNameOrigem, groupNameDestino, message);
			}
		}
	}
}
