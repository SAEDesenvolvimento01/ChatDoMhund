class ChatController {
	constructor() {

	}

	async GetFotoDoUsuario() {
		let foto = sessionStorage.getItem("foto");
		if (!foto) {
			const response = new SaeResponse(await new SaeAjax({
				url: "/Usuario/GetImagemDoUsuario"
			}).Start());

			if (response.Status()) {
				foto = response.Content();
			} else {
				await response.Swal();
			}
		}

		return foto;
	}

	async GetConversas() {
		const response = new SaeResponse(await new SaeAjax({
			url: "/Chat/GetConversas"
		}).Start());

		return response;
	}
}