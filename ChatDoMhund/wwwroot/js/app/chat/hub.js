class Hub {
	_connection;
	_conversas;

	constructor() {
		this._connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
		this._conversas = new Conversas();
	}

	Inicializar() {
		this.Start();

		return this;
	}

	async Start() {
		await this._connection
			.start()
			.then(async () => {
				await this.AddToGroup();
				this.ConfigurarReceiveMessage();
			})
			.catch((err) => {
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
				this._conversas.AddMensagem(groupNameOrigem, groupNameDestino, message);
				AtualizarConversa();
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