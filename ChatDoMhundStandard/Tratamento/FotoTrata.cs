using System;

namespace ChatDoMhundStandard.Tratamento
{
    public class FotoTrata
    {
        public static string ToBase64String(byte[] foto, string retornoCasoSejaNull = "/image/no-user-image.gif")
        {
            if (foto != null)
            {
                return $"data:image/png;base64,{Convert.ToBase64String(foto)}";
            }

            return retornoCasoSejaNull;
        }
    }
}
