using System;
using ChatDoMhund.Controllers.Abstract;
using ChatDoMhund.Data.Repository;
using ChatDoMhund.Models.ViewModels;
using HelperMhundCore31.Data.Entity.Models;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ChatDoMhundStandard.Tratamento;
using HelperSaeCore31.Models.Enum;

namespace ChatDoMhund.Controllers
{
    public class ChatController : AbsController
    {
        private readonly ISaeHelperCookie _saeHelperCookie;
        private readonly AlunosRepository _alunosRepository;

        public ChatController(ISaeHelperCookie saeHelperCookie, AlunosRepository alunosRepository)
        {
            _saeHelperCookie = saeHelperCookie;
            _alunosRepository = alunosRepository;
        }

        public IActionResult Index()
        {
            return this.View("Index", new ChatIndexViewModel());
        }

        public JsonResult GetConversas()
        {
            var lista = new List<Alunos>
            {
                this._alunosRepository.GetById(924).Content, this._alunosRepository.GetById(750).Content
            };

            string codigoDaEscola = this._saeHelperCookie.GetCookie(ECookie.CodigoDoCliente);
            SaeResponse response = new SaeResponse
            {
                Status = true,
                Content = lista.Select(x => new
                {
                    nome = x.Nome,
                    foto = this.TratarFoto(x.Foto),
                    status = "teste",
                    codigo = x.Codigo,
                    tipo = TipoDeUsuarioTrata.Aluno,
                    codigoDaEscola = codigoDaEscola
                })
            };

            return this.Json(response);
        }

        private string TratarFoto(byte[] foto)
        {
            if (foto != null)
            {
                return $"data:image/png;base64,{Convert.ToBase64String(foto)}";
            }

            return "/image/no-user-image.gif";
        }
    }
}