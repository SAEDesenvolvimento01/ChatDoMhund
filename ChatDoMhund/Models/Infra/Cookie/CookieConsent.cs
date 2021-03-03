using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace ChatDoMhund.Models.Infra.Cookie
{
    public class CookieConsent
    {
        private readonly ITrackingConsentFeature _consentFeature;

        public CookieConsent(HttpContext context)
        {
            this._consentFeature = context.Features.Get<ITrackingConsentFeature>();
        }

        public bool NeedShowBanner()
        {
            if (!this._consentFeature.CanTrack &&
                !this._consentFeature.HasConsent &&
                this._consentFeature.IsConsentNeeded)
            {
                return true;
            }

            return false;
        }

        public IHtmlContent GetConsentCookie()
        {
            return new HtmlString(this._consentFeature.CreateConsentCookie());
        }
    }
}
