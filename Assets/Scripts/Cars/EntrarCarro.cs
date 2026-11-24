using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EntrarCarro : MonoBehaviour
{
    [Header("Personagem")]
    public Transform player;
    public CharacterController characterController;
    public Collider[] colisoresPlayer;
    public Renderer[] modelosPlayer;
    public Behaviour[] scriptsMovimento;

    [Header("Carro")]
    public Transform carro;
    public ControleCarro controleCarro;

    [Header("Ponto de saida")]
    public Transform pontoSaida;
    public float alturaExtraSaida = 0.5f;

    [Header("Cameras")]
    public Camera cameraPlayer;
    public Camera cameraCarro;

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
            Debug.Log("Player entrou na área do carro!");
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
            controleCarro == null || cameraPlayer == null ||
            cameraCarro == null)
        {
            Debug.LogError("Configure as referências do EntrarCarro!");
            return;
        }

        dentroDoCarro = true;
        dentroDaArea = false;

        paiOriginal = player.parent;

        if (characterController != null)
            characterController.enabled = false;

        player.SetParent(carro, true);
        player.position = carro.position;

        foreach (Renderer modelo in modelosPlayer)
            if (modelo != null)
                modelo.enabled = false;

        foreach (Collider colisor in colisoresPlayer)
            if (colisor != null)
                colisor.enabled = false;

        foreach (Behaviour script in scriptsMovimento)
            if (script != null && script != this)
                script.enabled = false;

        cameraPlayer.enabled = false;
        cameraCarro.enabled = true;

        cameraPlayer.gameObject.tag = "Untagged";
        cameraCarro.gameObject.tag = "MainCamera";

        if (audioPlayer != null)
            audioPlayer.enabled = false;

        if (audioCarro != null)
            audioCarro.enabled = true;

        controleCarro.SetPlayerInside(true);

        Debug.Log("Player entrou no carro!");
    }

    void Sair()
    {
        if (pontoSaida == null)
        {
            Debug.LogError("Configure o PontoSaida!");
            return;
        }

        controleCarro.StopCar();

        // Desativa o CharacterController antes de mover o jogador.
        if (characterController != null)
            characterController.enabled = false;

        // Retira o jogador da hierarquia do carro.
        player.SetParent(paiOriginal, true);

        // Posiciona o jogador ao lado do carro.
        Vector3 novaPosicao = pontoSaida.position;
        novaPosicao.y += alturaExtraSaida;

        player.position = novaPosicao;
        player.rotation = pontoSaida.rotation;

        // Restaura as câmeras.
        cameraCarro.enabled = false;
        cameraPlayer.enabled = true;

        cameraCarro.gameObject.tag = tagCarroOriginal;
        cameraPlayer.gameObject.tag = tagPlayerOriginal;

        if (audioCarro != null)
            audioCarro.enabled = false;

        if (audioPlayer != null)
            audioPlayer.enabled = true;

        // Mostra o personagem novamente.
        foreach (Renderer modelo in modelosPlayer)
            if (modelo != null)
                modelo.enabled = true;

        foreach (Collider colisor in colisoresPlayer)
            if (colisor != null)
                colisor.enabled = true;

        // Reativa o controlador.
        if (characterController != null)
            characterController.enabled = true;

        // Reativa os scripts de movimentação.
        foreach (Behaviour script in scriptsMovimento)
            if (script != null && script != this)
                script.enabled = true;

        dentroDoCarro = false;

        Debug.Log("Player saiu em: " + player.position);
    }

}