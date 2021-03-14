﻿class PesquisarContatos {
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
				html: response.View(),
				maxHeight: 60,
				overflowElementSelector: "#modal-pesquisar-contatos .modal-body [lista-usuarios-para-conversar]"
			});

			this._modal.Create();

			$("#CursoEFase")
				.on("change", async () => {
					await this.AtualizarLista();
				});

			$(this.GetTiposDeUsuariosSelecionadosDoStorage()).each((i, tipo) => {
				const $item = $(`[tipo-de-usuario-para-filtrar="${tipo}"]`);

				if ($item.length) {
					this.SelecionouTipo($item);
				}
			});

			$("[tipo-de-usuario-para-filtrar]").on("click", async event => {
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
		const tipo = $elemento.attr("tipo-de-usuario-para-filtrar");
		const cor = $(`#cor-${tipo}`).val()
		$elemento.addClass(`${cor} gradient-shadow white-text`);
		this.AdicionaTipoDeUsuarioSelecionadoNoStorage(tipo);
	}

	CancelouSelecaoDeTipo($elemento) {
		$elemento.removeAttr("selecionado");
		const tipo = $elemento.attr("tipo-de-usuario-para-filtrar");
		const cor = $(`#cor-${tipo}`).val()
		$elemento.removeClass(`${cor} gradient-shadow white-text`);
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

		if (!lista.includes(tipo)) {
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

		if (lista.includes(tipo)) {
			const index = lista.indexOf(tipo);
			lista.splice(index);
		}

		lista = JSON.stringify(lista);
		localStorage.setItem(key, lista);
	}

	async AtualizarLista() {
		const form = {
			CursoEFase: $("#CursoEFase").val(),
			TiposSelecionados: new Array()
		};

		$("[tipo-de-usuario-para-filtrar][selecionado]").each((i, item) => {
			const tipo = $(item).attr("tipo-de-usuario-para-filtrar");
			form.TiposSelecionados.push(tipo);
		});

		if (form.TiposSelecionados.length) {
			const $divLista = $("[lista-usuarios-para-conversar]");

			$divLista.html(new MaterialLoading().GetCircularLoading());

			const response = new SaeResponse(await new SaeAjax({
				type: "post",
				url: "/PesquisarContatos/AtualizarLista",
				data: form
			}).Start());

			if (response.Status()) {
				$divLista.html(response.View());

				$divLista.find("[selecionar-para-conversar]")
					.on("click",
						event => {
							const $usuario = $(event.target)
								.closest("[selecionar-para-conversar]");

							const response = new PesquisarContatosResponse();
							response.codigo = parseInt($usuario.attr("codigo"));
							response.groupName = $usuario.attr("group-name");
							response.tipo = $usuario.attr("tipo");
							response.codigoDaEscola = parseInt($usuario.attr("codigo-da-escola"));
							response.foto = $usuario.find("img[foto]")
								.attr("src");
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
		} else {
			new MaterialToast({ html: "Selecione ao menos um tipo de usuário para buscar!" }).Show();
		}
	}
}