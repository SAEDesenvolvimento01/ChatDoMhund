const groupNameUsuarioLogado = $("#group-name-usuario-logado").val();
const $mensagem = $("#mensagem");
var observe = (element, event, handler) => {
	if (window.attachEvent) {
		element.attachEvent(`on${event}`, handler);
	} else {
		element.addEventListener(event, handler, false);
	}
};

let hub = new Hub().Inicializar();
const chatController = new ChatController();
const conversas = new Conversas();

$(() => {
	CarregarConversas();
	InicializarChat();
});


$(".message").on("keydown", event => {
	if (event.keyCode === 13 && !event.shiftKey) {
		event.preventDefault();
		SendMessage();
	}
});

$("#sendButton").on("click", function (event) {
	event.preventDefault();
	SendMessage();
});

function AtualizarConversa() {
	const $conversaSelecionada = $GetConversaSelecionada();
	const groupNameDestino = $conversaSelecionada.attr("group-name");
	if ($conversaSelecionada.length) {
		const listaDeConversas = conversas.GetConversas();
		const conversa = listaDeConversas.find(x => x.groupName === groupNameDestino);
		InserirMensagensNoChat(conversa);
	} else {
		const $mensagensNovas = $conversaSelecionada.find("[novas-mensagens]");
		const mensagensNovas = parseInt($mensagensNovas.attr("novas-mensagens")) + 1;
		$mensagensNovas.attr("novas-mensagens", mensagensNovas);
		$mensagensNovas.html(mensagensNovas);
		PlaySound("new-message");
	}
}

async function SendMessage() {
	const $message = $(".message");
	const message = $message.val();
	if (message) {
		$message.val("");
		const $conversaSelecionada = $GetConversaSelecionada();
		const codigo = parseInt($conversaSelecionada.attr("codigo"));
		const tipo = $conversaSelecionada.attr("tipo");
		const codigoDaEscola = parseInt($conversaSelecionada.attr("codigo-da-escola"));
		const groupNameDestino = `${codigoDaEscola}-${tipo}-${codigo}`;
		const groupNameOrigem = $("#group-name-usuario-logado").val();

		await hub.SendMessage(groupNameDestino, message);

		//conversas.AddMensagem(groupNameOrigem, groupNameDestino, message);

		if (!isMobile.any()) {
			$message.focus();
		}
	}
}

function $GetConversaSelecionada() {
	return $("[conversar-com-usuario].active");
}

$(".chat-list")
	.on("click",
		"[conversar-com-usuario]",
		async event => {
			const $pessoa = $(event.target).closest("[conversar-com-usuario]");

			$pessoa.siblings().removeClass("active");
			$pessoa.addClass("active");

			const $chatContentArea = $(".chat-content-area");

			if ($chatContentArea.is(":visible")) {
				$chatContentArea.hide(300);
				await sleep(400);
			}

			const codigo = parseInt($pessoa.attr("codigo"));
			const listaDeConversas = conversas.GetConversas();
			const conversa = listaDeConversas.find(conversa => conversa.codigo === codigo);

			if (conversa) {
				$chatContentArea.find("[foto-da-conversa]").attr("src", conversa.foto);
				$chatContentArea.find("[nome-da-conversa]").html(conversa.nome);
				$chatContentArea.find("[status-da-conversa]").html(conversa.status);

				if ($("#chat-sidenav").hasClass("sidenav")) {
					$(".sidenav-trigger[data-target=\"chat-sidenav\"]").click();
				}

				InserirMensagensNoChat(conversa);

				if ($chatContentArea.is(":hidden")) {
					$chatContentArea.show(300);
					await sleep(400);
				}

				//if ($mensagem.attr("disabled")) {
				//	$mensagem.removeAttr("disabled");
				//}

				AtualizaScrollDaConversa();

				if (!isMobile.any() && !$mensagem.is("[disabled]")) {
					$mensagem.focus();
				}
			}
		});

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

	AtualizaScrollDaConversa();
}

function AtualizaScrollDaConversa() {
	$(".chat-area").scrollTop($(".chat-area > .chats").height());
}

async function CarregarConversas() {
	const listaDeConversas = new Array();
	const response = await chatController.GetConversas();

	if (response.Status()) {
		$(response.Content())
			.each((i, conversa) => {
				if (conversa) {
					listaDeConversas.push(conversa);
				}
			});

		conversas.SetConversas(listaDeConversas);

		await AtualizarListaDeConversas();
	} else {
		await response.Swal();
	}
}

async function AtualizarListaDeConversas() {
	const $chatList = $(".chat-list");
	$chatList.html("");
	const listaDeConversas = conversas.GetConversas();
	$(listaDeConversas).each((i, conversa) => {
		AdicionarConversaNaLista({
			$chatList: $chatList,
			conversa: conversa
		});
	});
}

function AdicionarConversaNaLista({
	$chatList = $(".chat-list"),
	conversa = new Conversa(),
	inserirNoInicio = false
}) {
	let funcao;

	if (inserirNoInicio) {
		funcao = "prepend";
	} else {
		funcao = "append";
	}

	$chatList[funcao](`
            <div conversar-com-usuario
				codigo="${conversa.codigo}"
				tipo="${conversa.tipo}"
				codigo-da-escola="${conversa.codigoDaEscola}"
				group-name="${conversa.groupName}"
				class="chat-user animate fadeUp hoverable delay-1">
	                <div class="user-section">
	                    <div class="row valign-wrapper">
	                        <div class="col s2 media-image online pr-0">
	                            <img src="${conversa.foto}" alt="" class="circle z-depth-2 responsive-img">
	                        </div>
	                        <div class="col s10">
	                            <p class="m-0 blue-grey-text text-darken-4 font-weight-700">${conversa.nome}</p>
	                            <p class="m-0 info-text">${conversa.status}</p>
	                        </div>
	                    </div>
	                </div>
	                <div class="info-section">
	                    <div class="star-timing">
	                        <div class="time">
	                            <span>${conversa.dataDaUltimaMensagem}</span>
	                        </div>
	                    </div>
	                    <span novas-mensagens="${1}" class="badge badge pill red"></span>
	                </div>
	        </div>`);
}

async function InicializarChat() {
	await CarregaImagemDoUsuarioLogado();
	InicializarInputMensagem();
}

async function CarregaImagemDoUsuarioLogado() {
	let foto = sessionStorage.getItem("foto");
	if (!foto) {
		foto = await chatController.GetFotoDoUsuario();
		sessionStorage.setItem("foto", foto);
	}

	$("[foto-do-usuario-logado]").attr("src", foto);
}

function InicializarInputMensagem() {
	var element = document.getElementById("mensagem");
	function resize() {
		element.style.minHeight = "64px";
		element.style.height = "auto";
		element.style.height = `${element.scrollHeight + 20}px`;
	}
	/* 0-timeout to get the already changed text */
	function delayedResize() {
		window.setTimeout(resize, 0);
	}
	observe(element, "change", resize);
	observe(element, "focus", resize);
	observe(element, "cut", delayedResize);
	observe(element, "paste", delayedResize);
	observe(element, "drop", delayedResize);
	observe(element, "keydown", delayedResize);

	if (!isMobile.any()) {
		element.focus();
		element.select();
	}
	resize();
}

async function ConexaoInterrompida() {
	const $mensagem = $("#mensagem");
	const $sendButton = $("#sendButton");
	$mensagem.attr("disabled", "disabled");
	$sendButton.attr("disabled", "disabled");
	new MaterialToast({ html: "Reconectando..." }).Show();
	await sleep(1000);
	const estaReconectando = true;
	hub = new Hub().Inicializar(estaReconectando);
}

function ConexaoEstabelecida(estaReconectando) {
	const $mensagem = $("#mensagem");
	const $sendButton = $("#sendButton");
	$mensagem.removeAttr("disabled");
	if ($mensagem.is(":visible")) {
		$mensagem.focus();
	}
	$sendButton.removeAttr("disabled");
	if (estaReconectando) {
		new MaterialToast({ html: "Conexão estabelecida novamente." }).Show();
	}
}

function PlaySound(soundObj) {
	try {
		const element = document.getElementById(soundObj);
		//element.muted = true;

		element.play();
	} catch (e) {
		console.error(e);
	}
}

$("[nova-conversa]").on("click", async () => {
	await new PesquisarContatos({
		callback: response => {
			const conversa = new Conversa();
			conversa.mensagens = null;
			conversa.codigo = response.codigo;
			conversa.groupName = response.groupName;
			conversa.tipo = response.tipo;
			conversa.codigoDaEscola = response.codigoDaEscola;
			conversa.dataDaUltimaMensagem = "agora";
			conversa.foto = response.foto;
			conversa.nome = response.nome;
			conversa.status = response.status;

			const $conversaComPessoaSelecionadaJaExistente = $(`.chat-list [group-name="${conversa.groupName}"]`);
			if (!$conversaComPessoaSelecionadaJaExistente.length) {
				conversas.AddConversa(conversa);

				AdicionarConversaNaLista({
					conversa: conversa,
					inserirNoInicio: true
				});

				$(`.chat-list [group-name="${conversa.groupName}"]`)
					.click();
			} else {
				$conversaComPessoaSelecionadaJaExistente.click();
			}
		}
	}).Start();
});