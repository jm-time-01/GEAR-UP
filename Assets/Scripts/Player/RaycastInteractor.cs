using UnityEngine;
using UnityEngine.UI;

public class RaycastInteractor : MonoBehaviour
{
    [Tooltip("Distância máxima do raycast para interagir")]
    public float maxDistance = 3f;

    [Tooltip("LayerMask para objetos interagíveis. Por padrão tudo")]
    public LayerMask interactLayer = ~0;

    public KeyCode interactKey = KeyCode.E;

    [Tooltip("Referência ao componente de UI que mostra nome/prompt/progresso")]
    public RaycastUI ui;

    [Header("Integração com inventário")]
    public InventoryUI inventoryUI;

    private Interactable currentInteractable;
    private float repairTimer = 0f;

    private Interactable highlightedInteractable;
    void Awake()
    {
        // Define por padrão a LayerMask para a layer chamada "Interactable" se existir
        int mask = LayerMask.GetMask("Interactable");
        if (mask != 0)
        {
            interactLayer = mask;
        }
        else
        {
            Debug.LogWarning("Layer 'Interactable' não encontrada. Verifique Layers no project settings.", this);
        }
    }


    void Update()
    {
        // Não interage com objetos enquanto o inventário estiver aberto.
        if (inventoryUI != null &&
            inventoryUI.inventoryPanel != null &&
            inventoryUI.inventoryPanel.activeSelf)
        {
            currentInteractable = null;
            repairTimer = 0f;

            UpdateHighlight(null);

            if (ui != null)
                ui.Hide();

            return;
        }

        DoRaycast();
        HandleInput();
    }



    void DoRaycast()
    {
        currentInteractable = null;

        var cam = Camera.main;

        if (cam == null)
        {
            UpdateHighlight(null);

            if (ui != null)
                ui.Hide();

            repairTimer = 0f;
            return;
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray,out RaycastHit hit,maxDistance,interactLayer))
        {
            Interactable interact = hit.collider.GetComponentInParent<Interactable>();

            if (interact != null && interact.isActiveAndEnabled)
            {
                currentInteractable = interact;
            }
        }

        // Atualiza o contorno somente quando o alvo mudar.
        UpdateHighlight(currentInteractable);

        if (currentInteractable == null)
        {
            if (ui != null)
                ui.Hide();

            repairTimer = 0f;
            return;
        }

        if (currentInteractable.kind == InteractableKind.Pickup)
        {
            if (ui != null)
            {
                ui.Show(currentInteractable.displayName, $"Pressione '{interactKey}' para coletar");

                ui.SetProgress(0f);
            }
        }
        else if (currentInteractable.kind == InteractableKind.Repairable)
        {
            if (ui != null)
            {
                if (currentInteractable.isRepaired)
                {
                    ui.Show(currentInteractable.displayName, "Reparado");
                    ui.SetProgress(1f);
                }
                else
                {
                    float pct = Mathf.Clamp01(repairTimer / Mathf.Max(0.0001f, currentInteractable.repairDuration));

                    ui.Show(currentInteractable.displayName, $"Segure '{interactKey}' para reparar " + $"({Mathf.RoundToInt(pct * 100)}%)");

                    ui.SetProgress(pct);
                }
            }
        }
    }


    void UpdateHighlight(Interactable target)
    {
        if (highlightedInteractable == target)
            return;

        if (highlightedInteractable != null)
        {
            InteractableHighlight previous =
                highlightedInteractable.GetComponent<InteractableHighlight>();

            if (previous != null)
            {
                Debug.Log("Desativando outline: " + highlightedInteractable.name);
                previous.SetHighlighted(false);
            }
        }

        highlightedInteractable = target;

        if (highlightedInteractable != null)
        {
            InteractableHighlight highlight =
                highlightedInteractable.GetComponent<InteractableHighlight>();

            if (highlight != null)
            {
                Debug.Log("Ativando outline: " + highlightedInteractable.name);
                highlight.SetHighlighted(true);
            }
            else
            {
                Debug.LogWarning(
                    "O objeto não possui InteractableHighlight: " +
                    highlightedInteractable.name
                );
            }
        }
    }
    void HandleInput()
    {
        if (currentInteractable == null) return;

        if (currentInteractable.kind == InteractableKind.Pickup)
        {
            if (Input.GetKeyDown(interactKey))
            {
                currentInteractable.Pickup();
                ui.Hide();
            }
        }
        else if (currentInteractable.kind == InteractableKind.Repairable)
        {
            if (currentInteractable.isRepaired) return;

            if (Input.GetKey(interactKey))
            {
                repairTimer += Time.deltaTime;
                float pct = Mathf.Clamp01(repairTimer / currentInteractable.repairDuration);
                ui.SetProgress(pct);
                if (repairTimer >= currentInteractable.repairDuration)
                {
                    currentInteractable.FinishRepair();
                    ui.Show(currentInteractable.displayName, "Reparado");
                    ui.SetProgress(1f);
                    repairTimer = 0f;
                }
            }
            else if (Input.GetKeyUp(interactKey) || !Input.GetKey(interactKey))
            {
                // soltou antes de completar
                if (repairTimer > 0f)
                {
                    repairTimer = 0f;
                    ui.SetProgress(0f);
                }
            }
        }
    }
}
