using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    public bool stackable;
    public int maxStack = 1;

    public GameObject worldPrefab;


    [Header("Equipamento na mão")]
    public GameObject heldPrefab;

    public Vector3 heldPosition = Vector3.zero;
    public Vector3 heldRotation = Vector3.zero;

}
