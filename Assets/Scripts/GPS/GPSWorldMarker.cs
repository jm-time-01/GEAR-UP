
using UnityEngine;

public class GPSWorldMarker : MonoBehaviour
{
    [Header("Configuração")]
    public Transform destino;
    public Camera cameraJogador;
    public float altura = 3f;

    private void LateUpdate()
    {
        if (destino == null)
            return;

        transform.position = destino.position + Vector3.up * altura;

        if (cameraJogador != null)
        {
            transform.rotation = cameraJogador.transform.rotation;
        }
    }

    public void Mostrar(Transform novoDestino)
    {
        destino = novoDestino;
        gameObject.SetActive(true);
    }

    public void Esconder()
    {
        destino = null;
        gameObject.SetActive(false);
    }
}
