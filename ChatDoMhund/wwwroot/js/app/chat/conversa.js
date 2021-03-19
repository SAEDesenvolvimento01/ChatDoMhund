class Conversa {
	nome = "";
	foto = "";
	status = "";
	codigo = 0;
	tipo = "";
	codigoDaEscola = 0;
	groupName = "";
	dataDaUltimaMensagem = "";
	mensagens = [];

	constructor(conversa) {
		if (conversa) {
			this.nome = conversa.nome;
			this.foto = conversa.foto;
			this.status = conversa.status;
			this.codigo = conversa.codigo;
			this.tipo = conversa.tipo;
			this.codigoDaEscola = conversa.codigoDaEscola;
			this.groupName = conversa.groupName;
			this.dataDaUltimaMensagem = conversa.dataDaUltimaMensagem;
			this.mensagens = conversa.mensagens;
		}
	}

	Build({ $conversa, conversas }) {
		if ($conversa && $mensagem.length) {
			this.nome = $conversa.find("[nome]").attr("nome");
			this.foto = $conversa
				.find("img")
				.attr("src");
			this.status = $conversa
				.find("[status]")
				.attr("status");
			this.codigo = parseInt($conversa.attr("codigo"));
			this.tipo = $conversa.attr("tipo");
			this.codigoDaEscola = $conversa.attr("codigo-da-escola");
			this.groupName = $conversa.attr("group-name");
			this.dataDaUltimaMensagem = $conversa
				.find("[data-da-ultima-mensagem]")
				.attr("data-da-ultima-mensagem");

		}

		if (conversas) {
			const conversa = conversas.find(x => x.groupName === this.groupName);
			this.mensagens = conversa.mensagens;
		}

		return this;
	}

	EstaSelecionada() {
		const $conversaNoSidebar = $GetConversaNoSidebar(this);

		return $conversaNoSidebar.hasClass("active");
	}
}