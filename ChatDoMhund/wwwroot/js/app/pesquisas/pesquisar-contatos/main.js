class PesquisarContatos {
	_callback;// = (response = new PesquisarContatosResponse()) => { };
	constructor({
		callback = (response = new PesquisarContatosResponse()) => { }
	}) {
		this._callback = callback;
	}

	async Start() {
		const response = new SaeResponse(await new SaeAjax({
			url: "/PesquisarContatos/Index"
		}).Start());

		if (response.Status()) {
			const modal = new SaeMaterialModal({
				id: "modal-pesquisar-contatos",
				html: response.View()
			}).Create();

			$("[selecionar-para-conversar]").on("click", event => {
				const $usuario = $(event.target)
					.closest("[selecionar-para-conversar]");

				const response = new PesquisarContatosResponse();
				response.codigo = parseInt($usuario.attr("codigo"));
				response.groupName = $usuario.attr("group-name");
				response.tipo = $usuario.attr("tipo");
				response.codigoDaEscola = parseInt($usuario.attr("codigo-da-escola"));
				response.foto = $usuario.find("img[foto]").attr("src");
				response.nome = $usuario.attr("nome");
				response.status = $usuario.attr("status");

				if (this._callback) {
					this._callback(response);
				}

				modal.Close();
			});
		} else {
			await response.Swal();
		}
	}
}