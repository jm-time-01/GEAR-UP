
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CallPopupUI : MonoBehaviour
{
    [Header("Sistemas")]
    public CallGenerator callGenerator;
    public PhoneController phoneController;

    [Header("Tempo de exibição do chamado")]
    [Min(1f)]
    public float tempoExibicao = 8f;

    [Header("Popup 1 - Chamado recebido")]
    public GameObject popupChamadoRecebido;
    public TMP_Text textoCliente;
    public TMP_Text textoPagamento;
    public Button botaoAceitar;
    public Button botaoRecusar;

    [Header("Popup 2 - Serviço aceito")]
    public GameObject popupServicoAceito;
    public TMP_Text textoProblema;
    public TMP_Text textoLocal;
    public Button botaoEntendi;

    private float contadorPopup;
    private bool popupRecebidoVisivel;
    private bool detalhesServicoAbertos;

    private void Start()
    {
        // Esconde os dois popups ao iniciar
        EsconderPopups();

        if (callGenerator == null)
        {
            Debug.LogError("CallGenerator não configurado no CallPopupUI!");
            return;
        }

        if (phoneController == null)
        {
            Debug.LogError("PhoneController não configurado no CallPopupUI!");
            return;
        }

        // Conecta aos eventos
        callGenerator.OnCallGenerated += MostrarChamado;

        phoneController.OnPhoneStateChanged += AtualizarEstadoCelular;

        // Configura os botões
        if (botaoAceitar != null)
            botaoAceitar.onClick.AddListener(AceitarChamado);

        if (botaoRecusar != null)
            botaoRecusar.onClick.AddListener(RecusarChamado);

        if (botaoEntendi != null)
            botaoEntendi.onClick.AddListener(FecharDetalhes);

        Debug.Log("CallPopupUI conectado!");
    }

    private void Update()
    {
        // O contador só funciona enquanto o popup
        // está visível e o celular está aberto.
        if (!popupRecebidoVisivel)
            return;

        if (phoneController == null || !phoneController.IsPhoneOpen)
            return;

        contadorPopup -= Time.deltaTime;

        if (contadorPopup <= 0f)
        {
            EsconderPopupRecebido();
        }
    }

    // CHAMADO GERADO
    private void MostrarChamado(CallData chamado)
    {
        if (chamado == null)
            return;

        // Atualiza a primeira mensagem
        if (textoCliente != null)
            textoCliente.text = chamado.nomeCliente;

        if (textoPagamento != null)
        {
            textoPagamento.text =
                "Valor do serviço: R$ " +
                chamado.pagamento.ToString("F2");
        }

        // Reinicia o tempo de exibição
        contadorPopup = tempoExibicao;

        popupRecebidoVisivel = true;

        // Só mostra se o celular estiver aberto
        if (phoneController != null &&
            phoneController.IsPhoneOpen)
        {
            MostrarPopupRecebido();
        }

        Debug.Log("Chamado recebido registrado na interface!");
    }

    private void MostrarPopupRecebido()
    {
        if (popupChamadoRecebido != null)
            popupChamadoRecebido.SetActive(true);

        if (popupServicoAceito != null)
            popupServicoAceito.SetActive(false);
    }

    private void EsconderPopupRecebido()
    {
        popupRecebidoVisivel = false;

        if (popupChamadoRecebido != null)
            popupChamadoRecebido.SetActive(false);
    }

    // ACEITAR CHAMADO
    public void AceitarChamado()
    {
        if (callGenerator == null)
            return;

        callGenerator.AceitarChamado();

        // Confirma que o serviço realmente foi aceito
        if (!callGenerator.servicoAtivo)
            return;

        CallData chamado = callGenerator.chamadoAtual;

        if (chamado == null)
            return;

        // Preenche o segundo popup
        if (textoProblema != null)
        {
            textoProblema.text =
                "Problema: " +
                chamado.tipoConserto.ToString();
        }

        if (textoLocal != null)
        {
            textoLocal.text =
                "Local do serviço: " +
                chamado.endereco;
        }

        popupRecebidoVisivel = false;
        detalhesServicoAbertos = true;

        // Primeiro popup desaparece
        if (popupChamadoRecebido != null)
            popupChamadoRecebido.SetActive(false);

        // Segundo popup aparece
        if (phoneController != null &&
            phoneController.IsPhoneOpen)
        {
            MostrarPopupServico();
        }

        Debug.Log("Chamado aceito. Detalhes do serviço exibidos!");
    }

    private void MostrarPopupServico()
    {
        if (popupChamadoRecebido != null)
            popupChamadoRecebido.SetActive(false);

        if (popupServicoAceito != null)
            popupServicoAceito.SetActive(true);
    }

    // RECUSAR CHAMADO
    public void RecusarChamado()
    {
        if (callGenerator == null)
            return;

        callGenerator.RecusarChamado();

        popupRecebidoVisivel = false;
        detalhesServicoAbertos = false;

        EsconderPopups();
    }

    // BOTÃO ENTENDI
    public void FecharDetalhes()
    {
        detalhesServicoAbertos = false;

        if (popupServicoAceito != null)
            popupServicoAceito.SetActive(false);

        // O serviço continua ativo.
    }

    // CELULAR ABERTO OU FECHADO
    private void AtualizarEstadoCelular(bool aberto)
    {
        if (!aberto)
        {
            // Fechar o celular apenas esconde a interface.
            // Não recusa nem cancela o chamado.
            EsconderPopups();
            return;
        }

        // Ao abrir o celular, verifica o estado do chamado.
        if (callGenerator == null)
            return;

        if (callGenerator.chamadoAtual == null)
        {
            EsconderPopups();
            return;
        }

        if (callGenerator.chamadoAtual.status == CallStatus.Pendente)
        {
            // Chamado pendente: mostra novamente
            // e reinicia o tempo de exibição.
            popupRecebidoVisivel = true;
            contadorPopup = tempoExibicao;

            MostrarPopupRecebido();
        }
        else if (callGenerator.servicoAtivo &&
                 detalhesServicoAbertos)
        {
            // Serviço aceito: mostra os detalhes
            // se o jogador ainda não os fechou.
            MostrarPopupServico();
        }
    }

    private void EsconderPopups()
    {
        if (popupChamadoRecebido != null)
            popupChamadoRecebido.SetActive(false);

        if (popupServicoAceito != null)
            popupServicoAceito.SetActive(false);
    }

    private void OnDestroy()
    {
        if (callGenerator != null)
            callGenerator.OnCallGenerated -= MostrarChamado;

        if (phoneController != null)
            phoneController.OnPhoneStateChanged -= AtualizarEstadoCelular;

        if (botaoAceitar != null)
            botaoAceitar.onClick.RemoveListener(AceitarChamado);

        if (botaoRecusar != null)
            botaoRecusar.onClick.RemoveListener(RecusarChamado);

        if (botaoEntendi != null)
            botaoEntendi.onClick.RemoveListener(FecharDetalhes);
    }
}
