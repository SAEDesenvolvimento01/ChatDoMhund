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
				console.error(err.toString());
			});
	}

	ConfigurarReceiveMessage() {
		this._connection.on("ReceiveMessage", (groupNameOrigem, groupNameDestino, message) => {
			try {
				const mensagem = this._conversas.AddMensagem(groupNameOrigem, groupNameDestino, message);
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

	async SendMessage(groupNameDestino, message) {
		await this._connection
			.invoke("SendMessage", groupNameDestino, message)
			.catch((err) => {
				return console.error(err.toString());
			});
	}
}