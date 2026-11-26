
using System;
using UnityEngine;

// Tipos de conserto disponíveis
public enum RepairType
{
    Bateria,
    Pneu,
    AguaRadiador
}

// Situação atual do chamado
public enum CallStatus
{
    Pendente,
    Aceito,
    Recusado,
    Concluido
}

[System.Serializable]
public class CallData
{
    [Header("Identificação")]
    public string id;

    [Header("Informações do cliente")]
    public string nomeCliente;
    public string endereco;

    [Header("Serviço")]
    public RepairType tipoConserto;

    [Header("Pagamento")]
    public float pagamento;

    [Header("Situação do chamado")]
    public CallStatus status;

    [Header("Localização no mapa")]
    public CallLocation localizacao;

    [Header("Data e horário")]
    public string horario;
}
