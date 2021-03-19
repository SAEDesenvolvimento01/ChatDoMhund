class ChatToast {
	constructor() {

	}

	NotificaMensagem(conversa = new Conversa()) {
		const mensagens = conversa.mensagens;
		if (mensagens.length) {
			const ultimaMensagem = mensagens[mensagens.length - 1];
			let texto = ultimaMensagem.texto;
			if (texto) {
				if (texto.length > 10) {
					texto = texto.subString(0, 9);
				}

				new MaterialToast({ html: `${conversa.nome}: ${texto}` }).Show();
			}
		}
	}

	Reconectando() {
		new MaterialToast({ html: "Reconectando..." }).Show();
	}

	Reconectado() {
		new MaterialToast({ html: "Conexão estabelecida novamente." }).Show();
	}

	FalhaAoReconectar() {
		new MaterialToast({ html: "Falha ao tentar reconectar." }).Show();
	}
}