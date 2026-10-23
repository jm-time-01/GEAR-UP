using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    [Header("Referências")]
    public Transform holdPoint;
    public InventoryManager inventoryManager;
    public InventoryUI inventoryUI;

    [Header("Estado")]
    public int selectedSlot = -1;

    private GameObject equippedObject;
    private ItemData equippedItem;

    private void Start()
    {
        if (inventoryManager == null)
            inventoryManager = InventoryManager.Instance;

        EquipSlot(-1);
    }

    private void Update()
    {
        if (inventoryManager == null || holdPoint == null)
            return;

        // Não troca ferramentas enquanto o inventário está aberto.
        if (inventoryUI != null && inventoryUI.inventoryPanel != null && inventoryUI.inventoryPanel.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            EquipSlot(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            EquipSlot(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            EquipSlot(2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            EquipSlot(3);
    }

    public void EquipSlot(int index)
    {
        if (inventoryManager == null || holdPoint == null)
            return;

        // Selecionar o slot atual novamente não faz nada.
        if (index == selectedSlot && equippedItem != null)
            return;

        selectedSlot = index;

        // Remove a ferramenta anteriormente equipada.
        if (equippedObject != null)
            Destroy(equippedObject);

        equippedObject = null;
        equippedItem = null;

        // -1 significa mãos vazias.
        if (index < 0 || index >= inventoryManager.hotbar.Length)
            return;

        InventorySlot slot = inventoryManager.hotbar[index];

        if (slot == null || slot.IsEmpty())
            return;

        ItemData item = slot.item;

        // O prefab da mão é independente do prefab do chão.
        GameObject prefab = item.heldPrefab;

        if (prefab == null)
        {
            Debug.LogWarning("O item " + item.itemName + " não possui Held Prefab configurado.");
            return;
        }

        equippedObject = Instantiate(prefab, holdPoint);
        equippedObject.transform.localPosition = item.heldPosition;
        equippedObject.transform.localRotation = Quaternion.Euler(item.heldRotation);

        // Desativa colisões e física enquanto estiver equipado.
        foreach (Collider col in equippedObject.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        foreach (Rigidbody rb in equippedObject.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        equippedItem = item;
    }

    public void Unequip()
    {
        selectedSlot = -1;

        if (equippedObject != null)
            Destroy(equippedObject);

        equippedObject = null;
        equippedItem = null;
    }
}


