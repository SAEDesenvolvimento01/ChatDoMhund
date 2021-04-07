const groupNameUsuarioLogado = $("#group-name-usuario-logado").val();
const $mensagem = $("#mensagem");
let hub = new Hub().Inicializar();
const chatController = new ChatController();
const conversas = new Conversas();
const chatToast = new ChatToast();
var estouDigitando = false;
var timeOutReconexao = 1000;
var debugUsers = ["99123-AL-750", "99123-RE-1182", "99123-PR-1143"];

$(() => {
	CarregarConversas();
	InicializarChat();
	EsconderElementosIncompativeis();
	SetDebugMessages();
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

async function AtualizarConversa(mensagem = new Mensagem()) {
	let groupName;
	const usuarioLogadoQueEnviou = groupNameUsuarioLogado === mensagem.groupNameOrigem;
	if (usuarioLogadoQueEnviou) {
		groupName = mensagem.groupNameDestino;
	} else {
		groupName = mensagem.groupNameOrigem;
	}
	const $conversa = $GetConversasNoSidebar().filter(`[group-name="${groupName}"]`);
	//Se a conversa não for a primeira, removo e adiciono novamente para o primeiro lugar da lista
	if ($conversa.length && $GetConversasNoSidebar().first().attr("group-name") !== groupName) {
		const html = $conversa.clone();
		$conversa.remove();
		$(".chat-list").prepend(html);
	}

	const listaDeConversas = conversas.GetConversas();
	const conversa = listaDeConversas.find(x => x.groupName === groupName);
	if (conversa.EstaSelecionada()) {
		InserirMensagensNoChat(conversa);
		hub.AbriuConversa(conversa.groupName);
	} else {
		const conversa = listaDeConversas.find(x => x.groupName === groupName);
		if (mensagem.groupNameOrigem !== groupNameUsuarioLogado) {
			PlaySound("new-message");
		}

		//As vezes a pessoa recém apertou enter (e desencadeou o "estou digitando")
		//Assim, eu limpo isso quando recebo a mensagem. Se ela continuar digitando, recebemos novamente o invoke disso
		NaoEstaMaisDigitando(conversa);
		if (!usuarioLogadoQueEnviou) {
			chatToast.NotificaMensagem(conversa);
		}
	}

	await AtualizarListaDeConversas({});
}

async function SendMessage() {
	const $message = $(".message");
	const message = $message.val();
	if (message) {
		$message.val("");
		M.textareaAutoResize($mensagem);
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
			const $conversaSelecionada = $(event.target).closest("[conversar-com-usuario]");

			$conversaSelecionada.siblings().removeClass("active");
			$conversaSelecionada.addClass("active");

			const $chatContentArea = $(".chat-content-area");

			if ($chatContentArea.is(":visible")) {
				$chatContentArea.hide(300);
				await sleep(400);
			}

			const codigo = parseInt($conversaSelecionada.attr("codigo"));
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

				AtualizaScrollDaConversa();

				if (!isMobile.any() && !$mensagem.is("[disabled]")) {
					$mensagem.focus();
				}

				$conversaSelecionada.find("[novas-mensagens]")
					.attr("novas-mensagens", "0")
					.html("")
					.hide(600);

				await hub.AbriuConversa(conversa.groupName);
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
			const hours = ConverteToLocaleString(date.getHours());
			const minutes = ConverteToLocaleString(date.getMinutes());
			const data = `${date.toLocaleDateString()} às ${hours}:${minutes}`;

			const iconeLida = GetIconeSeMensagemEstaLida(mensagem);

			const $ultimaPessoaQueEnviou = $chats.find(".chat").last();
			if ($ultimaPessoaQueEnviou.is(`[group-name="${mensagem.groupNameOrigem}"]`)) {
				$ultimaPessoaQueEnviou
					.find(".chat-body")
					.append(`
            <div class="chat-text" mensagem id="${mensagem.id}">
                <p>
					<span>${mensagem.texto}</span>
					<br />
					<span class="${classeDeOrientacaoDaHora} data-mensagem" data-mensagem>${data}${iconeLida}</span>
				</p>
            </div>
`);
			} else {
				$chats.append(`
            <div class="${classes}" group-name="${mensagem.groupNameOrigem}">
                <div class="chat-avatar hide-on-small-only">
                    <a class="avatar">
                        <img src="${foto}" class="circle" alt="avatar">
                    </a>
                </div>
                <div class="chat-body">
                    <div class="chat-text" mensagem id="${mensagem.id}">
                        <p>
							<span>${mensagem.texto}</span>
							<br />
							<span class="${classeDeOrientacaoDaHora} data-mensagem" data-mensagem>${data}${iconeLida}</span>
						</p>
                    </div>
                </div>
            </div>
`);
			}
		});

	InicializaTooltip();

	AtualizaScrollDaConversa();
}

function AtualizaScrollDaConversa(pixelsEmRelacaoAoTopo) {
	if (!pixelsEmRelacaoAoTopo) {
		pixelsEmRelacaoAoTopo = $(".chat-area > .chats").height();
	}
	$(".chat-area").scrollTop(pixelsEmRelacaoAoTopo);
	//console.log(`Scroll atualizado para ${pixelsEmRelacaoAoTopo} px`);
}

async function CarregarConversas() {
	const listaDeConversas = new Array();
	const response = await chatController.GetConversas();

	if (response.Status()) {
		$(response.Content())
			.each((i, conversa) => {
				if (conversa) {
					listaDeConversas.push(new Conversa(conversa));
				}
			});

		conversas.SetConversas(listaDeConversas);

		await AtualizarListaDeConversas({
			ehOCarregamentoInicial: true
		});
	} else {
		await response.Swal();
	}
}

async function AtualizarListaDeConversas({ ehOCarregamentoInicial = false }) {
	const $chatList = $(".chat-list");
	const listaDeConversas = conversas.GetConversas();
	const $mensagemNenhumaConversa = $("#no-data-listed");

	let acao = "prepend";
	if (ehOCarregamentoInicial) {
		acao = "append";
	}

	if (listaDeConversas.length) {
		$(listaDeConversas)
			.each((i, conversa = new Conversa()) => {
				const $conversaNoSidebar = $GetConversaNoSidebar(conversa);
				if (!$conversaNoSidebar.length) {
					$chatList[acao](`
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

				AtualizaIconeDeMensagensNaoLidas({ conversa: conversa });
				AtualizaHoraDaUltimaMensagem({ conversa: conversa });
			});
		$mensagemNenhumaConversa
			.hide();
		$("#no-data-found")
			.hide();
	} else {
		$mensagemNenhumaConversa
			.show();
	}
}

function AtualizaHoraDaUltimaMensagem({ conversa: conversa }) {
	const $dataDaUltimaMensagem = $GetConversaNoSidebar(conversa)
		.find(`span[data-da-ultima-mensagem]`);

	$dataDaUltimaMensagem.attr("data-da-ultima-mensagem", conversa.dataDaUltimaMensagem).html(conversa.dataDaUltimaMensagem);
}

function AtualizaIconeDeMensagensNaoLidas({ conversa = new Conversa() }) {
	const mensagens = conversa.mensagens;
	let quantidadeDeMensagensNaoLidas;
	if (mensagens) {
		quantidadeDeMensagensNaoLidas = mensagens.filter(x => x.groupNameOrigem !== groupNameUsuarioLogado && !x.lida)
			.length;
	} else {
		quantidadeDeMensagensNaoLidas = 0;
	}
	const $quantidadeDeMensagensNaoLidas = $GetConversaNoSidebar(conversa)
		.find(`span[novas-mensagens]`);

	if (quantidadeDeMensagensNaoLidas) {
		if (quantidadeDeMensagensNaoLidas >= 10) {
			quantidadeDeMensagensNaoLidas = "9+";
		}
		$quantidadeDeMensagensNaoLidas
			.attr("novas-mensagens", quantidadeDeMensagensNaoLidas)
			.html(quantidadeDeMensagensNaoLidas)
			.show();
	} else {
		$quantidadeDeMensagensNaoLidas
			.attr("novas-mensagens", 0)
			.html("")
			.hide();
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
	//InicializarInputMensagem();
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

async function ConexaoInterrompida() {
	const $sendButton = $btnSendMessage;
	$mensagem.attr("disabled", "disabled");
	$sendButton.attr("disabled", "disabled");
	chatToast.Reconectando();
	await sleep(timeOutReconexao += 1000);
	const estaReconectando = true;
	hub = new Hub().Inicializar(estaReconectando);
}

function ConexaoEstabelecida(estaReconectando) {
	const $sendButton = $btnSendMessage;
	$mensagem.removeAttr("disabled");
	if ($mensagem.is(":visible")) {
		$mensagem.focus();
	}
	$sendButton.removeAttr("disabled");
	if (estaReconectando) {
		chatToast.Reconectado();
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
	await IniciarPesquisaDeContatos();
});

async function IniciarPesquisaDeContatos() {
	await new PesquisarContatos({
		callback: async response => {
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

			let $conversaComPessoaSelecionadaJaExistente = $(`.chat-list [group-name="${conversa.groupName}"]`);

			if (!$conversaComPessoaSelecionadaJaExistente.length) {
				conversas.AddConversa(conversa);

				AtualizarListaDeConversas({});

				$conversaComPessoaSelecionadaJaExistente = $(`.chat-list [group-name="${conversa.groupName}"]`);
			}

			$conversaComPessoaSelecionadaJaExistente.click();
		}
	}).Start();
}

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
		const estaDigitando = "Está digitando...";
		$GetConversaNoSidebar(conversa)
			.find("[status]")
			.html(estaDigitando);


		if (conversa.EstaSelecionada()) {
			const $statusNoChat = $(".chat-header [status-da-conversa]");
			$statusNoChat.html(`${$statusNoChat.attr("status-da-conversa")}: ${estaDigitando}`);
		}

		setTimeout(() => {
			NaoEstaMaisDigitando(conversa);
		}, 3500);
	}
}

function NaoEstaMaisDigitando(conversa) {
	const $statusNoSidebar = $GetConversaNoSidebar(conversa).find("[status]");

	$statusNoSidebar.html($statusNoSidebar.attr("status"));

	const $statusNoChat = $(".chat-header [status-da-conversa]");
	$statusNoChat.html($statusNoChat.attr("status-da-conversa"));
}

$("#chat-filter").on("keyup", () => {
	FiltrarChats();
});

function FiltrarChats() {
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
}

function LeuMensagens(
	groupNameConversaAberta,
	groupNameQueAbriuAConversa,
	mensagensLidas = new Array(new MensagemLida())) {
	let groupName;

	if (groupNameUsuarioLogado !== groupNameConversaAberta) {
		groupName = groupNameConversaAberta
	} else {
		groupName = groupNameQueAbriuAConversa;
	}

	const listaDeConversas = conversas
		.GetConversas();
	const conversa = listaDeConversas
		.find(x => x.groupName === groupName);

	const $conversaSelecionada = $GetConversaSelecionada();
	const conversaEstaAberta = $conversaSelecionada.attr("group-name") === groupName;

	if (conversa.mensagens && conversa.mensagens.length) {
		conversa
			.mensagens
			.filter(mensagem => mensagensLidas.some(x => x.id === mensagem.id))
			.forEach(mensagem => {
				const mensagemLida = mensagensLidas.find(x => x.id === mensagem.id);

				mensagem.lida = true;
				mensagem.dataDeLeitura = mensagemLida.dataDeLeitura;

				if (conversaEstaAberta) {
					const iconeLida = GetIconeSeMensagemEstaLida(mensagem);
					const $dataMensagem = $(`[mensagem][id="${mensagem.id}"]`)
						.find("[data-mensagem]");
					const htmlDaDataDaMensagem = $dataMensagem.html();

					if (iconeLida && !htmlDaDataDaMensagem.includes(iconeLida)) {
						$dataMensagem
							.html(`${htmlDaDataDaMensagem}${iconeLida}`)
							.show(600);
						InicializaTooltip();
					}
				}
			});

		AtualizaIconeDeMensagensNaoLidas({ conversa: conversa });
		AtualizaHoraDaUltimaMensagem({ conversa: conversa });
	}

	conversas.SetConversas(listaDeConversas);
}

function ConverteToLocaleString(input) {
	return input
		.toLocaleString("pt-BR",
			{
				minimumIntegerDigits: 2
			});
}

function GetIconeSeMensagemEstaLida(mensagem = new Mensagem()) {
	let iconeLida = "";
	if (mensagem.lida && mensagem.groupNameOrigem === groupNameUsuarioLogado) {
		if (mensagem.dataDeLeitura) {
			const dataDeLeitura = new Date(mensagem.dataDeLeitura);
			const horaDeLeitura = ConverteToLocaleString(dataDeLeitura.getHours());
			const minutosDeLeitura = ConverteToLocaleString(dataDeLeitura.getMinutes());
			const dataLeitura = `${dataDeLeitura.toLocaleDateString()} às ${horaDeLeitura}:${minutosDeLeitura}`;
			iconeLida =
				`<i class="material-icons tiny tooltipped" data-position="left" data-tooltip="Visualizado em ${dataLeitura}">check</i>`;
		}
	}

	return iconeLida;
}

function InicializaTooltip() {
	$(".tooltipped")
		.tooltip();
}

$(".chat-area").on("scroll", async e => {
	const div = e.target;
	//if (div.offsetHeight + div.scrollTop >= div.scrollHeight) {
	//	//scrolledToBottom(e);
	//}

	if (!div.scrollTop) {
		//console.log(`offsetHeight: ${div.offsetHeight}`);
		//console.log(`offsetHeight + ScrollTop: ${div.offsetHeight + div.scrollTop}`);
		//console.log(`scrollHeight: ${div.scrollHeight}`);
		//debugger;
		const $conversaSelecionada = $GetConversaSelecionada();
		const conversa = new Conversa().Build({ $conversa: $conversaSelecionada });
		if (!conversa.carregouTodasAsMensagens) {
			let primeiraMensagemAdicionadaNoChat = null;
			const response = await chatController.BuscarMaisMensagens({
				groupName: $conversaSelecionada.attr("group-name"),
				codigoDaPrimeiraMensagemNoChat: parseInt($("[mensagem]").first().attr("id"))
			});

			const listaDeMensagens = response.Content().mensagens;

			if (!listaDeMensagens.length) {
				$conversaSelecionada.attr("carregou-todas-as-conversas", true);
			}

			$(listaDeMensagens)
				.each(async (i, mensagem) => {
					const estaCarregandoMaisMensagens = true;
					mensagem = await conversas.AddMensagem(mensagem, estaCarregandoMaisMensagens);
					//await AtualizarConversa(mensagem, estaCarregandoMaisMensagens, div.scrollHeight);

					const $chats = $(".chats");
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
					const hours = ConverteToLocaleString(date.getHours());
					const minutes = ConverteToLocaleString(date.getMinutes());
					const data = `${date.toLocaleDateString()} às ${hours}:${minutes}`;

					const iconeLida = GetIconeSeMensagemEstaLida(mensagem);

					const $primeiraPessoaQueEnviou = $chats.find(".chat").first();
					if ($primeiraPessoaQueEnviou.is(`[group-name="${mensagem.groupNameOrigem}"]`)) {
						$primeiraPessoaQueEnviou
							.find(".chat-body")
							.prepend(`
            <div class="chat-text" mensagem id="${mensagem.id}">
                <p>
					<span>${mensagem.texto}</span>
					<br />
					<span class="${classeDeOrientacaoDaHora} data-mensagem" data-mensagem>${data}${iconeLida}</span>
				</p>
            </div>
`);
					} else {
						$chats.prepend(`
            <div class="${classes}" group-name="${mensagem.groupNameOrigem}">
                <div class="chat-avatar hide-on-small-only">
                    <a class="avatar">
                        <img src="${foto}" class="circle" alt="avatar">
                    </a>
                </div>
                <div class="chat-body">
                    <div class="chat-text" mensagem id="${mensagem.id}">
                        <p>
							<span>${mensagem.texto}</span>
							<br />
							<span class="${classeDeOrientacaoDaHora} data-mensagem" data-mensagem>${data}${iconeLida
							}</span>
						</p>
                    </div>
                </div>
            </div>
`);
					}
					if (!primeiraMensagemAdicionadaNoChat) {
						primeiraMensagemAdicionadaNoChat = $chats.find(".chat-text").first()[0];
					}
				});
			await sleep(100);
			InicializaTooltip();
			//AtualizaScrollDaConversa(100);
			//div.scrollTop = div.clientHeight;
			if (primeiraMensagemAdicionadaNoChat && primeiraMensagemAdicionadaNoChat.offsetTop) {
				if (top) {
					$(".chat-area").scrollTop(primeiraMensagemAdicionadaNoChat.offsetTop);
				}
			}
			//console.log(`scrollHeight depois: ${div.scrollHeight}`);
		}
	}
});


function SetDebugMessages() {
	try {
		if (debugUsers.includes(groupNameUsuarioLogado)) {
			window.onerror = function (message, source, lineno, colno, error) {
				const mensagem = `Message: "${message}", source: "${source}", line: "${lineno}", column: "${colno}", error: "${error}"`;
				try {
					new SaeMaterialSwal().Alert({
						mensagem: mensagem
					});
				} catch (e) {
					alert(mensagem);
				}
			};
		}
	} catch (e) {
		console.log(e);
	}
}

function EsconderElementosIncompativeis() {
	if (isMobile.iOS()) {
		$("div[div-titulo]").removeClass("s8").addClass("s12");
		$("div[div-fullscreen]").hide();
	}
}