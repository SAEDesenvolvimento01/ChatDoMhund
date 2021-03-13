using ChatDoMhund.Controllers.Abstract;
using ChatDoMhund.Data.Repository;
using ChatDoMhundStandard.Tratamento;
using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Handlers;
using HelperSaeStandard11.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatDoMhund.Controllers
{
    public class UsuarioController : AbsController
    {
        private readonly AlunosRepository _alunosRepository;
        private readonly CadforpsRepository _cadforpsRepository;
        private readonly PessoasRepository _pessoasRepository;
        private readonly ISaeHelperCookie _saeHelperCookie;

        public UsuarioController(AlunosRepository alunosRepository,
            CadforpsRepository cadforpsRepository,
            PessoasRepository pessoasRepository,
            ISaeHelperCookie saeHelperCookie)
        {
            this._alunosRepository = alunosRepository;
            this._cadforpsRepository = cadforpsRepository;
            this._pessoasRepository = pessoasRepository;
            this._saeHelperCookie = saeHelperCookie;
        }

        public JsonResult GetImagemDoUsuario()
        {
            SaeResponseRepository<byte[]> responseRepository = new SaeResponseRepository<byte[]>(true, null);

            int codigoDoUsuario = this._saeHelperCookie.GetCookie(ECookie.CodigoDoUsuario).ConvertToInt32();
            string tipoDeUsuario = this._saeHelperCookie.GetCookie(ECookie.TipoUsuario);
            if (tipoDeUsuario == TipoDeUsuarioDoChatTrata.Aluno)
            {
                responseRepository = this._alunosRepository.GetFoto(codigoDoUsuario);
            }
            else if (tipoDeUsuario == TipoDeUsuarioDoChatTrata.Responsavel)
            {
                responseRepository = this._pessoasRepository.GetFoto(codigoDoUsuario);
            }
            else if (tipoDeUsuario == TipoDeUsuarioDoChatTrata.Coordenador || tipoDeUsuario == TipoDeUsuarioDoChatTrata.Professor)
            {
                responseRepository = this._cadforpsRepository.GetFoto(codigoDoUsuario);
            }

            string base64String = FotoTrata.ToBase64String(responseRepository.Content);
            return this.Json(new SaeResponse
            {
                Status = true,
                Content = base64String
            });
        }
    }
}