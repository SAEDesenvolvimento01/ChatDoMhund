using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatDoMhund.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISaeHelperCookie _saeHelperCookie;

        public ChatHub(IHttpContextAccessor httpContextAccessor,
            ISaeHelperCookie saeHelperCookie)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._saeHelperCookie = saeHelperCookie;
        }

        public async Task AddToGroup()
        {
            string codigoDoCliente = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente);
            string tipoDeUsuario = this._saeHelperCookie.GetCookie(ECookie.TipoUsuario);
            string codigoDoUsuario = this._saeHelperCookie.GetCookie(ECookie.CodigoDoUsuario);
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, $"{codigoDoCliente}{tipoDeUsuario}{codigoDoUsuario}");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            string codigoDoCliente = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente);
            string tipoDeUsuario = this._saeHelperCookie.GetCookie(ECookie.TipoUsuario);
            string codigoDoUsuario = this._saeHelperCookie.GetCookie(ECookie.CodigoDoUsuario);
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, $"{codigoDoCliente}{tipoDeUsuario}{codigoDoUsuario}");
        }

        public async Task SendMessage(string groupName, string message)
        {
            await this.Clients.OthersInGroup(groupName).SendAsync("ReceiveMessage", message);
        }
    }
}
