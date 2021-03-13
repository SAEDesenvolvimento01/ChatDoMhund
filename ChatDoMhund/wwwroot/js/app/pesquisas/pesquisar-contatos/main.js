class PesquisarContatos {
	_callback;// = (response = new PesquisarContatosResponse()) => { };
	_modal;
	constructor({
		callback = (response = new PesquisarContatosResponse()) => { }
	}) {
		this._callback = callback;
	}

	async Start() {
		const response = new SaeResponse(await new SaeAjax({
			url: "/PesquisarContatos/Index",
			showLoading: true
		}).Start());

		if (response.Status()) {
			this._modal = new SaeMaterialModal({
				id: "modal-pesquisar-contatos",
				html: response.View()
			}).Create();

			$("#CursosEFases")
				.on("change", async () => {
					await this.AtualizarLista();
				})
				.select2({
				placeholder: "Selecione um curso",
				escapeMarkup: function (es) { return es; }
			});

			$(this.GetTiposDeUsuariosSelecionadosDoStorage()).each((i, tipo) => {
				const $item = $(`[tipo-de-usuario-para-filtrar="${tipo}"]`);

				if ($item.length) {
					this.SelecionouTipo($item);
				}
			});

			$("[tipo-de-usuario-para-filtrar]").on("click", async  event => {
				const $elemento = $(event.target);
				if ($elemento.is("[selecionado]")) {
					this.CancelouSelecaoDeTipo($elemento);
				} else {
					this.SelecionouTipo($elemento);
				}

				await this.AtualizarLista();
			});

			await this.AtualizarLista();
		} else {
			await response.Swal();
		}
	}

	SelecionouTipo($elemento) {
		$elemento.attr("selecionado", true);
		$elemento.addClass("gradient-45deg-purple-deep-orange gradient-shadow white-text");
		const tipo = $elemento.attr("tipo-de-usuario-para-filtrar");
		this.AdicionaTipoDeUsuarioSelecionadoNoStorage(tipo);
	}

	CancelouSelecaoDeTipo($elemento) {
		$elemento.removeAttr("selecionado");
		$elemento.removeClass("gradient-45deg-purple-deep-orange gradient-shadow white-text");
		const tipo = $elemento.attr("tipo-de-usuario-para-filtrar");
		this.RemoveTipoDeUsuarioSelecionadoNoStorage(tipo);
	}

	GetTiposDeUsuariosSelecionadosDoStorage() {
		const key = "tiposDeUsuariosSelecionados";
		
		let lista = localStorage.getItem(key);
		if (!lista) {
			lista = new Array();
		} else {
			lista = JSON.parse(lista);
		}

		return lista;
	}

	AdicionaTipoDeUsuarioSelecionadoNoStorage(tipo) {
		const key = "tiposDeUsuariosSelecionados";
		let lista = this.GetTiposDeUsuariosSelecionadosDoStorage();

		if(!lista.includes(tipo)) {
			lista.push(tipo);
		}

		lista = JSON.stringify(lista);
		localStorage.setItem(key, lista);
	}

	RemoveTipoDeUsuarioSelecionadoNoStorage(tipo) {
		const key = "tiposDeUsuariosSelecionados";
		let lista = localStorage.getItem(key);
		if (!lista) {
			lista = new Array();
		} else {
			lista = JSON.parse(lista);
		}

		if(lista.includes(tipo)) {
			const index = lista.indexOf(tipo);
			lista.splice(index);
		}

		lista = JSON.stringify(lista);
		localStorage.setItem(key, lista);
	}

	async AtualizarLista() {
		const form = {
			CursoEFase: $("#CursosEFases").val(),
			TiposSelecionados: new Array()
		};

		$("[tipo-de-usuario-para-filtrar][selecionado]").each((i, item) => {
			const tipo = $(item).attr("tipo-de-usuario-para-filtrar");
			form.TiposSelecionados.push(tipo);
		});

		const response = new SaeResponse(await new SaeAjax({
			type:"post",
			url: "/PesquisarContatos/AtualizarLista",
			data: form
		}).Start());

		if (response.Status()) {
			const $divLista = $("[lista-usuarios-para-conversar]");
			$divLista.html(response.View());

			$divLista.find("[selecionar-para-conversar]").on("click", event => {
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

				this._modal.Close();
			});
		} else {
			await response.Swal();
		}
	}
}