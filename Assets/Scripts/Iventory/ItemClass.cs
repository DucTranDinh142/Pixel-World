using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ItemClass
{
    public enum ItemType
    {
        block,
        tool
    };
    public enum ToolType
    {
        none,
        axe,
        pickaxe,
        hammer,
        unbreakable
    };

    public ItemType itemType;
    public ToolType toolType;

    public string itemName;
    public Sprite sprite;
    public bool isStackable;

    public TileClass tile;
    public ToolClass tool;
    public ItemClass (TileClass _tile)
    {
        itemName = _tile.tileName;
        sprite = _tile.tileDrop.tileSprites[0];
        isStackable = _tile.isStackable;
        itemType = ItemType.block;
        tile = _tile;
    }
    public ItemClass(ToolClass _tool)
    {
        itemName = _tool.name;
        sprite = _tool.sprite;
        isStackable = false;
        itemType = ItemType.tool;
        toolType = _tool.toolType;
        tool = _tool;
    }
}
