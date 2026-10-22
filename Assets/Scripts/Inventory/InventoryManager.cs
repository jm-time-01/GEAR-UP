using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Configuração")]
    public int hotbarSize = 4;
    public int inventorySize = 11;

    [Header("Slots")]
    public InventorySlot[] hotbar;
    public InventorySlot[] inventory;

    [Header("UI")]
    public HotbarUI hotbarUI;

    public InventoryPanelUI inventoryPanelUI;

    [Header("Arrastar e soltar")]
    public Transform dropOrigin;
    public RectTransform inventoryPanelRect;
    public RectTransform hotbarPanelRect;

    void Awake()
    {
        Instance = this;

        hotbar = new InventorySlot[hotbarSize];
        inventory = new InventorySlot[inventorySize];

        for (int i = 0; i < hotbar.Length; i++)
        {
            hotbar[i] = new InventorySlot();
        }

        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = new InventorySlot();
        }
    }

    public bool AddItem(ItemData item)
    {
        // Primeiro tenta a Hotbar
        for (int i = 0; i < hotbar.Length; i++)
        {
            if (hotbar[i].IsEmpty())
            {
                hotbar[i].item = item;
                hotbar[i].quantity = 1;

                Debug.Log(item.itemName + " foi colocado na Hotbar.");

                UpdateUI();

                return true;
            }
        }

        // Depois tenta o Inventário
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].IsEmpty())
            {
                inventory[i].item = item;
                inventory[i].quantity = 1;

                Debug.Log(item.itemName + " foi colocado no Inventário.");

                UpdateUI();

                return true;
            }
        }

        Debug.Log("Hotbar e Inventário estão cheios!");

        return false;
    }

    public void UpdateUI()
    {
        if (hotbarUI != null)
        {
            hotbarUI.UpdateHotbar();
        }

        if (inventoryPanelUI != null)
        {
            inventoryPanelUI.UpdateInventory();
        }
    }

  

    public InventorySlot GetSlot(bool isHotbar, int index)
    {
        InventorySlot[] slots = isHotbar ? hotbar : inventory;

        if (slots == null || index < 0 || index >= slots.Length)
            return null;

        return slots[index];
    }

    public void MoveOrSwap(bool sourceIsHotbar, int sourceIndex, bool targetIsHotbar, int targetIndex)
    {
        InventorySlot source = GetSlot(sourceIsHotbar, sourceIndex);
        InventorySlot target = GetSlot(targetIsHotbar, targetIndex);

        if (source == null || target == null || source == target || source.IsEmpty())
            return;

        // Troca item e quantidade, inclusive entre hotbar e inventário.
        ItemData tempItem = source.item;
        int tempQuantity = source.quantity;

        source.item = target.item;
        source.quantity = target.quantity;

        target.item = tempItem;
        target.quantity = tempQuantity;

        UpdateUI();
    }

    public bool IsOutsideInventoryUI(Vector2 screenPosition)
    {
        bool insideInventory = inventoryPanelRect != null && RectTransformUtility.RectangleContainsScreenPoint(inventoryPanelRect, screenPosition, null);

        bool insideHotbar = hotbarPanelRect != null && RectTransformUtility.RectangleContainsScreenPoint(hotbarPanelRect, screenPosition, null);

        return !insideInventory && !insideHotbar;
    }

    public bool DropItemFromSlot(bool isHotbar, int index)
    {
        InventorySlot slot = GetSlot(isHotbar, index);

        if (slot == null || slot.IsEmpty())
            return false;

        if (slot.item.worldPrefab == null)
        {
            Debug.LogWarning("O item " + slot.item.itemName + " não possui um World Prefab configurado.");
            return false;
        }

        Transform origin = dropOrigin;

        if (origin == null && Camera.main != null)
            origin = Camera.main.transform;

        if (origin == null)
        {
            Debug.LogWarning("Configure o Drop Origin ou a câmera principal.");
            return false;
        }

        Vector3 spawnPosition = origin.position + origin.forward * 1.5f;

        GameObject droppedItem = Instantiate(slot.item.worldPrefab, spawnPosition, Quaternion.identity);

        // Descarta apenas uma unidade por vez.
        if (slot.quantity > 1)
        {
            slot.quantity--;
        }
        else
        {
            slot.Clear();
        }

        UpdateUI();
        return true;
    }

}