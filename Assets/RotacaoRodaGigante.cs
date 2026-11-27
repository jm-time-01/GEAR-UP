using UnityEngine;

public class RotacaoRodaGigante : MonoBehaviour
{
    public float velocidade = 10f;

    void Update()
    {
        transform.Rotate(0, 0, velocidade * Time.deltaTime);
    }
}