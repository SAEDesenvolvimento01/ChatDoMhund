var conversas = new Array();
$(() => {
    CarregarConversas();
});

$(".chat-list").on("click", "[conversar-com-usuario]", event => {
    const $pessoa = $(event.target).closest("[conversar-com-usuario]");
    const $chatContentArea = $(".chat-content-area");
    const codigo = parseInt($pessoa.attr("codigo"));
    const filter = $(conversas).filter((i,item) => {
        return item.codigo === codigo;
    });
    if(filter.length) {
        const conversa = filter.first()[0];
        $chatContentArea.find("[foto-da-conversa]").attr("src", conversa.foto);
        $chatContentArea.find("[nome-da-conversa]").html(conversa.nome);
        $chatContentArea.find("[status-da-conversa]").html(conversa.status);
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
            <div conversar-com-usuario codigo="${item.codigo}" tipo="${item.tipo}" codigo-da-escola="${item.codigoDaEscola}" class="chat-user animate fadeUp delay-1">
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
                    <span class="badge badge pill red">4</span>
                </div>
            </div>`);
    });
}

$(() => {
    InicializarChat();
});


function InicializarChat() {
    InicializarEventos();
    CorrigirTamanhoDoCorpoDoChat();
    CorrigirTamanhoDoHistoricoDeContatos();
    InicializarInputMensagem();
}

function InicializarEventos() {
    $(window).resize(() => {
        CorrigirTamanhoDoCorpoDoChat();
        CorrigirTamanhoDoHistoricoDeContatos();
    });

    $("#mensagem")
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

var observe;
if (window.attachEvent) {
    observe = function (element, event, handler) {
        element.attachEvent(`on${event}`, handler);
    };
}
else {
    observe = function (element, event, handler) {
        element.addEventListener(event, handler, false);
    };
}

function InicializarInputMensagem() {
    var text = document.getElementById('mensagem');
    function resize() {
        text.style.height = "auto";
        text.style.height = `${text.scrollHeight}px`;
    }
    /* 0-timeout to get the already changed text */
    function delayedResize() {
        window.setTimeout(resize, 0);
    }
    observe(text, 'change', resize);
    observe(text, 'cut', delayedResize);
    observe(text, 'paste', delayedResize);
    observe(text, 'drop', delayedResize);
    observe(text, 'keydown', delayedResize);

    text.focus();
    text.select();
    resize();
}
