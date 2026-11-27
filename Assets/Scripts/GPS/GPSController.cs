
using System.Collections.Generic;
using UnityEngine;

public class GPSController : MonoBehaviour
{
    [Header("Referências principais")]
    public Transform jogador;
    public CallGenerator gerenciadorChamados;
    public GPSPathfinder pathfinder;
    public GPSRouteRenderer renderizadorRota;

    [Header("Marcador no minimapa")]
    public RectTransform iconeDestino;
    public RectTransform areaMinimapa;

    [Header("Marcador 3D")]
    public GPSWorldMarker marcador3D;

    private List<Vector3> rotaCompleta = new List<Vector3>();
    private List<Vector3> rotaVisivel = new List<Vector3>();

    private void OnEnable()
    {
        if (gerenciadorChamados != null)
        {
            gerenciadorChamados.OnCallAccepted += IniciarRota;
            gerenciadorChamados.OnCallCompleted += LimparRota;
        }
    }

    private void OnDisable()
    {
        if (gerenciadorChamados != null)
        {
            gerenciadorChamados.OnCallAccepted -= IniciarRota;
            gerenciadorChamados.OnCallCompleted -= LimparRota;
        }
    }

    private void Update()
    {
        if (rotaCompleta.Count >= 2 && jogador != null)
        {
            AtualizarRotaVisivel();
        }

        AtualizarIconeDestino();
    }

    private void IniciarRota(CallData chamado)
    {
        if (jogador == null ||
            gerenciadorChamados == null ||
            pathfinder == null ||
            renderizadorRota == null)
        {
            Debug.LogError("GPS: existem referências não configuradas!");
            return;
        }

        Transform carro = gerenciadorChamados.CarroAtivo;

        if (carro == null)
        {
            Debug.LogError("GPS: não foi possível encontrar o carro do chamado!");
            return;
        }

        GPSWaypoint inicio = EncontrarWaypointMaisProximo(jogador.position);
        GPSWaypoint destino = EncontrarWaypointMaisProximo(carro.position);

        if (inicio == null || destino == null)
        {
            Debug.LogError("GPS: não existem waypoints suficientes!");
            return;
        }

        List<GPSWaypoint> caminho = pathfinder.CalcularRota(inicio, destino);

        if (caminho.Count == 0)
        {
            Debug.LogError("GPS: não foi possível calcular o caminho!");
            return;
        }

        rotaCompleta.Clear();
        rotaVisivel.Clear();

        rotaCompleta.Add(jogador.position);

        foreach (GPSWaypoint waypoint in caminho)
        {
            rotaCompleta.Add(waypoint.transform.position);
        }

        rotaCompleta.Add(carro.position);

        AtualizarRotaVisivel();

        if (iconeDestino != null)
        {
            iconeDestino.gameObject.SetActive(true);
        }

        if (marcador3D != null)
        {
            marcador3D.Mostrar(carro);
        }

        Debug.Log("GPS: rota calculada com sucesso!");
    }

    private void AtualizarRotaVisivel()
    {
        if (rotaCompleta.Count < 2 || jogador == null)
            return;

        Vector3 posicaoJogador = jogador.position;

        float menorDistancia = Mathf.Infinity;
        int segmentoMaisProximo = 0;
        float progressoSegmento = 0f;

        Vector3 jogadorPlano = new Vector3(
            posicaoJogador.x,
            0f,
            posicaoJogador.z
        );

        for (int i = 0; i < rotaCompleta.Count - 1; i++)
        {
            Vector3 a = rotaCompleta[i];
            Vector3 b = rotaCompleta[i + 1];

            Vector3 aPlano = new Vector3(a.x, 0f, a.z);
            Vector3 bPlano = new Vector3(b.x, 0f, b.z);

            Vector3 segmento = bPlano - aPlano;
            float comprimentoQuadrado = segmento.sqrMagnitude;

            float t = comprimentoQuadrado > 0f
                ? Mathf.Clamp01(
                    Vector3.Dot(jogadorPlano - aPlano, segmento) /
                    comprimentoQuadrado
                )
                : 0f;

            Vector3 pontoMaisProximo = Vector3.Lerp(aPlano, bPlano, t);

            float distancia = Vector3.Distance(
                jogadorPlano,
                pontoMaisProximo
            );

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                segmentoMaisProximo = i;
                progressoSegmento = t;
            }
        }

        rotaVisivel.Clear();

        Vector3 inicioAtual = Vector3.Lerp(
            rotaCompleta[segmentoMaisProximo],
            rotaCompleta[segmentoMaisProximo + 1],
            progressoSegmento
        );

        rotaVisivel.Add(inicioAtual);

        for (int i = segmentoMaisProximo + 1; i < rotaCompleta.Count; i++)
        {
            rotaVisivel.Add(rotaCompleta[i]);
        }

        if (rotaVisivel.Count >= 2)
        {
            renderizadorRota.pontosRota = new List<Vector3>(rotaVisivel);
        }
        else
        {
            renderizadorRota.pontosRota.Clear();
        }
    }

    private void AtualizarIconeDestino()
    {
        if (iconeDestino == null ||
            areaMinimapa == null ||
            gerenciadorChamados == null ||
            gerenciadorChamados.CarroAtivo == null ||
            renderizadorRota == null)
        {
            return;
        }

        Camera cameraMapa = renderizadorRota.cameraMinimap;

        if (cameraMapa == null)
            return;

        Vector3 posicaoViewport = cameraMapa.WorldToViewportPoint(
            gerenciadorChamados.CarroAtivo.position
        );

        Vector2 posicaoLocal = new Vector2(
            (posicaoViewport.x - 0.5f) * areaMinimapa.rect.width,
            (posicaoViewport.y - 0.5f) * areaMinimapa.rect.height
        );

        float limiteX = (areaMinimapa.rect.width / 2f) -
                        (iconeDestino.rect.width / 2f);

        float limiteY = (areaMinimapa.rect.height / 2f) -
                        (iconeDestino.rect.height / 2f);

        posicaoLocal.x = Mathf.Clamp(
            posicaoLocal.x,
            -limiteX,
            limiteX
        );

        posicaoLocal.y = Mathf.Clamp(
            posicaoLocal.y,
            -limiteY,
            limiteY
        );

        iconeDestino.anchoredPosition = posicaoLocal;
    }

    private GPSWaypoint EncontrarWaypointMaisProximo(Vector3 posicao)
    {
        GPSWaypoint[] todosWaypoints = FindObjectsByType<GPSWaypoint>(
            FindObjectsSortMode.None
        );

        GPSWaypoint maisProximo = null;
        float menorDistancia = Mathf.Infinity;

        foreach (GPSWaypoint waypoint in todosWaypoints)
        {
            float distancia = Vector3.Distance(
                posicao,
                waypoint.transform.position
            );

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                maisProximo = waypoint;
            }
        }

        return maisProximo;
    }

    private void LimparRota(CallData chamado)
    {
        rotaCompleta.Clear();
        rotaVisivel.Clear();

        if (renderizadorRota != null)
        {
            renderizadorRota.pontosRota.Clear();
        }

        if (iconeDestino != null)
        {
            iconeDestino.gameObject.SetActive(false);
        }

        if (marcador3D != null)
        {
            marcador3D.Esconder();
        }

        Debug.Log("GPS: rota e marcadores removidos!");
    }
}
