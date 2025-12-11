using System.Collections;
using UnityEngine;

[System.Serializable]
public class BiomeClass
{
    public string biomeName; //tên quần xã sinh vật
    public Color bioCol; // màu đại diện quần xã

    public TileAtlas tileAtlas; //Chứa các TileClass (grass, dirt, stone, log, leaf...) để vẽ terrain.
    [Header("Noise Settings")]
    public Texture2D caveNoiseTexture; //Texture dùng để sinh hang động (nếu generateCaves = true).

    [Header("Genneration Settings")]
    public bool generateCaves = true; //Nếu true, biome sẽ có hang động.
    public int dirtLayerHeight = 5; //Độ dày lớp đất (số block đất bên trên lớp đá).
    public float surfaceValue = 0.25f; //Ngưỡng để xác định hang động (giá trị Perlin Noise > surfaceValue → hang động).
    public float heightMultiplier = 4f; //Nhân độ cao địa hình (biome cao hơn nếu giá trị lớn).

    [Header("Tree")]
    public int treeChance = 10; //Tỷ lệ xuất hiện cây
    public int minTreeHeight = 4;
    public int maxTreeHeight = 6;

    [Header("Addons")]
    public int tallGrassChance = 10; //Tỷ lệ xuất hiện cỏ

    [Header("Ore Settings")]
    public OreClass[] ores; //Mảng các loại quặng (coal, iron, gold...) xuất hiện trong biome.
}
