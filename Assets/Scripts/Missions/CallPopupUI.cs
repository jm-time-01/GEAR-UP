
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CallPopupUI : MonoBehaviour
{
    [Header("Sistema de chamados")]
    public CallGenerator callGenerator;

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

    private void Start()
    {
        // Os dois popups começam escondidos
        popupChamadoRecebido.SetActive(false);
        popupServicoAceito.SetActive(false);

        if (callGenerator == null)
        {
            Debug.LogError("CallGenerator não configurado!");
            return;
        }

        callGenerator.OnCallGenerated += MostrarChamado;

        botaoAceitar.onClick.AddListener(AceitarChamado);
        botaoRecusar.onClick.AddListener(RecusarChamado);
        botaoEntendi.onClick.AddListener(FecharDetalhes);

        Debug.Log("CallPopupUI conectado!");
    }

    // PRIMEIRO POPUP
    private void MostrarChamado(CallData chamado)
    {
        if (chamado == null)
            return;

        textoCliente.text = chamado.nomeCliente;

        textoPagamento.text =
            "Valor: R$ " +
            chamado.pagamento.ToString("F2");

        // Mostra somente o popup de recebimento
        popupServicoAceito.SetActive(false);
        popupChamadoRecebido.SetActive(true);

        Debug.Log("Popup de chamado recebido exibido!");
    }

    // ACEITAR
    public void AceitarChamado()
    {
        if (callGenerator == null)
            return;

        callGenerator.AceitarChamado();

        // Só troca os painéis se o chamado foi aceito
        if (!callGenerator.servicoAtivo)
            return;

        CallData chamado = callGenerator.chamadoAtual;

        if (chamado == null)
            return;

        // Preenche o segundo popup
        textoProblema.text =
            "Problema: " + chamado.tipoConserto.ToString();

        textoLocal.text =
            "Local: " + chamado.endereco;

        // Esconde o primeiro e mostra o segundo
        popupChamadoRecebido.SetActive(false);
        popupServicoAceito.SetActive(true);

        Debug.Log("Popup de serviço aceito exibido!");
    }

    // RECUSAR
    public void RecusarChamado()
    {
        if (callGenerator == null)
            return;

        callGenerator.RecusarChamado();

        // Fecha somente o primeiro popup
        popupChamadoRecebido.SetActive(false);

        // O segundo continua escondido
        popupServicoAceito.SetActive(false);
    }

    // BOTÃO ENTENDI
    public void FecharDetalhes()
    {
        popupServicoAceito.SetActive(false);

        // O serviço continua ativo no CallGenerator.
        // Este botão apenas fecha a janela de informações.
    }

    private void OnDestroy()
    {
        if (callGenerator != null)
        {
            callGenerator.OnCallGenerated -= MostrarChamado;
        }

        if (botaoAceitar != null)
            botaoAceitar.onClick.RemoveListener(AceitarChamado);

        if (botaoRecusar != null)
            botaoRecusar.onClick.RemoveListener(RecusarChamado);

        if (botaoEntendi != null)
            botaoEntendi.onClick.RemoveListener(FecharDetalhes);
    }
}
