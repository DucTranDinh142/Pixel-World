using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "Tile Class")]
public class TileClass : ScriptableObject
{
    public string tileName;

    public TileClass wallVariant;
    //public Sprite tileSprite;
    public Sprite[] tileSprites;
    public bool inBackground;
    public TileClass tileDrop;
    public ItemClass.ToolType toolToBreak;

    public bool isStackable = true;
    public bool naturallyPlaced = true;

    public static TileClass CreateInstance(TileClass tile, bool isNaturallyPlaced)
    {
        var thisTile = ScriptableObject.CreateInstance<TileClass>();
        thisTile.Init(tile, isNaturallyPlaced);
        return thisTile;
    }

    public void Init(TileClass tile, bool isNaturallyPlaced)
    {
        tileName = tile.tileName;
        wallVariant = tile.wallVariant;
        tileSprites = tile.tileSprites;
        tileDrop = tile.tileDrop;
        isStackable = tile.isStackable;
        toolToBreak = tile.toolToBreak;
        naturallyPlaced = isNaturallyPlaced;
        inBackground = tile.inBackground;
    }
}
