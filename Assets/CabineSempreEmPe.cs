
using UnityEngine;

public class CabineSempreEmPe : MonoBehaviour
{
    private Quaternion rotacaoInicial;

    void Start()
    {
        rotacaoInicial = transform.rotation;
    }

    void LateUpdate()
    {
        transform.rotation = rotacaoInicial;
    }
}
