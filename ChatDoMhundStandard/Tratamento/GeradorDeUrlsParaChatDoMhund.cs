using System;
using System.Security.Cryptography;
using System.Text;

namespace ChatDoMhundStandard.Tratamento
{
	public class GeradorDeUrlsParaChatDoMhund
	{
		private readonly int _codigoDoCliente;
		private readonly int _codigoDoUsuarioLogado;
		private readonly string _tipoDeUsuarioLogado;
		private readonly string _origemDeRequest;
		private readonly int? _codigoDoAluno;
		private readonly string _tipoDeRelacaoComAluno;
		private readonly bool _rodandoComoLocalhost;

		/// <summary>
		/// Método utilizado para logar o usuário vindo de outro sistema
		/// </summary>
		/// <param name="codigoDoCliente">Código da escola</param>
		/// <param name="codigoDoUsuarioLogado">Código do usuário a ser logado</param>
		/// <param name="tipoDeUsuarioLogado">Tipo de usuário a ser logado</param>
		/// <param name="origemDeRequest">Origem do request (mhund, professus+ ou professus pro)</param>
		/// <param name="rodandoComoLocalhost">Rodando como localhost?</param>
		/// <returns></returns>
		public GeradorDeUrlsParaChatDoMhund(
			int codigoDoCliente,
			int codigoDoUsuarioLogado,
			string tipoDeUsuarioLogado,
			string origemDeRequest,
			bool rodandoComoLocalhost = false)
		{
			this._codigoDoCliente = codigoDoCliente;
			this._codigoDoUsuarioLogado = codigoDoUsuarioLogado;
			this._tipoDeUsuarioLogado = tipoDeUsuarioLogado;
			this._origemDeRequest = origemDeRequest;
			this._rodandoComoLocalhost = rodandoComoLocalhost;
		}

		/// <summary>
		/// Método utilizado para logar o usuário vindo de outro sistema
		/// </summary>
		/// <param name="codigoDoCliente">Código da escola</param>
		/// <param name="codigoDoUsuarioLogado">Código do usuário a ser logado</param>
		/// <param name="tipoDeUsuarioLogado">Tipo de usuário a ser logado</param>
		/// <param name="origemDeRequest">Origem do request (mhund, professus+ ou professus pro)</param>
		/// <param name="codigoDoAluno">Código do aluno (Preenchido quando logar como responsável)</param>
		/// <param name="tipoDeRelacaoComAluno">Tipo de relação com o aluno (Preenchido quando logar como responsável)</param>
		/// <param name="rodandoComoLocalhost">Rodando como localhost?</param>
		/// <returns></returns>
		public GeradorDeUrlsParaChatDoMhund(
			int codigoDoCliente,
			int codigoDoUsuarioLogado,
			string tipoDeUsuarioLogado,
			string origemDeRequest,
			int? codigoDoAluno,
			string tipoDeRelacaoComAluno,
			bool rodandoComoLocalhost = false)
		{
			this._codigoDoCliente = codigoDoCliente;
			this._codigoDoUsuarioLogado = codigoDoUsuarioLogado;
			this._tipoDeUsuarioLogado = tipoDeUsuarioLogado;
			this._origemDeRequest = origemDeRequest;
			this._codigoDoAluno = codigoDoAluno;
			this._tipoDeRelacaoComAluno = tipoDeRelacaoComAluno;
			this._rodandoComoLocalhost = rodandoComoLocalhost;
		}

		public Uri Gerar()
		{
			string domain;
			if (this._rodandoComoLocalhost)
			{
				domain = "http://localhost:61439";
			}
			else
			{
				domain = "http://chat.mhund.com.br";
			}

			string uriString;
			string hash = this.GetHash();

			if (this._tipoDeUsuarioLogado == TipoDeUsuarioDoChatTrata.Responsavel)
			{
				uriString = $"{domain}/Login/Auth?" +
							$"e={this._codigoDoCliente}&" +
							$"c={this._codigoDoUsuarioLogado}&" +
							$"t={this._tipoDeUsuarioLogado}&" +
							$"o={this._origemDeRequest}&" +
							$"h={hash}&" +
							$"ca={this._codigoDoAluno}&" +
							$"tr={this._tipoDeRelacaoComAluno}";
			}
			else
			{
				uriString = $"{domain}/Login/Auth?" +
							$"e={this._codigoDoCliente}&" +
							$"c={this._codigoDoUsuarioLogado}&" +
							$"t={this._tipoDeUsuarioLogado}&" +
							$"o={this._origemDeRequest}&" +
							$"h={hash}";
			}

			return new Uri(uriString);
		}

		private string GetHash()
		{
			string texto;

			if (this._tipoDeUsuarioLogado == TipoDeUsuarioDoChatTrata.Responsavel)
			{
				texto = $"{this._codigoDoCliente}" +
				        $"{this._codigoDoUsuarioLogado}" +
				        $"{this._tipoDeUsuarioLogado}" +
				        $"{this._origemDeRequest}" +
				        $"{this._codigoDoAluno}" +
				        $"{this._tipoDeRelacaoComAluno}";
			}
			else
			{
				texto = $"{this._codigoDoCliente}" +
				        $"{this._codigoDoUsuarioLogado}" +
				        $"{this._tipoDeUsuarioLogado}" +
				        $"{this._origemDeRequest}";
			}

			MD5 md5 = MD5.Create();
			byte[] inputBytes = Encoding.ASCII.GetBytes(texto);
			byte[] hash = md5.ComputeHash(inputBytes);

			StringBuilder sb = new StringBuilder();
			foreach (byte t in hash)
			{
				sb.Append(t.ToString("X2"));
			}

			return sb.ToString();
		}
	}
}
