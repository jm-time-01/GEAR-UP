
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EntrarCarro : MonoBehaviour
{
    [Header("Personagem")]
    public Transform player;
    public Collider[] colisoresPlayer;
    public Renderer[] modelosPlayer;
    public Behaviour[] scriptsMovimento;

    [Header("Carro")]
    public Transform carro;
    public ControleCarro controleCarro;

    [Header("Ponto de saida")]
    public Transform pontoSaida;

    [Header("Cameras")]
    public Camera cameraPlayer;
    public Camera cameraCarro;

    [Header("Ajuste de saida")]
    public float alturaExtraSaida = 0.5f;
    public CharacterController characterController;

    private bool dentroDaArea = false;
    private bool dentroDoCarro = false;

    private Transform paiOriginal;

    private string tagPlayerOriginal;
    private string tagCarroOriginal;

    private AudioListener audioPlayer;
    private AudioListener audioCarro;

    void Start()
    {
        if (player != null)
            paiOriginal = player.parent;

        if (cameraPlayer != null)
        {
            tagPlayerOriginal = cameraPlayer.gameObject.tag;
            audioPlayer = cameraPlayer.GetComponent<AudioListener>();
            cameraPlayer.enabled = true;
        }

        if (cameraCarro != null)
        {
            tagCarroOriginal = cameraCarro.gameObject.tag;
            audioCarro = cameraCarro.GetComponent<AudioListener>();
            cameraCarro.enabled = false;
        }

        if (audioCarro != null)
            audioCarro.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) &&
            dentroDaArea && !dentroDoCarro)
        {
            Entrar();
        }

        if (Input.GetKeyDown(KeyCode.F) && dentroDoCarro)
        {
            Sair();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dentroDaArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dentroDaArea = false;
        }
    }

    void Entrar()
    {

        if (player == null || carro == null ||
            controleCarro == null)
        {
            Debug.LogError("Configure Player e Carro no Inspector!");
            return;
        }

        dentroDoCarro = true;
        dentroDaArea = false;

        paiOriginal = player.parent;

        player.SetParent(carro, true);
        player.position = carro.position;

        foreach (Renderer modelo in modelosPlayer)
            if (modelo != null) modelo.enabled = false;

        foreach (Collider colisor in colisoresPlayer)
            if (colisor != null) colisor.enabled = false;

        foreach (Behaviour script in scriptsMovimento)
            if (script != null && script != this) script.enabled = false;

        cameraPlayer.enabled = false;
        cameraCarro.enabled = true;

        cameraPlayer.gameObject.tag = "Untagged";
        cameraCarro.gameObject.tag = "MainCamera";

        if (audioPlayer != null) audioPlayer.enabled = false;
        if (audioCarro != null) audioCarro.enabled = true;

        controleCarro.SetPlayerInside(true);

        Debug.Log("Player entrou no carro!");
    }

    void Sair()
    {
        if (pontoSaida == null)
        {
            Debug.LogError("PontoSaida não foi configurado!");
            return;
        }

        controleCarro.StopCar();

        // Desativa o CharacterController antes de reposicionar
        if (characterController != null)
            characterController.enabled = false;

        // Desvincula o jogador do carro
        player.SetParent(paiOriginal, true);

        // Posiciona o jogador no ponto de saída
        Vector3 novaPosicao = pontoSaida.position;
        novaPosicao.y += alturaExtraSaida;

        player.position = novaPosicao;
        player.rotation = pontoSaida.rotation;

        // Reativa câmera do jogador
        cameraCarro.enabled = false;
        cameraPlayer.enabled = true;

        cameraCarro.gameObject.tag = tagCarroOriginal;
        cameraPlayer.gameObject.tag = tagPlayerOriginal;

        if (audioCarro != null) audioCarro.enabled = false;
        if (audioPlayer != null) audioPlayer.enabled = true;

        // Reativa os modelos
        foreach (Renderer modelo in modelosPlayer)
            if (modelo != null) modelo.enabled = true;

        // Reativa os colisores
        foreach (Collider colisor in colisoresPlayer)
            if (colisor != null) colisor.enabled = true;

        // Reativa o CharacterController
        if (characterController != null)
            characterController.enabled = true;

        // Reativa os scripts de movimentação
        foreach (Behaviour script in scriptsMovimento)
            if (script != null && script != this) script.enabled = true;

        dentroDoCarro = false;

        Debug.Log("Player reposicionado em: " + player.position);
    }
}
