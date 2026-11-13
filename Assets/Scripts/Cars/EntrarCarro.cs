using UnityEngine;

public class EntrarCarro : MonoBehaviour
{
    [Header("Configurações")]
    public float distanciaEntrada = 3f;
    public KeyCode teclaEntrada = KeyCode.E;



    [Header("Referências")]
    public CameraCarro cameraCarro;
    public CharacterController characterController;
    public GameObject modeloPersonagem;
    public Behaviour[] scriptsMovimento;
    private Carro carroAtual;
    private bool dentroDoCarro = false;
    public bool playerControlled = false;
    void Update()
    {
        if (dentroDoCarro)
        {
            if (Input.GetKeyDown(teclaEntrada))
                SairDoCarro();
            return;
        }
        ProcurarCarro();
        if (carroAtual != null &&
            Input.GetKeyDown(teclaEntrada))
        {
            EntrarNoCarro();
        }
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
        dentroDoCarro = true;
        carroAtual.playerControlled = true;
        if (characterController != null)
            characterController.enabled = false;
        if (modeloPersonagem != null)
            modeloPersonagem.SetActive(false);
        foreach (Behaviour script in scriptsMovimento)
        {
            if (script != null)
                script.enabled = false;
        }
        cameraCarro.SeguirCarro(carroAtual.transform);
    }
    void SairDoCarro()
    {
        dentroDoCarro = false;
        carroAtual.playerControlled = false;
        transform.position =
            carroAtual.transform.position -
            carroAtual.transform.right * 2f;
        if (characterController != null)
            characterController.enabled = true;
        if (modeloPersonagem != null)
            modeloPersonagem.SetActive(true);
        foreach (Behaviour script in scriptsMovimento)
        {
            if (script != null)
                script.enabled = true;
        }
        cameraCarro.VoltarPersonagem();
        carroAtual = null;
    }
}


