
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Identificação do slot")]
    public bool isHotbar;
    public int slotIndex;

    [Header("Referências")]
    public Image icon;

    private Canvas canvas;
    private Image draggedImage;
    private RectTransform draggedRect;
    private InventoryManager manager;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        manager = InventoryManager.Instance;

        if (icon == null)
            icon = GetComponentInChildren<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        manager = InventoryManager.Instance;

        if (manager == null || canvas == null)
            return;

        InventorySlot slot = manager.GetSlot(isHotbar, slotIndex);

        if (slot == null || slot.IsEmpty())
            return;

        GameObject ghost = new GameObject("DraggedItemIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

        ghost.transform.SetParent(canvas.rootCanvas.transform, false);
        ghost.transform.SetAsLastSibling();

        draggedImage = ghost.GetComponent<Image>();
        draggedImage.sprite = slot.item.icon;
        draggedImage.preserveAspect = true;
        draggedImage.raycastTarget = false;

        draggedRect = ghost.GetComponent<RectTransform>();
        draggedRect.sizeDelta = icon != null ? icon.rectTransform.rect.size : new Vector2(64, 64);

        UpdateDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedRect == null)
            return;

        UpdateDraggedPosition(eventData);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedRect == null)
            return;

        InventoryManager inventoryManager = InventoryManager.Instance;

        bool handled = false;

        if (inventoryManager != null && EventSystem.current != null)
        {
            // Detecta todos os elementos UI sob o mouse.
            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(eventData, results);

            InventorySlotUI targetSlot = null;

            foreach (RaycastResult result in results)
            {
                InventorySlotUI candidate = result.gameObject.GetComponentInParent<InventorySlotUI>();

                if (candidate != null && candidate != this)
                {
                    targetSlot = candidate;
                    break;
                }
            }

            // Se encontrou outro slot, troca os itens.
            if (targetSlot != null)
            {
                inventoryManager.MoveOrSwap(isHotbar, slotIndex, targetSlot.isHotbar, targetSlot.slotIndex);

                handled = true;
            }
            // Se não encontrou slot, verifica se soltou fora dos painéis.
            else if (inventoryManager.IsOutsideInventoryUI(eventData.position))
            {
                handled = inventoryManager.DropItemFromSlot(isHotbar, slotIndex);
            }
        }

        if (!handled)
            Debug.Log("Item mantido no slot original.");

        Destroy(draggedRect.gameObject);

        draggedRect = null;
        draggedImage = null;
    }


    private void UpdateDraggedPosition(PointerEventData eventData)
    {
        if (draggedRect == null)
            return;

        draggedRect.position = eventData.position;
    }
}
