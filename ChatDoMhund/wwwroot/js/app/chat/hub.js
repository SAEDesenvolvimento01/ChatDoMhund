class Hub {
	_connection;
	_conversas;

	constructor() {
		this._connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
		this._conversas = new Conversas();
	}

	Inicializar(estaReconectando = false) {
		this.Start(estaReconectando);

		return this;
	}

	async Start(estaReconectando) {
		await this._connection
			.start()
			.then(async () => {
				await this.AddToGroup();
				this.ConfigurarReceiveMessage();
				this.ConfigurarOnDisconnected();
				if(!estaReconectando) {
					//Só rodo na primeira execução para não registrar várias vezes
					await this.ConfigurarEstouDigitando();
					await this.ConfigurarEstaDigitando();
				}
				await ConexaoEstabelecida(estaReconectando);
			})
			.catch((err) => {
				if (estaReconectando) {
					new MaterialToast({ html: "Falha ao tentar reconectar." }).Show();
					setTimeout(() => { ConexaoInterrompida(); }, 10000);
				}
				console.error(err);
			});

	}

	async AddToGroup() {
		await this._connection
			.invoke("AddToGroup")
			.catch((err) => {
				console.error(err);
			});
	}

	ConfigurarReceiveMessage() {
		this._connection.on("ReceiveMessage", async mensagem => {
			try {
				mensagem = await this._conversas.AddMensagem(mensagem);
				AtualizarConversa(mensagem);
			} catch (e) {
				console.error(e);
			}
		});
	}

	ConfigurarOnDisconnected() {
		this._connection.onclose(async () => {
			try {
				await ConexaoInterrompida();
			} catch (e) {
				console.error(e);
			}
		});
	}

	async ConfigurarEstouDigitando() {
		setInterval(async () => {
			if (estouDigitando) {
				//console.log("esta digitando");
				const conversa = new Conversa().Build({ $conversa: $GetConversaSelecionada() });
				
				await this._connection
					.invoke("EstouDigitando", conversa.groupName)
					.catch((err) => {
						return console.error(err);
					});
			} else {
				//console.log("nao esta digitando")
			}
		}, 2500);
	}

	async ConfigurarEstaDigitando() {
		this._connection.on("EstaDigitando", groupName => {
			try {
				EstaDigitando(groupName);
			} catch (e) {
				console.error(e);
			}
		});
	}

	async SendMessage(groupNameDestino, message) {
		await this._connection
			.invoke("SendMessage", groupNameDestino, message)
			.catch((err) => {
				return console.error(err);
			});
	}
}