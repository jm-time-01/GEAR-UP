
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSRouteRenderer : MaskableGraphic
{
    [Header("Configuração do minimapa")]
    public Camera cameraMinimap;

    [Header("Aparência da rota")]
    public float espessura = 5f;
    public Color corRota = Color.yellow;

    [Header("Pontos da rota")]
    public List<Vector3> pontosRota = new List<Vector3>();

    protected override void Awake()
    {
        base.Awake();
        raycastTarget = false;
    }

    private void LateUpdate()
    {
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (cameraMinimap == null || pontosRota == null || pontosRota.Count < 2)
            return;

        Rect area = rectTransform.rect;

        Color32 cor = corRota;

        for (int i = 0; i < pontosRota.Count - 1; i++)
        {
            Vector3 pontoA = cameraMinimap.WorldToViewportPoint(pontosRota[i]);
            Vector3 pontoB = cameraMinimap.WorldToViewportPoint(pontosRota[i + 1]);

            if (pontoA.z <= 0 || pontoB.z <= 0)
                continue;

            Vector2 a = new Vector2(
                area.xMin + Mathf.Clamp01(pontoA.x) * area.width,
                area.yMin + Mathf.Clamp01(pontoA.y) * area.height
            );

            Vector2 b = new Vector2(
                area.xMin + Mathf.Clamp01(pontoB.x) * area.width,
                area.yMin + Mathf.Clamp01(pontoB.y) * area.height
            );

            Vector2 direcao = (b - a).normalized;
            Vector2 perpendicular = new Vector2(-direcao.y, direcao.x) * (espessura / 2f);

            int indice = vh.currentVertCount;

            vh.AddVert(a - perpendicular, cor, Vector2.zero);
            vh.AddVert(a + perpendicular, cor, Vector2.zero);
            vh.AddVert(b + perpendicular, cor, Vector2.zero);
            vh.AddVert(b - perpendicular, cor, Vector2.zero);

            vh.AddTriangle(indice, indice + 1, indice + 2);
            vh.AddTriangle(indice, indice + 2, indice + 3);
        }
    }
}
