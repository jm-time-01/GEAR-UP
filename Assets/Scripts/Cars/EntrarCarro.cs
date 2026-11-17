
using UnityEngine;

public class EntrarCarro : MonoBehaviour
{
    [Header("Configurações")]
    public float distanciaEntrada = 3f;
    public KeyCode teclaEntrada = KeyCode.E;
    public KeyCode teclaSaida = KeyCode.F;

    [Header("Referências")]
    public Camera cameraCarro;
    public Camera cameraPlayer;
    public CharacterController characterController;
    public Behaviour[] scriptsMovimento;

    private Camera cameraVeiculo;
    private AudioListener audioPlayer;
    private AudioListener audioVeiculo;

    private Carro carroAtual;
    private bool dentroDoCarro = false;
    private Renderer[] renderizadores;
    private Collider[] colisoesPersonagem;
    private bool[] estadosColisoes;

    void Awake()
    {
        renderizadores = GetComponentsInChildren<Renderer>(true);
        colisoesPersonagem = GetComponentsInChildren<Collider>(true);
        estadosColisoes = new bool[colisoesPersonagem.Length];

        for (int i = 0; i < colisoesPersonagem.Length; i++)
        {
            estadosColisoes[i] = colisoesPersonagem[i].enabled;
        }

        // Encontra a câmera que está dentro do DriverView
        if (cameraCarro != null)
        {
            cameraVeiculo = cameraCarro.GetComponentInChildren<Camera>(true);
        }

        // Encontra os Audio Listeners
        if (cameraPlayer != null)
            audioPlayer = cameraPlayer.GetComponent<AudioListener>();

        if (cameraVeiculo != null)
            audioVeiculo = cameraVeiculo.GetComponent<AudioListener>();

        // Começa utilizando a câmera do jogador
        AtivarCameraPlayer();
    }

    void Update()
    {
        if (dentroDoCarro)
        {
            if (Input.GetKeyDown(teclaSaida))
                SairDoCarro();

            return;
        }

        ProcurarCarro();

        if (carroAtual != null && Input.GetKeyDown(teclaEntrada))
            EntrarNoCarro();
    }

    void ProcurarCarro()
    {
        carroAtual = null;

        Collider[] objetos = Physics.OverlapSphere(
            transform.position,
            distanciaEntrada
        );

        foreach (Collider objeto in objetos)
        {
            Carro carro = objeto.GetComponentInParent<Carro>();

            if (carro != null)
            {
                carroAtual = carro;
                break;
            }
        }
    }

    void EntrarNoCarro()
    {
        if (carroAtual == null) return;

        dentroDoCarro = true;
        carroAtual.playerControlled = true;

        foreach (Collider col in colisoesPersonagem)
        {
            if (col != null)
                col.enabled = false;
        }

        foreach (Renderer r in renderizadores)
            r.enabled = false;

        foreach (Behaviour script in scriptsMovimento)
        {
            if (script != null)
                script.enabled = false;
        }

        // Troca para a câmera do carro
        AtivarCameraCarro();
    }

    void SairDoCarro()
    {
        if (carroAtual == null) return;

        transform.position =
            carroAtual.transform.position -
            carroAtual.transform.right * 2.5f;

        carroAtual.playerControlled = false;
        dentroDoCarro = false;

        for (int i = 0; i < colisoesPersonagem.Length; i++)
        {
            if (colisoesPersonagem[i] != null)
                colisoesPersonagem[i].enabled = estadosColisoes[i];
        }

        foreach (Renderer r in renderizadores)
            r.enabled = true;

        foreach (Behaviour script in scriptsMovimento)
        {
            if (script != null)
                script.enabled = true;
        }

        // Volta para a câmera do jogador
        AtivarCameraPlayer();

        carroAtual = null;
    }

    void AtivarCameraCarro()
    {
        if (cameraPlayer != null)
            cameraPlayer.enabled = false;

        if (audioPlayer != null)
            audioPlayer.enabled = false;

        if (cameraVeiculo != null)
            cameraVeiculo.enabled = true;

        if (audioVeiculo != null)
            audioVeiculo.enabled = true;

        if (cameraCarro != null)
            cameraCarro.enabled = true;
    }

    void AtivarCameraPlayer()
    {
        if (cameraVeiculo != null)
            cameraVeiculo.enabled = false;

        if (audioVeiculo != null)
            audioVeiculo.enabled = false;

        if (cameraCarro != null)
            cameraCarro.enabled = false;

        if (cameraPlayer != null)
            cameraPlayer.enabled = true;

        if (audioPlayer != null)
            audioPlayer.enabled = true;
    }
}
