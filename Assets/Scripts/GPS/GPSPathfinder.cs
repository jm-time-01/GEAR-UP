
using System.Collections.Generic;
using UnityEngine;

public class GPSPathfinder : MonoBehaviour
{
    public List<GPSWaypoint> CalcularRota(
        GPSWaypoint inicio,
        GPSWaypoint destino)
    {
        List<GPSWaypoint> rota = new List<GPSWaypoint>();

        if (inicio == null || destino == null)
        {
            Debug.LogWarning("Ponto inicial ou destino inválido!");
            return rota;
        }

        List<GPSWaypoint> abertos = new List<GPSWaypoint>();
        HashSet<GPSWaypoint> fechados = new HashSet<GPSWaypoint>();

        Dictionary<GPSWaypoint, GPSWaypoint> anteriores =
            new Dictionary<GPSWaypoint, GPSWaypoint>();

        Dictionary<GPSWaypoint, float> custos =
            new Dictionary<GPSWaypoint, float>();

        Dictionary<GPSWaypoint, float> estimativas =
            new Dictionary<GPSWaypoint, float>();

        abertos.Add(inicio);
        custos[inicio] = 0f;
        estimativas[inicio] = Distancia(inicio, destino);

        while (abertos.Count > 0)
        {
            GPSWaypoint atual = EncontrarMelhorPonto(
                abertos,
                estimativas
            );

            if (atual == destino)
            {
                return ReconstruirRota(anteriores, atual);
            }

            abertos.Remove(atual);
            fechados.Add(atual);

            foreach (GPSWaypoint vizinho in atual.conexoes)
            {
                if (vizinho == null || fechados.Contains(vizinho))
                    continue;

                float novoCusto = custos[atual] +
                    Distancia(atual, vizinho);

                if (!custos.ContainsKey(vizinho) ||
                    novoCusto < custos[vizinho])
                {
                    anteriores[vizinho] = atual;
                    custos[vizinho] = novoCusto;

                    estimativas[vizinho] =
                        novoCusto + Distancia(vizinho, destino);

                    if (!abertos.Contains(vizinho))
                        abertos.Add(vizinho);
                }
            }
        }

        Debug.LogWarning("Não foi possível encontrar uma rota!");
        return rota;
    }

    private GPSWaypoint EncontrarMelhorPonto(
        List<GPSWaypoint> pontos,
        Dictionary<GPSWaypoint, float> estimativas)
    {
        GPSWaypoint melhor = pontos[0];

        foreach (GPSWaypoint ponto in pontos)
        {
            if (estimativas[ponto] < estimativas[melhor])
                melhor = ponto;
        }

        return melhor;
    }

    private float Distancia(GPSWaypoint a, GPSWaypoint b)
    {
        return Vector3.Distance(
            a.transform.position,
            b.transform.position
        );
    }

    private List<GPSWaypoint> ReconstruirRota(
        Dictionary<GPSWaypoint, GPSWaypoint> anteriores,
        GPSWaypoint atual)
    {
        List<GPSWaypoint> rota = new List<GPSWaypoint>();

        rota.Add(atual);

        while (anteriores.ContainsKey(atual))
        {
            atual = anteriores[atual];
            rota.Insert(0, atual);
        }

        return rota;
    }
}
