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
var estouDigitando = false;
var timeOutReconexao = 1000

$(() => {
	CarregarConversas();
	InicializarChat();
});

const enterKey = 13;
$(".message").on("keydown", event => {
	if (event.keyCode === enterKey && !event.shiftKey) {
		event.preventDefault();
		SendMessage();
	}
});

const $btnSendMessage = $("#sendButton");
$btnSendMessage.on("click", function (event) {
	event.preventDefault();
	SendMessage();
});

function AtualizarConversa(mensagem = new Mensagem()) {
	const $conversaSelecionada = $GetConversaSelecionada();
	const listaDeConversas = conversas.GetConversas();
	if ($conversaSelecionada.length) {
		const groupNameDestino = $conversaSelecionada.attr("group-name");
		const conversa = listaDeConversas.find(x => x.groupName === groupNameDestino);
		InserirMensagensNoChat(conversa);
	} else {
		const usuarioLogadoQueEnviou = groupNameUsuarioLogado === mensagem.groupNameOrigem;
		let groupName;
		if (usuarioLogadoQueEnviou) {
			groupName = mensagem.groupNameDestino;
		} else {
			groupName = mensagem.groupNameOrigem;
		}

		const conversa = listaDeConversas.find(x => x.groupName === groupName);

		const $mensagensNovas = $("[conversar-com-usuario].active").find("[novas-mensagens]");
		const mensagensNovas = parseInt($mensagensNovas.attr("novas-mensagens")) + 1;
		$mensagensNovas.attr("novas-mensagens", mensagensNovas);
		$mensagensNovas.html(mensagensNovas);

		PlaySound("new-message");

		//As vezes a pessoa recém apertou enter (e desencadeou o "estou digitando")
		//Assim, eu limpo isso quando recebo a mensagem. Se ela continuar digitando, recebemos novamente o invoke disso
		NaoEstaMaisDigitando(conversa);


		if (!usuarioLogadoQueEnviou) {
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
	}

	AtualizarListaDeConversas();
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
				$chatContentArea
					.find("[status-da-conversa]")
					.attr("status-da-conversa", conversa.status)
					.html(conversa.status);

				if ($("#chat-sidenav").hasClass("sidenav")) {
					FecharMenu();
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
			let classes = "chat";
			let classeDeOrientacaoDaHora;
			if (origemEhUsuarioLogado) {
				classes += " chat-right";
				classeDeOrientacaoDaHora = "right";
			} else {
				classeDeOrientacaoDaHora = "left";
			}
			const date = new Date(mensagem.dataDaMensagem);
			const hours = date.getHours().toLocaleString("pt-BR", {
				minimumIntegerDigits: 2
			});
			const minutes = date.getMinutes().toLocaleString("pt-BR", {
				minimumIntegerDigits: 2
			});
			const day = date.getDay().toLocaleString("pt-BR", {
				minimumIntegerDigits: 2
			});
			const month = date.getMonth().toLocaleString("pt-BR", {
				minimumIntegerDigits: 2
			});
			const data = `${day}/${month} às ${hours}:${minutes}`;

			const $ultimaPessoaQueEnviou = $chats.find(".chat").last();
			if ($ultimaPessoaQueEnviou.is(`[group-name="${mensagem.groupNameOrigem}"]`)) {
				$ultimaPessoaQueEnviou.find(".chat-body")
					.append(`
            <div class="chat-text">
                <p>
					<span>${mensagem.texto}</span>
					<br />
					<span class="${classeDeOrientacaoDaHora} data-mensagem">${data}</span>
				</p>
            </div>
`);
			} else {
				$chats.append(`
            <div class="${classes}" group-name="${mensagem.groupNameOrigem}">
                <div class="chat-avatar">
                    <a class="avatar">
                        <img src="${foto}" class="circle" alt="avatar">
                    </a>
                </div>
                <div class="chat-body">
                    <div class="chat-text">
                        <p>
							<span>${mensagem.texto}</span>
							<br />
							<span class="${classeDeOrientacaoDaHora} data-mensagem">${data}</span>
						</p>
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
	const listaDeConversas = conversas.GetConversas();
	const $mensagemNenhumaConversa = $("#no-data-listed");
	if (listaDeConversas.length) {
		$(listaDeConversas)
			.each((i, conversa = new Conversa()) => {
				if (!$GetConversaNoSidebar(conversa)
					.length) {
					$chatList.prepend(`
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
	                            <p class="m-0 blue-grey-text text-darken-4 font-weight-700" nome="${conversa
							.nome}">${conversa.nome}</p>
	                            <p class="m-0 info-text" status="${conversa.status}">${conversa.status}</p>
	                        </div>
	                    </div>
	                </div>
	                <div class="info-section">
	                    <div class="star-timing">
	                        <div class="time">
	                            <span data-da-ultima-mensagem="${conversa.dataDaUltimaMensagem}">${conversa
							.dataDaUltimaMensagem}</span>
	                        </div>
	                    </div>
	                    <span novas-mensagens="0" class="badge badge pill red" style="display: none;"></span>
	                </div>
	        </div>`);
				}

				let mensagens = conversa.mensagens;
				let quantidadeDeMensagensNaoLidas;
				if (mensagens) {
					quantidadeDeMensagensNaoLidas = mensagens.filter(x => !x.lida)
						.length;
				} else {
					quantidadeDeMensagensNaoLidas = 0;
				}
				const $quantidadeDeMensagensNaoLidas = $GetConversaNoSidebar(conversa)
					.find(`span[novas-mensagens]`);

				if (quantidadeDeMensagensNaoLidas) {
					$quantidadeDeMensagensNaoLidas
						.attr("novas-mensagens", quantidadeDeMensagensNaoLidas)
						.html(quantidadeDeMensagensNaoLidas)
						.show(600);
				} else {
					$quantidadeDeMensagensNaoLidas
						.attr("novas-mensagens", 0)
						.html("")
						.hide(600);
				}
			});
		$mensagemNenhumaConversa
			.hide(600);
		$("#no-data-found")
			.hide(600);
	} else {
		$mensagemNenhumaConversa
			.show(600);
	}
}

function $GetConversaNoSidebar(conversa = new Conversa()) {
	return $GetConversasNoSidebar().filter(`[group-name="${conversa.groupName}"]`);
}

function $GetConversasNoSidebar() {
	return $(".chat-list")
		.find("div[conversar-com-usuario]");
}

async function InicializarChat() {
	await CarregaImagemDoUsuarioLogado();
	InicializarInputMensagem();
	if (isMobile.any()) {
		$btnSendMessage.addClass("btn-floating").html("<i class=\"material-icons\">send</i>")
	} else {
		$btnSendMessage.addClass("border-round").html("Enviar")
	}
	if ($("#chat-sidenav").hasClass("sidenav")) {
		AbrirMenu();
	}
}

function AbrirMenu() {
	$(".sidenav").sidenav("open");
}

function FecharMenu() {
	$(".sidenav").sidenav("close");
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
	const $sendButton = $btnSendMessage;
	$mensagem.attr("disabled", "disabled");
	$sendButton.attr("disabled", "disabled");
	new MaterialToast({ html: "Reconectando..." }).Show();
	await sleep(timeOutReconexao += 1000);
	const estaReconectando = true;
	hub = new Hub().Inicializar(estaReconectando);
}

function ConexaoEstabelecida(estaReconectando) {
	const $mensagem = $("#mensagem");
	const $sendButton = $btnSendMessage;
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

				AtualizarListaDeConversas();

				$(`.chat-list [group-name="${conversa.groupName}"]`)
					.click();
			} else {
				$conversaComPessoaSelecionadaJaExistente.click();
			}
		}
	}).Start();
});

$mensagem.on("focus", () => {
	AtualizaScrollDaConversa();
}).on("keydown", event => {

	if (event.keyCode !== enterKey) {
		estouDigitando = true;
	} else {
		estouDigitando = false;
	}

	setTimeout(() => {
		estouDigitando = false;
	}, 2500);
});

function EstaDigitando(groupName) {
	const conversa = conversas.GetConversas().find(x => x.groupName === groupName);
	if (conversa) {
		$GetConversaNoSidebar(conversa)
			.find("[status]")
			.html("Está digitando...");

		setTimeout(() => {
			NaoEstaMaisDigitando(conversa);
		}, 3500);
	}
}

function NaoEstaMaisDigitando(conversa) {
	const $status = $GetConversaNoSidebar(conversa).find("[status]");

	$status.html($status.attr("status"));
}

$("#chat-filter").on("keyup", () => {
	const filtro = $("#chat-filter").val().toLowerCase();
	const listaDeConversas = conversas.GetConversas();
	let itensMostrados = listaDeConversas.length;
	$GetConversasNoSidebar().each((i, item) => {
		const $conversa = $(item);
		if (filtro) {
			const conversa = listaDeConversas.find(x => x.groupName === $conversa.attr("group-name"));
			if (conversa) {
				if (conversa.nome) {
					if (conversa.nome.toLowerCase().includes(filtro)) {
						$conversa.show(600);
					} else {
						$conversa.hide(600);
						itensMostrados--;
					}
				}
			}
		} else {
			$conversa.show(600);
		}
	});

	const $mensagemNenhumItemEncontrado = $("#no-data-found");
	if (itensMostrados) {
		$mensagemNenhumItemEncontrado
			.hide(600);
	} else {
		$mensagemNenhumItemEncontrado
			.show(600);
		$("#no-data-listed")
			.hide(600);
	}
});