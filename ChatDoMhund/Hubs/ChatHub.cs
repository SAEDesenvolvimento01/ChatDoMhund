using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using ChatDoMhund.Models.Tratamento;

namespace ChatDoMhund.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISaeHelperCookie _saeHelperCookie;
        private readonly GroupBuilder _groupBuilder;

        public ChatHub(IHttpContextAccessor httpContextAccessor,
            ISaeHelperCookie saeHelperCookie,
            GroupBuilder  groupBuilder)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._saeHelperCookie = saeHelperCookie;
            this._groupBuilder = groupBuilder;
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
	        string groupNameOrigem = this._groupBuilder.GetGroupName();
            await this
	            .Clients
	            .Groups(groupNameOrigem, groupNameDestino)
	            .SendAsync("ReceiveMessage", groupNameOrigem, groupNameDestino, message);
        }
    }
}
