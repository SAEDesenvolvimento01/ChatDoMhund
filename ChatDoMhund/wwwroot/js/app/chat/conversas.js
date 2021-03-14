class Conversas {
	constructor() {
	}

	GetConversas() {
		const conversasJson = sessionStorage.getItem("conversas");
		let conversas = new Array(new Conversa());

		if (conversasJson) {
			conversas = JSON.parse(conversasJson);
		}

		return conversas;
	}

	SetConversas(conversas = new Array(new Conversa())) {
		const jsonConversas = JSON.stringify(conversas);

		sessionStorage.setItem("conversas", jsonConversas);
	}

	AddConversa(conversa = new Conversa()) {
		const conversas = this.GetConversas();

		conversas.unshift(conversa);

		this.SetConversas(conversas);
	}

	async AddMensagem(mensagem = new Mensagem()) {
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

				conversa.mensagens.push(mensagem);
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