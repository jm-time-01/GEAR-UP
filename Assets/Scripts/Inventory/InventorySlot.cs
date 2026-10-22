
using System;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public bool IsEmpty()
    {
        return item == null;
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
    }
}
