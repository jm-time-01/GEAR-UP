
using UnityEngine;

[System.Serializable]
public class CallLocation
{
    [Header("Informações do endereço")]
    public string endereco;

    [Header("Local onde o carro será criado")]
    public Transform pontoDeSpawn;
}
