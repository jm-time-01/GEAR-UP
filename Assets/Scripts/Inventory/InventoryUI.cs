
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public MouseController mouseController;

    void Start()
    {
        inventoryPanel.SetActive(false);
        mouseController.SetCursorLocked(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) ||
            Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        bool isOpen = inventoryPanel.activeSelf;

        inventoryPanel.SetActive(!isOpen);

        // Se estava aberto, agora fecha e trava o mouse.
        // Se estava fechado, agora abre e libera o mouse.
        mouseController.SetCursorLocked(isOpen);
    }
}
