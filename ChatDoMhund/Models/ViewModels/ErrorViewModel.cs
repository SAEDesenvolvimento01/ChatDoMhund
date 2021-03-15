using ChatDoMhund.Models.Infra;
using ChatDoMhund.Models.ViewModels.Base;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace ChatDoMhund.Models.ViewModels
{
    public class ErrorViewModel : BaseViewModelIndex
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public int StatusCode { get; set; }
        public string Origem { get; }

        public ErrorViewModel(int statusCode, string error, string message, IHttpContextAccessor httpContextAccessor,
	        string origem)
        {
            this.StatusCode = statusCode;
            Origem = origem;
            this._httpContextAccessor = httpContextAccessor;

            if (error is null)
            {
                error = string.Empty;
            }

            if (!string.IsNullOrEmpty(message))
            {
                this.Title = message;
            }
            else
            {
                string errorLower = error.ToLower();
                if (statusCode == 404 || errorLower.Contains("not found"))
                {
                    this.Title = "Recurso não encontrado.";
                }
                else if (statusCode == 400 || errorLower.Contains("not allowed"))
                {
                    this.Title = "Acesso negado.";
                }
                else if (statusCode == 403 || errorLower.Contains("sua sessão expirou"))
                {
                    this.Title = "Sessão expirada";
                }
                else
                {
                    this.Title = "Erro interno no sistema.";
                }
            }
        }

        public IHtmlContent GetTitulo() => new HtmlString($"<b>{this.Title}</b>");

        public IHtmlContent GetMensagem()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (Util.EhDebug())
            {
                var feature = this._httpContextAccessor
                    .HttpContext
                    .Features
                    .Get<IExceptionHandlerFeature>();

                if (this._httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("Referer", out StringValues referer) ?? false)
                {
                    stringBuilder.Append($"<b>Referer</b> {referer}");
                }

                if (feature?.Error != null)
                {
                    stringBuilder.Append($"<b>Mensagem</b>: {feature.Error.Message}");

                    if (feature.Error.InnerException != null)
                    {
                        stringBuilder.Append($"</br> <b>InnerException</b>: {feature.Error.InnerException}");
                    }

                    if (feature.Error.StackTrace != null)
                    {
                        stringBuilder.Append($"</br> <b>StackTrace</b>: {feature.Error.StackTrace}");
                    }
                }
            }

            return new HtmlString(stringBuilder.ToString());
        }
    }
}
