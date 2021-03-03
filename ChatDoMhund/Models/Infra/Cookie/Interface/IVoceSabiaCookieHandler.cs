namespace ChatDoMhund.Models.Infra.Cookie.Interface
{
    public interface IVoceSabiaCookieHandler
    {
        string GetCookie(string key);
        void SetCookie(string key, string value, int durationInMinutes = 20, bool httpOnly = true);
    }
}
