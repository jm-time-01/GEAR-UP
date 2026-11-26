using UnityEngine;

public class CameraCarro : MonoBehaviour
{
    public Transform pontoCameraCarro;
    public float velocidadeCamera = 8f;
    public float alturaOlhar = 1.5f;

    private Transform alvo;
    private Transform paiOriginal;
    private Vector3 posicaoOriginal;
    private Quaternion rotacaoOriginal;

    void Awake()
    {
        paiOriginal = transform.parent;
        posicaoOriginal = transform.localPosition;
        rotacaoOriginal = transform.localRotation;
    }

    void LateUpdate()
    {
        if (alvo == null)
            return;

        transform.position = Vector3.Lerp(
            transform.position,
            pontoCameraCarro.position,
            velocidadeCamera * Time.deltaTime
        );

        Vector3 olhar = alvo.position + Vector3.up * alturaOlhar;

        Quaternion rotacao = Quaternion.LookRotation(
            olhar - transform.position
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            rotacao,
            velocidadeCamera * Time.deltaTime
        );
    }

    public void SeguirCarro(Transform carro)
    {
        alvo = carro;
        transform.SetParent(null);
    }

    public void VoltarPersonagem()
    {
        alvo = null;

        transform.SetParent(paiOriginal);
        transform.localPosition = posicaoOriginal;
        transform.localRotation = rotacaoOriginal;
    }

}