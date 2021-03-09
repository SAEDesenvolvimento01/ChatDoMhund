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

	SetConversas(conversas = new Conversa()) {
		const jsonConversas = JSON.stringify(conversas);

		sessionStorage.setItem("conversas", jsonConversas);
	}

	AddMensagem(groupNameOrigem, groupNameDestino, message) {
		const listaConversas = this.GetConversas();

		if (listaConversas) {
			let groupName = "";
			const groupNameUsuarioLogado = $("#group-name-usuario-logado").val();

			//Pego o que não seja a origem
			if (groupNameOrigem !== groupNameUsuarioLogado) {
				groupName = groupNameOrigem;
			} else {
				groupName = groupNameDestino;
			}

			const conversa = listaConversas.find(x => x.groupName === groupName);

			if (!conversa.mensagens) {
				conversa.mensagens = new Array();
			}

			const mensagem = new Mensagem();
			mensagem.groupNameDestino = groupNameDestino;
			mensagem.groupNameOrigem = groupNameOrigem;
			mensagem.texto = message;

			conversa.mensagens.push(mensagem);

			this.SetConversas(listaConversas);
		} else {
			alert("Fluxo ausente");
		}
	}
}