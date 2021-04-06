class Conversas {
	constructor() {
	}

	GetConversas() {
		const conversasJson = sessionStorage.getItem("conversas");
		let conversas = new Array(new Conversa());

		if (conversasJson) {
			conversas = new Array();
			$(JSON.parse(conversasJson))
				.each((i, conversa) => {
					conversas.push(new Conversa(conversa));
				});
		}

		return conversas;
	}

	SetConversas(conversas = new Array(new Conversa())) {
		const jsonConversas = JSON.stringify(conversas);

		sessionStorage.setItem("conversas", jsonConversas);
	}

	AddConversa(conversa = new Conversa()) {
		const conversas = this.GetConversas();

		//Adiciona uma nova conversa no inicio da array
		conversas.unshift(conversa);

		this.SetConversas(conversas);
	}

	async AddMensagem(mensagem = new Mensagem(), estaCarregandoMaisMensagens = false) {
		const listaConversas = this.GetConversas();
		if (listaConversas) {
			let groupName = "";
			const groupNameUsuarioLogado = $("#group-name-usuario-logado").val();

			//Pego o que não seja a origem
			if (mensagem.groupNameOrigem !== groupNameUsuarioLogado) {
				groupName = mensagem.groupNameOrigem;
			} else {
				groupName = mensagem.groupNameDestino;
			}

			let conversa = listaConversas.find(x => x.groupName === groupName);

			if (conversa) {
				if (!conversa.mensagens) {
					conversa.mensagens = new Array();
				}

				let acao;
				if (estaCarregandoMaisMensagens) {
					acao = "unshift";
				} else {
					acao = "push";
				}

				conversa.mensagens[acao](mensagem);
				const date = new Date(mensagem.dataDaMensagem);

				const hours = ConverteToLocaleString(date.getHours());
				const minutes = ConverteToLocaleString(date.getMinutes());
				conversa.dataDaUltimaMensagem = `${hours}:${minutes}`;
			} else {
				const response = new SaeResponse(await new SaeAjax({
					url: "/Chat/GetUsuarioParaConversa",
					data: {
						groupName: groupName
					}
				}).Start());

				if (response.Status()) {
					conversa = response.Content();

					listaConversas.push(conversa);
				} else {
					await response.Swal();
				}
			}


			this.SetConversas(listaConversas);

			return mensagem;
		} else {
			alert("Fluxo ausente");
		}
	}
}