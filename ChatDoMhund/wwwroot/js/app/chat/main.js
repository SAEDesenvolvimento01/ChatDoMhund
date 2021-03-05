const groupNameUsuarioLogado = $("#group-name-usuario-logado").val();
const $mensagem = $("#mensagem");
const mensagemExemplo = { groupNameOrigem: "", groupNameDestino: "", texto: "" };
const mensagensExemplo = [mensagemExemplo];
const conversasExemplo = [
	{
		nome: "",
		foto: "",
		status: "",
		codigo: 0,
		tipo: "",
		codigoDaEscola: 0,
		groupName: "",
		mensagens: mensagensExemplo
	}
];
var conversas = conversasExemplo;
var observe = (element, event, handler) => {
	if (window.attachEvent) {
		element.attachEvent(`on${event}`, handler);
	} else {
		element.addEventListener(event, handler, false);
	}
};
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

$(() => {
	connection.on("ReceiveMessage", (groupNameOrigem, groupNameDestino, message) => {
		const conversa = conversas.find(item => [groupNameOrigem, groupNameDestino].includes(item.groupName));
		if (conversa) {
			if (!conversa.mensagens) {
				conversa.mensagens = [];
			}

			conversa.mensagens.push({
				groupNameOrigem: groupNameOrigem,
				groupNameDestino: groupNameDestino,
				texto: message
			});

			AtualizarConversa(conversa);
		}
	});

	connection
		.start()
		.then(async () => {
			connection
				.invoke("AddToGroup")
				.catch((err) => {
					return console.error(err.toString());
				});
		})
		.catch((err) => {
			return console.error(err.toString());
		});

	$(".message").on("keydown", event => {
		if (event.keyCode === 13 && !event.shiftKey) {
			event.preventDefault();
			SendMessage();
		}
	})

	$("#sendButton").on("click", function (event) {
		event.preventDefault();
		SendMessage();
	});

	CarregarConversas();
	InicializarChat();
});

function AtualizarConversa(conversa) {
	const conversaSelecionada = GetConversaSelecionada();
	if (conversaSelecionada.length) {
		InserirMensagensNoChat(conversa);
	} else {
		const $conversa = $(`[conversar-com-usuario][group-name="${conversa.groupName}"]`);

		const $mensagensNovas = $conversa.find("[novas-mensagens]");
		const mensagensNovas = parseInt($mensagensNovas.attr("novas-mensagens")) + 1;
		$mensagensNovas.attr("novas-mensagens", mensagensNovas);
		$mensagensNovas.html(mensagensNovas);
	}
}

function InserirMensagensNoChat(conversa) {
	const $chats = $(".chats").html("");
	$(conversa.mensagens)
		.each((i, mensagem) => {
			let foto;
			const origemEhUsuarioLogado = groupNameUsuarioLogado === mensagem.groupNameOrigem;
			if (origemEhUsuarioLogado) {
				foto = sessionStorage.getItem("foto");
			} else {
				foto = conversa.foto;
			}
			const $ultimaPessoaQueEnviou = $chats.find(".chat").last();
			if ($ultimaPessoaQueEnviou.is(`[group-name="${mensagem.groupNameOrigem}"]`)) {
				$ultimaPessoaQueEnviou.find(".chat-body")
					.append(`
            <div class="chat-text">
                <p>${mensagem.texto}</p>
            </div>
`);
			} else {
				let classes = "chat";
				if (origemEhUsuarioLogado) {
					classes += " chat-right";
				}
				$chats.append(`
            <div class="${classes}" group-name="${mensagem.groupNameOrigem}">
                <div class="chat-avatar">
                    <a class="avatar">
                        <img src="${foto}" class="circle" alt="avatar">
                    </a>
                </div>
                <div class="chat-body">
                    <div class="chat-text">
                        <p>${mensagem.texto}</p>
                    </div>
                </div>
            </div>
`);
			}
		});
}

function SendMessage() {
	const $message = $(".message");
	const message = $message.val();
	if (message) {
		$message.val("");
		const $conversaSelecionada = GetConversaSelecionada();
		const codigo = parseInt($conversaSelecionada.attr("codigo"));
		const tipo = $conversaSelecionada.attr("tipo");
		const codigoDaEscola = parseInt($conversaSelecionada.attr("codigo-da-escola"));
		const groupNameDestino = `${codigoDaEscola}-${tipo}-${codigo}`;
		connection
			.invoke("SendMessage", groupNameDestino, message)
			.then(() => {
				if (!isMobile.any()) {
					$message.focus();
				}
			})
			.catch((err) => {
				return console.error(err.toString());
			});
	}
}

function GetConversaSelecionada() {
	return $("[conversar-com-usuario].active");
}

$(".chat-list").on("click", "[conversar-com-usuario]", event => {
	const $pessoa = $(event.target).closest("[conversar-com-usuario]");

	$pessoa.siblings().removeClass("active");
	$pessoa.addClass("active");

	const $chatContentArea = $(".chat-content-area");
	const codigo = parseInt($pessoa.attr("codigo"));
	const conversa = conversas.find(conversa => conversa.codigo === codigo);

	if (conversa) {
		$chatContentArea.find("[foto-da-conversa]").attr("src", conversa.foto);
		$chatContentArea.find("[nome-da-conversa]").html(conversa.nome);
		$chatContentArea.find("[status-da-conversa]").html(conversa.status);

		if ($("#chat-sidenav").hasClass("sidenav")) {
			$(".sidenav-trigger[data-target=\"chat-sidenav\"]").click();
		}

		$mensagem.removeAttr("disabled");

		if (!isMobile.any()) {
			$mensagem.focus();
		}

		InserirMensagensNoChat(conversa);
	}
})

async function CarregarConversas() {
	conversas = new Array();
	const response = new SaeResponse(await new SaeAjax({
		url: "/Chat/GetConversas"
	}).Start());

	if (response.Status()) {
		$(response.Content())
			.each((i, conversa) => {
				if (conversa) {
					conversas.push(conversa);
				}
			});

		await AtualizarListaDeConversas();
	} else {
		await response.Swal();
	}
}

async function AtualizarListaDeConversas() {
	const $chatList = $(".chat-list");
	$chatList.html("");
	$(conversas).each((i, item) => {
		$chatList.append(`
            <div conversar-com-usuario
				codigo="${item.codigo}"
				tipo="${item.tipo}"
				codigo-da-escola="${item.codigoDaEscola}"
				group-name="${item.groupName}"
				class="chat-user animate fadeUp delay-1">
	                <div class="user-section">
	                    <div class="row valign-wrapper">
	                        <div class="col s2 media-image online pr-0">
	                            <img src="${item.foto}" alt="" class="circle z-depth-2 responsive-img">
	                        </div>
	                        <div class="col s10">
	                            <p class="m-0 blue-grey-text text-darken-4 font-weight-700">${item.nome}</p>
	                            <p class="m-0 info-text">${item.status}</p>
	                        </div>
	                    </div>
	                </div>
	                <div class="info-section">
	                    <div class="star-timing">
	                        <div class="time">
	                            <span>2.38 pm</span>
	                        </div>
	                    </div>
	                    <span novas-mensagens="${0}" class="badge badge pill red"></span>
	                </div>
	        </div>`);
	});
}

function InicializarChat() {
	InicializarEventos();
	CorrigirTamanhoDoCorpoDoChat();
	CorrigirTamanhoDoHistoricoDeContatos();
	InicializarInputMensagem();
	CarregaImagemDoUsuarioLogado();
}

async function CarregaImagemDoUsuarioLogado() {
	let foto = sessionStorage.getItem("foto");
	if (!foto) {
		const response = new SaeResponse(await new SaeAjax({
			url: "/Usuario/GetImagemDoUsuario"
		}).Start());

		if (response.Status()) {
			foto = response.Content();
			sessionStorage.setItem("foto", foto);
		} else {
			await response.Swal();
		}
	}

	$("[foto-do-usuario-logado]").attr("src", foto);
}

function InicializarEventos() {
	$(window).resize(() => {
		CorrigirTamanhoDoCorpoDoChat();
		CorrigirTamanhoDoHistoricoDeContatos();
	});

	$mensagem
		.on("keyup",
			() => {
				CorrigirTamanhoDoCorpoDoChat();
			});
}

function CorrigirTamanhoDoHistoricoDeContatos() {
	const alturaUtilDaPagina = innerHeight;
	const alturaDaInfoDoUsuario = 66;
	const alturaDaPesquisa = 43 + 14 + 14;
	const somaDosElementosMenosOHistoricoDeContatos = alturaUtilDaPagina - (alturaDaInfoDoUsuario + alturaDaPesquisa);
	$(".chat-application .app-chat .chat-content .sidebar-content.sidebar-chat")
		.css("height", `${somaDosElementosMenosOHistoricoDeContatos}px`);
}

function CorrigirTamanhoDoCorpoDoChat() {
	const alturaUtilDaPagina = innerHeight;
	const posicaoDoTituloDaPaginaEmRelacaoAoTopoDaPagina = 35;
	const alturaDoTituloDaPagina = posicaoDoTituloDaPaginaEmRelacaoAoTopoDaPagina + 65;
	const alturaDoHeaderDoChat = $(".chat-header").outerHeight();
	const alturaDoFooterDoChat = $(".chat-footer").outerHeight();
	const somaDosElementosMenosOHistoricoDeMensagensDoChat = alturaUtilDaPagina - (alturaDoTituloDaPagina + alturaDoHeaderDoChat + alturaDoFooterDoChat);
	$(".chat-application .app-chat .chat-content .chat-content-area .chat-area")
		.css("height", `${somaDosElementosMenosOHistoricoDeMensagensDoChat}px`);
}

function InicializarInputMensagem() {
	var text = document.getElementById("mensagem");
	function resize() {
		text.style.height = "auto";
		text.style.height = `${text.scrollHeight + 20}px`;
	}
	/* 0-timeout to get the already changed text */
	function delayedResize() {
		window.setTimeout(resize, 0);
	}
	observe(text, "change", resize);
	observe(text, "cut", delayedResize);
	observe(text, "paste", delayedResize);
	observe(text, "drop", delayedResize);
	observe(text, "keydown", delayedResize);

	if (!isMobile.any()) {
		text.focus();
		text.select();
	}
	resize();
}

var isMobile = {
	Android: function () {
		return navigator.userAgent.match(/Android/i);
	},
	BlackBerry: function () {
		return navigator.userAgent.match(/BlackBerry/i);
	},
	iOS: function () {
		return navigator.userAgent.match(/iPhone|iPad|iPod/i);
	},
	Opera: function () {
		return navigator.userAgent.match(/Opera Mini/i);
	},
	Windows: function () {
		return navigator.userAgent.match(/IEMobile/i) || navigator.userAgent.match(/WPDesktop/i);
	},
	any: function () {
		return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
	}
};