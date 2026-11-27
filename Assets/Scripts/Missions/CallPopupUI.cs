
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CallPopupUI : MonoBehaviour
{
    [Header("Sistemas")]
    public CallGenerator callGenerator;
    public PhoneController phoneController;

    [Header("Notificação inicial")]
    public GameObject notificacaoChamado;
    public TMP_Text textoNotificacao;

    [Min(1f)]
    public float tempoNotificacao = 5f;

    private float contadorNotificacao;
    private bool notificacaoAtiva;

    [Header("Notificação de chamados pendentes")]
    public GameObject notificacaoPendente;
    public TMP_Text textoPendente;

    [Min(1f)]
    public float intervaloLembrete = 120f;

    [Min(1f)]
    public float tempoNotificacaoPendente = 5f;

    private float contadorLembrete;
    private float contadorNotificacaoPendente;

    private bool chamadoPendente;
    private bool notificacaoPendenteAtiva;

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

    private bool detalhesServicoAbertos;

    private void Start()
    {
        EsconderPopups();
        EsconderNotificacao();
        EsconderNotificacaoPendente();

        if (callGenerator == null)
        {
            Debug.LogError("CallGenerator não configurado!");
            return;
        }

        if (phoneController == null)
        {
            Debug.LogError("PhoneController não configurado!");
            return;
        }

        callGenerator.OnCallGenerated += MostrarChamado;
        phoneController.OnPhoneStateChanged += AtualizarEstadoCelular;

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
        if (phoneController == null)
            return;

        if (notificacaoAtiva)
        {
            contadorNotificacao -= Time.deltaTime;

            if (contadorNotificacao <= 0f)
                EsconderNotificacao();
        }

        if (chamadoPendente)
        {
            contadorLembrete -= Time.deltaTime;

            if (contadorLembrete <= 0f)
            {
                if (!phoneController.IsPhoneOpen)
                    MostrarNotificacaoPendente();

                contadorLembrete = intervaloLembrete;
            }
        }

        if (notificacaoPendenteAtiva)
        {
            contadorNotificacaoPendente -= Time.deltaTime;

            if (contadorNotificacaoPendente <= 0f)
                EsconderNotificacaoPendente();
        }
    }

    private void MostrarChamado(CallData chamado)
    {
        if (chamado == null)
            return;

        if (textoCliente != null)
            textoCliente.text = chamado.nomeCliente;

        if (textoPagamento != null)
        {
            textoPagamento.text =
                "Valor do serviço: R$ " +
                chamado.pagamento.ToString("F2");
        }

        chamadoPendente = true;
        contadorLembrete = intervaloLembrete;

        if (phoneController != null && phoneController.IsPhoneOpen)
        {
            MostrarPopupRecebido();
            EsconderNotificacao();
            EsconderNotificacaoPendente();
        }
        else
        {
            EsconderPopups();
            MostrarNotificacao();
        }

        Debug.Log("Chamado recebido!");
    }

    private void MostrarNotificacao()
    {
        if (phoneController != null && phoneController.IsPhoneOpen)
            return;

        if (textoNotificacao != null)
        {
            textoNotificacao.text =
                "Novo chamado recebido!\n" +
                "Abra o celular para ver.";
        }

        if (notificacaoChamado != null)
            notificacaoChamado.SetActive(true);

        notificacaoAtiva = true;
        contadorNotificacao = tempoNotificacao;

        Debug.Log("Notificação inicial exibida!");
    }

    private void EsconderNotificacao()
    {
        notificacaoAtiva = false;

        if (notificacaoChamado != null)
            notificacaoChamado.SetActive(false);
    }

    private void MostrarNotificacaoPendente()
    {
        if (phoneController != null && phoneController.IsPhoneOpen)
            return;

        if (notificacaoPendente == null || textoPendente == null)
        {
            Debug.LogWarning(
                "Painel ou texto da notificação pendente não configurado!"
            );

            return;
        }

        textoPendente.text =
            "Você tem mensagens pendentes!\n" +
            "Abra o celular para conferir seus chamados.";

        notificacaoPendente.SetActive(true);

        notificacaoPendenteAtiva = true;
        contadorNotificacaoPendente = tempoNotificacaoPendente;

        Debug.Log("Lembrete de chamado pendente exibido!");
    }

    private void EsconderNotificacaoPendente()
    {
        notificacaoPendenteAtiva = false;

        if (notificacaoPendente != null)
            notificacaoPendente.SetActive(false);
    }

    private void MostrarPopupRecebido()
    {
        if (popupChamadoRecebido != null)
            popupChamadoRecebido.SetActive(true);

        if (popupServicoAceito != null)
            popupServicoAceito.SetActive(false);
    }

    public void AceitarChamado()
    {
        if (callGenerator == null)
            return;

        callGenerator.AceitarChamado();

        if (!callGenerator.servicoAtivo)
            return;

        CallData chamado = callGenerator.chamadoAtual;

        if (chamado == null)
            return;

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

        chamadoPendente = false;
        contadorLembrete = 0f;

        detalhesServicoAbertos = true;

        EsconderNotificacao();
        EsconderNotificacaoPendente();
        EsconderPopups();

        if (phoneController != null && phoneController.IsPhoneOpen)
            MostrarPopupServico();
    }

    private void MostrarPopupServico()
    {
        if (popupChamadoRecebido != null)
            popupChamadoRecebido.SetActive(false);

        if (popupServicoAceito != null)
            popupServicoAceito.SetActive(true);
    }

    public void RecusarChamado()
    {
        if (callGenerator == null)
            return;

        callGenerator.RecusarChamado();

        chamadoPendente = false;
        contadorLembrete = 0f;

        detalhesServicoAbertos = false;

        EsconderPopups();
        EsconderNotificacao();
        EsconderNotificacaoPendente();
    }

    public void FecharDetalhes()
    {
        detalhesServicoAbertos = false;

        if (popupServicoAceito != null)
            popupServicoAceito.SetActive(false);
    }

    private void AtualizarEstadoCelular(bool aberto)
    {
        if (!aberto)
        {
            EsconderPopups();
            return;
        }

        EsconderNotificacao();
        EsconderNotificacaoPendente();

        if (callGenerator == null || callGenerator.chamadoAtual == null)
        {
            EsconderPopups();
            return;
        }

        if (callGenerator.chamadoAtual.status == CallStatus.Pendente)
        {
            MostrarPopupRecebido();
        }
        else if (callGenerator.servicoAtivo && detalhesServicoAbertos)
        {
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
