
using UnityEngine;

public class StreetLight : MonoBehaviour
{
    [Header("Configuração da luz")]
    public Light luzPoste;

    [Header("Horário")]
    [Range(0f, 1f)]
    public float limiteParaAcender = 0.25f;

    [Range(0f, 1f)]
    public float limiteParaApagar = 0.35f;

    [Header("Intensidade")]
    public float intensidadeNoite = 2f;

    private DayNightCycle ciclo;

    void Start()
    {
        ciclo = FindObjectOfType<DayNightCycle>();

        if (luzPoste == null)
            luzPoste = GetComponentInChildren<Light>();

        if (luzPoste == null)
        {
            Debug.LogWarning("Nenhuma Point Light encontrada no poste!");
        }
    }

    void Update()
    {
        if (ciclo == null || luzPoste == null)
            return;

        float intensidadeSol = Mathf.Clamp01(
            Mathf.Sin(ciclo.AnguloAtual * Mathf.Deg2Rad)
        );

        bool noite = intensidadeSol < limiteParaAcender;

        luzPoste.enabled = noite;

        if (noite)
            luzPoste.intensity = intensidadeNoite;
    }
}
