using ChatDoMhund.Controllers.Abstract;
using ChatDoMhund.Data.Repository;
using ChatDoMhund.Models.Infra;
using HelperMhundCore31;
using HelperSaeCore31.Models.Infra.Criptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using HelperSaeCore31.Models.Enum;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeStandard11.Handlers;
using Microsoft.AspNetCore.Http;

namespace ChatDoMhund.Controllers
{
	[AllowAnonymous]
	public class LoginController : AbsController
	{
		private readonly SaeCriptography _saeCriptography;
		private readonly UsuarioLogado _usuarioLogado;
		private readonly ConnectionManager _connectionManager;
		private readonly AppCfgRepository _appCfgRepository;
		private readonly ISaeHelperCookie _helperCookie;

		public LoginController(SaeCriptography saeCriptography,
			UsuarioLogado usuarioLogado,
			ConnectionManager connectionManager,
			AppCfgRepository appCfgRepository,
			ISaeHelperCookie helperCookie)
		{
			this._saeCriptography = saeCriptography;
			this._usuarioLogado = usuarioLogado;
			this._connectionManager = connectionManager;
			this._appCfgRepository = appCfgRepository;
			this._helperCookie = helperCookie;
		}

		//Aluno Gabriel (750) => http://chat.mhund.com.br/Login/Auth?e=99123&c=750&t=AL&o=MH&h=1C8AB2EB4AC2285F8028824878FD2884
		//Aluno Gabriel (750) => http://localhost:61439/Login/Auth?e=99123&c=750&t=AL&o=MH&h=1C8AB2EB4AC2285F8028824878FD2884

		//Professor Gabriel (1143) => http://chat.mhund.com.br/Login/Auth?e=99123&c=1143&t=PR&o=MH&h=845B2C6B43FEA5F4E8AB63479FD5CAFA
		//Professor Gabriel (1143) => http://localhost:61439/Login/Auth?e=99123&c=1143&t=PR&o=MH&h=845B2C6B43FEA5F4E8AB63479FD5CAFA

		/// <summary>
		/// Método utilizado para logar o usuário vindo de outro sistema
		/// </summary>
		/// <param name="e">Código da escola</param>
		/// <param name="c">Código do usuário a ser logado</param>
		/// <param name="t">Tipo de usuário a ser logado</param>
		/// <param name="o">Origem do request (mhund, professus+ ou professus pro)</param>
		/// <param name="h">Hash</param>
		/// <param name="ca">Código do aluno (Preenchido quando logar como responsável)</param>
		/// <param name="tr">Tipo de relação com o aluno (Preenchido quando logar como responsável)</param>
		/// <returns></returns>
		public IActionResult Auth(int e, int c, string t, string o, string h, int? ca = null, string tr = "")
		{
			this._usuarioLogado.Sair();
			int codigoDoAluno = ca.ConvertToInt32();
			string hash = this._usuarioLogado.GetHashUsuarioLogado(e, c, t, o, codigoDoAluno, tr);
			if (this._saeCriptography.Comparar(hash, h))
			{
				if (this._connectionManager.TryCreateConnectionString(codigoDoCliente: e, deveCriarCookie: true))
				{
					//return Ok("teste");
					//var teste = this.Response.Headers;
					//this._helperCookie.SetCookie(ECookie.CodigoDoCliente, e.ToString(),50);
					//var teste2 = this.Response.Headers;
					//var teste3 = this.GetCookieValueFromResponse(this.Response, ECookie.CodigoDoCliente.ToString());
					if (this._appCfgRepository.EscolaUsaChat())
					{
						if (this._usuarioLogado.SetUsuarioLogado(
							codigoDaEscola: e,
							codigoDoUsuario: c,
							tipoDoUsuario: t,
							origemDeChat: o,
							codigoDoAluno: codigoDoAluno,
							tipoDeRelacao: tr))
						{
							return this.RedirectToAction("Index", "Chat");
						}
					}
					else
					{
						return this.RedirectToAction("HandleError", "Error", new
						{
							@message = "A sua escola não usa o chat.",
							@code = 403
						});
					}
				}
			}

			return this.RedirectToAction("HandleError", "Error", new { @error = "not allowed", @code = 403 });
		}

		public IActionResult Sair()
		{
			this._usuarioLogado.Sair();

			//Ainda nâo existe uma tela para isso
			throw new NotImplementedException();
		}
	}
}