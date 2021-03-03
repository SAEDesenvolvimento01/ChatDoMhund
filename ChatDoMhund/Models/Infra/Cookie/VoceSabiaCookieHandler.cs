using System;
using ChatDoMhund.Models.Infra.Cookie.Interface;
using Microsoft.AspNetCore.Http;

namespace ChatDoMhund.Models.Infra.Cookie
{
    public class VoceSabiaCookieHandler : IVoceSabiaCookieHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VoceSabiaCookieHandler(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public string GetCookie(string key)
        {
            bool haveValue = this._httpContextAccessor
                .HttpContext
                .Request
                .Cookies
                .TryGetValue(key, out string value);

            if (haveValue)
            {
                return value;
            }

            return string.Empty;
        }

        public void SetCookie(string cookieKey, string value, int durationInMinutes = 20, bool httpOnly = true)
        {
            this._httpContextAccessor.HttpContext.Response.Cookies.Append(cookieKey, value, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(durationInMinutes),
                HttpOnly = httpOnly
            });
        }
    }
}
