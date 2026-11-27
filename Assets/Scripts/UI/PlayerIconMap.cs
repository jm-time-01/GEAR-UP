
using UnityEngine;

public class PlayerIconMap : MonoBehaviour
{
    [Header("Referências")]
    public Transform jogador;
    public Camera cameraMapa;
    public RectTransform areaMapa;
    public RectTransform canvasRect;

    [Header("Direção do jogador")]
    public Transform cameraJogador;
    public float ajusteRotacao = 0f;

    [Header("Ajuste de posição")]
    public Vector2 deslocamento = Vector2.zero;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (jogador == null ||
            cameraMapa == null ||
            areaMapa == null ||
            rectTransform == null)
            return;

        AtualizarPosicao();
        AtualizarRotacao();
    }

    private void AtualizarPosicao()
    {
        Vector3 viewport = cameraMapa.WorldToViewportPoint(
            jogador.position
        );

        if (viewport.z <= 0f)
            return;

        Vector2 pontoLocal = new Vector2(
            Mathf.Lerp(areaMapa.rect.xMin, areaMapa.rect.xMax, viewport.x),
            Mathf.Lerp(areaMapa.rect.yMin, areaMapa.rect.yMax, viewport.y)
        );

        Vector3 pontoMundo = areaMapa.TransformPoint(pontoLocal);

        Vector3 pontoNaTela = RectTransformUtility.WorldToScreenPoint(
            canvasRect != null ? canvasRect.GetComponentInParent<Canvas>().worldCamera : null,
            pontoMundo
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)rectTransform.parent,
            pontoNaTela,
            canvasRect != null && canvasRect.GetComponentInParent<Canvas>().renderMode != RenderMode.ScreenSpaceOverlay
                ? canvasRect.GetComponentInParent<Canvas>().worldCamera
                : null,
            out Vector2 posicaoFinal
        );

        rectTransform.localPosition = posicaoFinal + deslocamento;
    }

    private void AtualizarRotacao()
    {
        if (cameraJogador == null)
            return;

        Vector3 direcao = cameraJogador.forward;
        direcao.y = 0f;

        if (direcao.sqrMagnitude < 0.001f)
            return;

        float angulo = Mathf.Atan2(direcao.x, direcao.z) * Mathf.Rad2Deg;

        rectTransform.localRotation = Quaternion.Euler(
            0f,
            0f,
            -angulo + ajusteRotacao
        );
    }
}
