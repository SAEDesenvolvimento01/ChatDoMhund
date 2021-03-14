using ChatDoMhund.Data.Repository;
using ChatDoMhund.Models.Tratamento;
using HelperMhundCore31.Data.Entity.Models;
using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Handlers;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using ChatDoMhund.Models.Poco;

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

		public async Task RemoveFromGroup()
		{
			await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, this._groupBuilder.BuildGroupName());
		}

		public async Task SendMessage(string groupNameDestino, string message)
		{
			string codigoDoCliente = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente);
			string tipoDeUsuarioOrigem = this._saeHelperCookie.GetCookie(ECookie.TipoUsuario);
			string codigoDoUsuarioOrigem = this._saeHelperCookie.GetCookie(ECookie.CodigoDoUsuario);
			groupNameDestino.Split("-");
			this._groupBuilder.DismantleGroupName(groupNameDestino,
				out int codigoDoClienteDestino,
				out string tipoDoUsuarioDestino,
				out int codigoDoUsuarioDestino);

			var chatProfess = new ChatProfess
			{
				DtMensagem = DateTime.Now,
				IdDestino = codigoDoUsuarioDestino,
				IdOrigem = codigoDoUsuarioOrigem.ConvertToInt32(),
				Lido = false,
				TextMens = message,
				TipoDestino = tipoDoUsuarioDestino,
				TipoOrigem = tipoDeUsuarioOrigem
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
	}
}
