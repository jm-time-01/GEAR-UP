
using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class InteractableHighlight : MonoBehaviour
{
    [Header("Cores do contorno")]
    public Color pickupColor = new Color(1f, 0.8f, 0.05f, 1f);
    public Color repairColor = new Color(0.05f, 0.9f, 1f, 1f);

    [Header("Configuração")]
    [Min(0.001f)]
    public float thickness = 5f;

    private readonly List<GameObject> outlineObjects = new();
    private readonly List<Material> outlineMaterials = new();

    private static Shader outlineShader;

    private bool isHighlighted;

    void Awake()
    {
        outlineShader ??= Shader.Find("GEAR UP/Interactable Outline");

        if (outlineShader == null)
        {
            Debug.LogError("Shader de contorno não encontrado. " + "Verifique o nome do shader.",this);
            return;
        }

        BuildOutline();
        SetHighlighted(false);
    }

    void BuildOutline()
    {
        // Guarda os renderers antes de criar os objetos de contorno.
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer source in renderers)
        {
            if (source == null) continue;

            // Não criar contorno sobre os próprios contornos.
            if (source.GetComponent<OutlinePart>() != null) continue;

            MeshFilter sourceFilter = source.GetComponent<MeshFilter>();

            if (sourceFilter == null || sourceFilter.sharedMesh == null) continue;

            GameObject outline = new GameObject(source.gameObject.name + "_Outline");

            outline.transform.SetParent(source.transform, false);
            outline.transform.localPosition = Vector3.zero;
            outline.transform.localRotation = Quaternion.identity;
            outline.transform.localScale = Vector3.one;

            outline.AddComponent<OutlinePart>();

            MeshFilter filter = outline.AddComponent<MeshFilter>();
            filter.sharedMesh = sourceFilter.sharedMesh;

            MeshRenderer renderer = outline.AddComponent<MeshRenderer>();

            Material material = new Material(outlineShader);
            material.SetFloat("_OutlineThickness", thickness);
            material.SetColor("_OutlineColor", pickupColor);

            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            outlineObjects.Add(outline);
            outlineMaterials.Add(material);

            // Evita renderizar o contorno enquanto não selecionado.
            outline.SetActive(false);
        }
    }

    public void SetHighlighted(bool highlighted)
    {
        isHighlighted = highlighted;

        Interactable interactable = GetComponent<Interactable>();

        Color color = pickupColor;

        if (interactable != null && interactable.kind == InteractableKind.Repairable)
        {
            color = repairColor;
        }

        for (int i = 0; i < outlineObjects.Count; i++)
        {
            if (outlineObjects[i] == null) continue;

            if (outlineMaterials[i] != null)
            {
                outlineMaterials[i].SetColor("_OutlineColor", color); 
                outlineMaterials[i].SetFloat("_OutlineThickness", thickness);
            }

            outlineObjects[i].SetActive(highlighted);
        }
    }

    void OnDisable()
    {
        SetHighlighted(false);
    }

    void OnDestroy()
    {
        foreach (Material material in outlineMaterials)
        {
            if (material != null)
                Destroy(material);
        }
    }
}

// Marca os objetos criados pelo sistema de contorno.
public class OutlinePart : MonoBehaviour
{
}
