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
				html: response.View(),
				overflowElementSelector: "#modal-pesquisar-contatos .modal-body [lista-usuarios-para-conversar]",
				maxHeight: 60
			});

			this._modal.Create();

			//espero um tempo antes de calcular as alturas. 
			//alguns elementos aumentam o tamanho no celular, porque quebram linha por falta de espaço
			await sleep(1000);
			const $modal = $("#modal-pesquisar-contatos");
			const alguraDoModalHeader = $modal
				.find(".modal-header")
				.outerHeight();
			const alturaDoModalFooter = $modal
				.find(".modal-footer")
				.outerHeight();
			const alturaDoFiltro = $modal.find("#filtro-pesquisa-usuarios")
				.outerHeight();

			const paddingTopAndBottomDaModalContent = 24 + 24;

			const espacoUtil = innerHeight -
				alguraDoModalHeader -
				alturaDoModalFooter -
				alturaDoFiltro -
				paddingTopAndBottomDaModalContent;

			$modal.find("[lista-usuarios-para-conversar]")
				.css("max-height", `${espacoUtil}px`);



			$("#CursoEFase")
				.on("change", async () => {
					await this.AtualizarLista(true);
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
		$elemento.addClass(`${cor} z-depth-3`);
		this.AdicionaTipoDeUsuarioSelecionadoNoStorage(tipo);
	}

	CancelouSelecaoDeTipo($elemento) {
		$elemento.removeAttr("selecionado");
		const tipo = $elemento.attr("tipo-de-usuario-para-filtrar");
		const cor = $(`#cor-${tipo}`).val()
		$elemento.removeClass(`${cor} z-depth-3`);
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

	async AtualizarLista(forcarAtualizacao = false) {
		const form = {
			CursoEFase: $("#CursoEFase").val(),
			TiposSelecionados: new Array()
		};

		$("[tipo-de-usuario-para-filtrar][selecionado]").each((i, item) => {
			const tipo = $(item).attr("tipo-de-usuario-para-filtrar");
			form.TiposSelecionados.push(tipo);
		});

		const $divLista = $("[lista-usuarios-para-conversar]");
		//$divLista.html("");

		//$("[selecionar-para-conversar]")
		//	.hide();

		if (form.TiposSelecionados.length) {
			let view = sessionStorage.getItem("viewContatos");
			if (view && !forcarAtualizacao) {
				if (!$divLista.html()) {
					$divLista.html(view);
				}
			}
			else {
				$divLista.html(new MaterialLoading().GetCircularLoading());
				const response = new SaeResponse(await new SaeAjax({
					type: "post",
					url: "/PesquisarContatos/AtualizarLista",
					data: form
				}).Start());

				if (response.Status()) {
					view = response.View();

					sessionStorage.setItem("viewContatos", view);

					$divLista.html(view);
				} else {
					await response.Swal();
				}
			}

			$divLista.find("[selecionar-para-conversar]")
				.on("click",
					async event => {
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
							if ($usuario.find("[nunca-esteve-online]")
								.length) {
								await new SaeMaterialSwal().Confirm({
									titulo: `${response.nome} ainda não usou o chat.`,
									mensagem: "Deseja mesmo enviar uma mensagem?",
									botaoConfirmar: "Continuar",
									callback: confirmou => {
										if (confirmou) {
											this._callback(response);
											this._modal.Close();
										}
									}
								});
							} else {
								this._callback(response);
								this._modal.Close();
							}
						}
					});

			const tipoProfessorCoordenador = "PR";

			$("[tipo-de-usuario-para-filtrar]")
				.each((i, item) => {
					const $tipo = $(item);
					const tipo = $tipo.attr("tipo-de-usuario-para-filtrar");

					let selector = "[selecionar-para-conversar]";
					if (tipo === tipoProfessorCoordenador) {
						selector += `[tipo-de-professor="${tipo}"]`
					} else {
						selector += `[tipo="${tipo}"]`;
					}
					if ($tipo.is("[selecionado]")) {
						$(selector)
							.show();
					}
					else {
						$(selector)
							.hide(600);
					}
				});
		} else {
			new MaterialToast({ html: "Selecione ao menos um tipo de usuário para buscar!" }).Show();
		}
	}
}