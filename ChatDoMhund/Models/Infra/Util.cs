namespace ChatDoMhund.Models.Infra
{
    public static class Util
    {
        public static bool EhDebug()
        {
#if DEBUG
            return true;
#endif

            return false;
        }
    }
}
