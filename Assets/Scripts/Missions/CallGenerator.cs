
using System;
using System.Collections.Generic;
using UnityEngine;

public class CallGenerator : MonoBehaviour
{
    [Header("TEMPO ENTRE CHAMADOS")]
    [Min(1f)]
    public float intervaloMinimo = 30f;

    [Min(1f)]
    public float intervaloMaximo = 60f;

    private float tempoRestante;

    [Header("CLIENTES")]
    public List<string> nomesClientes = new List<string>();

    [Header("ENDEREÇOS DISPONÍVEIS")]
    public List<CallLocation> locaisDisponiveis = new List<CallLocation>();

    [Header("PAGAMENTO - BATERIA")]
    public float bateriaMinimo = 100f;
    public float bateriaMaximo = 200f;

    [Header("PAGAMENTO - PNEU")]
    public float pneuMinimo = 80f;
    public float pneuMaximo = 150f;

    [Header("PAGAMENTO - RADIADOR")]
    public float radiadorMinimo = 120f;
    public float radiadorMaximo = 250f;

    [Header("VEÍCULO QUEBRADO")]
    public GameObject prefabCarroQuebrado;

    [Header("HISTÓRICO DA PARTIDA")]
    public List<CallData> historicoChamados = new List<CallData>();

    [Header("CHAMADO ATUAL")]
    [NonSerialized]
    public CallData chamadoAtual = null;

    private GameObject carroAtivo;

    public Transform CarroAtivo => carroAtivo != null ? carroAtivo.transform : null;

    public bool servicoAtivo { get; private set; }

    // Eventos para integração com outros sistemas
    public event Action<CallData> OnCallGenerated;
    public event Action<CallData> OnCallAccepted;
    public event Action<CallData> OnCallDeclined;
    public event Action<CallData> OnCallCompleted;

   
    void Start()
    {
        chamadoAtual = null;
        servicoAtivo = false;
        carroAtivo = null;

        AgendarProximoChamado();

        Debug.Log("Sistema de chamados iniciado!");
    }


    private void Update()
    {
        // Impede novos chamados enquanto existir
        // um chamado pendente ou serviço ativo.
        if (chamadoAtual != null || servicoAtivo)
            return;

        tempoRestante -= Time.deltaTime;

        if (tempoRestante <= 0f)
        {
            GerarChamado();
        }
    }

    private void AgendarProximoChamado()
    {
        float minimo = Mathf.Max(1f, intervaloMinimo);
        float maximo = Mathf.Max(minimo, intervaloMaximo);

        tempoRestante = UnityEngine.Random.Range(minimo, maximo);

        Debug.Log(
            "Próximo chamado em: " +
            tempoRestante.ToString("F1") + " segundos."
        );
    }

    public void GerarChamado()
    {
        // Proteção contra duplicação
        if (chamadoAtual != null || servicoAtivo)
            return;

        // Verifica os clientes
        if (nomesClientes.Count == 0)
        {
            Debug.LogWarning("Nenhum cliente cadastrado!");
            AgendarProximoChamado();
            return;
        }

        // Filtra endereços válidos
        List<CallLocation> locaisValidos = locaisDisponiveis.FindAll(
            local => local != null &&
                     local.pontoDeSpawn != null &&
                     !string.IsNullOrEmpty(local.endereco)
        );

        if (locaisValidos.Count == 0)
        {
            Debug.LogWarning(
                "Nenhum endereço com ponto de spawn válido!"
            );

            AgendarProximoChamado();
            return;
        }

        // Sorteia cliente
        string nome = nomesClientes[
            UnityEngine.Random.Range(0, nomesClientes.Count)
        ];

        // Sorteia endereço
        CallLocation localEscolhido = locaisValidos[
            UnityEngine.Random.Range(0, locaisValidos.Count)
        ];

        // Sorteia defeito
        RepairType defeito = (RepairType)UnityEngine.Random.Range(0, 3);

        // Cria o chamado
        CallData novoChamado = new CallData();

        novoChamado.id = Guid.NewGuid().ToString("N");
        novoChamado.nomeCliente = nome;
        novoChamado.endereco = localEscolhido.endereco;
        novoChamado.tipoConserto = defeito;
        novoChamado.pagamento = SortearPagamento(defeito);
        novoChamado.status = CallStatus.Pendente;
        novoChamado.localizacao = localEscolhido;
        novoChamado.horario = DateTime.Now.ToString("HH:mm");

        // Registra o chamado atual
        chamadoAtual = novoChamado;

        // Adiciona ao histórico
        historicoChamados.Add(novoChamado);

        Debug.Log("========== NOVO CHAMADO ==========");
        Debug.Log("ID: " + novoChamado.id);
        Debug.Log("Cliente: " + novoChamado.nomeCliente);
        Debug.Log("Endereço: " + novoChamado.endereco);
        Debug.Log("Problema: " + novoChamado.tipoConserto);
        Debug.Log("Pagamento: R$ " + novoChamado.pagamento.ToString("F2"));
        Debug.Log("==================================");

        // Notifica os outros sistemas
        OnCallGenerated?.Invoke(novoChamado);
    }

    private float SortearPagamento(RepairType tipo)
    {
        switch (tipo)
        {
            case RepairType.Bateria:
                return UnityEngine.Random.Range(
                    bateriaMinimo,
                    Mathf.Max(bateriaMinimo, bateriaMaximo)
                );

            case RepairType.Pneu:
                return UnityEngine.Random.Range(
                    pneuMinimo,
                    Mathf.Max(pneuMinimo, pneuMaximo)
                );

            case RepairType.AguaRadiador:
                return UnityEngine.Random.Range(
                    radiadorMinimo,
                    Mathf.Max(radiadorMinimo, radiadorMaximo)
                );

            default:
                return 0f;
        }
    }

    // ACEITAR CHAMADO
    public void AceitarChamado()
    {
        if (chamadoAtual == null)
        {
            Debug.LogWarning("Não existe chamado disponível!");
            return;
        }

        if (servicoAtivo)
        {
            Debug.LogWarning("Já existe um serviço em andamento!");
            return;
        }

        if (prefabCarroQuebrado == null)
        {
            Debug.LogError(
                "Prefab do carro quebrado não foi configurado!"
            );
            return;
        }

        if (chamadoAtual.localizacao == null ||
            chamadoAtual.localizacao.pontoDeSpawn == null)
        {
            Debug.LogError("Localização do chamado inválida!");
            return;
        }

        Transform ponto = chamadoAtual.localizacao.pontoDeSpawn;

        // Cria o veículo no endereço
        carroAtivo = Instantiate(
            prefabCarroQuebrado,
            ponto.position,
            ponto.rotation
        );

        chamadoAtual.status = CallStatus.Aceito;

        servicoAtivo = true;

        Debug.Log("Chamado aceito! Veículo criado no endereço.");

        OnCallAccepted?.Invoke(chamadoAtual);
    }

    // RECUSAR CHAMADO
    public void RecusarChamado()
    {
        if (chamadoAtual == null)
        {
            Debug.LogWarning("Não existe chamado para recusar!");
            return;
        }

        if (servicoAtivo)
            return;

        chamadoAtual.status = CallStatus.Recusado;

        Debug.Log("Chamado recusado!");

        OnCallDeclined?.Invoke(chamadoAtual);

        // Libera a chegada de outro chamado
        chamadoAtual = null;

        AgendarProximoChamado();
    }

    // CONCLUIR SERVIÇO
    public void ConcluirChamado()
    {
        if (chamadoAtual == null || !servicoAtivo)
        {
            Debug.LogWarning(
                "Não existe serviço ativo para concluir!"
            );
            return;
        }

        chamadoAtual.status = CallStatus.Concluido;

        Debug.Log("Serviço concluído com sucesso!");

        OnCallCompleted?.Invoke(chamadoAtual);

        // Remove o veículo da cena
        if (carroAtivo != null)
        {
            Destroy(carroAtivo);
            carroAtivo = null;
        }

        // Libera o próximo chamado
        chamadoAtual = null;

        servicoAtivo = false;

        AgendarProximoChamado();
    }
}
