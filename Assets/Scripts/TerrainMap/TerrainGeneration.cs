using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public PlayerController player;
    public CamController camera1;
    public GameObject tileDrop;

    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;
    public float seed;

    public BiomeClass[] biomes;

    [Header("Biomes")]
    public float biomeFrequency;
    public Gradient biomeGradient;
    public Texture2D biomeMap;

    [Header("Genneration Settings")]
    public int chunkSize = 16;
    public int worldSize = 100;
    public float heightAddition = 25;
    public bool generateCaves = true;

    //[Header("Ore Settings")]
    public OreClass[] ores;

    [Header("Noise Settings")]
    public float terrainFreq = 0.5f;
    public float caveFreq = 0.05f;
    public Texture2D caveNoiseTexture;



    private GameObject[] worldChuncks;

    private GameObject[,] world_ForeGroundObjects;
    private GameObject[,] world_BackgroundObjects;
    private TileClass[,] world_BackgroundTiles;
    private TileClass[,] world_ForegroundTiles;

    private BiomeClass curBiome;
    private Color[] biomeCols;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    private void Start()
    {
        world_ForegroundTiles = new TileClass[worldSize, worldSize];
        world_BackgroundTiles = new TileClass[worldSize, worldSize];
        world_ForeGroundObjects = new GameObject[worldSize, worldSize];
        world_BackgroundObjects = new GameObject[worldSize, worldSize];
        //tao dia hinh
        seed = Random.Range(-10000, 10000);
        for(int i = 0; i < ores.Length; i++)
        {
            ores[i].spreadTexture = new Texture2D(worldSize, worldSize);
        }

        biomeCols = new Color[biomes.Length];
        for (int i = 0; i < biomes.Length; i++)
        {
            biomeCols[i] = biomes[i].bioCol;
        }

        DrawBiomeMap();
        DrawCavesAndOres();

        CreateChunks();
        GenerateTerrain();

        camera1.Spawn(new Vector3(player.spawnPos.x, player.spawnPos.y, camera1.transform.position.z));
        camera1.worldSize = worldSize;
        player.Spawn();

        RefreshChunks();
    }

    private void Update()
    {
        RefreshChunks();
    }

    void RefreshChunks()
    {
        for(int i = 0; i < worldChuncks.Length; i++)
        {
            if(Vector2.Distance(new Vector2(i * chunkSize, 0), new Vector2(player.transform.position.x, 0)) > Camera.main.orthographicSize * 5.7f)
                worldChuncks[i].SetActive(false);
            else
                worldChuncks[i].SetActive(true);
        }
    }

    public void DrawBiomeMap()
    {
        float b;
        Color col;
        biomeMap = new Texture2D(worldSize, worldSize);
        for (int x = 0; x < biomeMap.width; x++)
        {
            for (int y = 0; y < biomeMap.height; y++)
            {
                b = Mathf.PerlinNoise((x + seed) * biomeFrequency, (y + seed) * biomeFrequency);
                col = biomeGradient.Evaluate(b);
                biomeMap.SetPixel(x, y, col);
            }
        }
        biomeMap.Apply();
    }

    public void DrawCavesAndOres()
    {
        float o;
        float v;
        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        for(int x = 0; x < caveNoiseTexture.width; x++)
        {
            for(int y = 0; y < caveNoiseTexture.height; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq);
                if (v > curBiome.surfaceValue)
                    caveNoiseTexture.SetPixel(x, y, Color.white);
                else
                    caveNoiseTexture.SetPixel(x, y, Color.black);

                for (int i = 0; i < ores.Length; i++)
                {
                    ores[i].spreadTexture.SetPixel(x, y, Color.black);
                    if (curBiome.ores.Length >= i + 1)
                    {
                        o = Mathf.PerlinNoise((x + seed) * curBiome.ores[i].frequency, (y + seed) * curBiome.ores[i].frequency);
                        if (o > curBiome.ores[i].size)
                            ores[i].spreadTexture.SetPixel(x, y, Color.white);

                        ores[i].spreadTexture.Apply();
                    }
                }
            }
        }

        caveNoiseTexture.Apply();
    }

    public void GenerateNoiseTextures(float frequency, float limit ,Texture2D noiseTexture)
    {
        float v;
        for(int x = 0; x < noiseTexture.width; x++)
        {
            for(int y = 0; y < noiseTexture.height; y++)
            {
                v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if (v > limit)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.black);
            }
        }
        noiseTexture.Apply();
        biomeMap.Apply();
    }

    public void CreateChunks()
    {
        int numChunks = worldSize / chunkSize;
        worldChuncks = new GameObject[numChunks];
        for(int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = this.transform;
            worldChuncks[i] = newChunk;
        }
    }

    public BiomeClass GetCurrentBiome(int x, int y)
    {

        if(System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y)) > 0)
        {
            return biomes[System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y))];
        }

        return curBiome;
    }
    public void GenerateTerrain()
    {
        TileClass tileClass;
        for(int x = 1; x < worldSize - 1; x++)
        {
            float height;

            for (int y = 1; y < worldSize - 1; y++)
            {

                curBiome = GetCurrentBiome(x, y);
                height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * curBiome.heightMultiplier + heightAddition;
                if (x == worldSize / 2)
                    player.spawnPos = new Vector2(x, height + 2); // Spawn nguoi choi
                if (y >= height)
                    break;
                if (y < height - curBiome.dirtLayerHeight)
                {
                    tileClass = curBiome.tileAtlas.stone;
                    
                    if (ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[0].maxSpawHeight)
                        tileClass = tileAtlas.coal;
                    if (ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[1].maxSpawHeight)
                        tileClass = tileAtlas.iron;
                    if (ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[2].maxSpawHeight)
                        tileClass = tileAtlas.gold;
                    if (ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[3].maxSpawHeight)
                        tileClass = tileAtlas.diamond;
                }
                else if(y < height - 1)
                {
                    tileClass = curBiome.tileAtlas.dirt;
                }
                else
                {
                    //layer tren cung
                    tileClass = curBiome.tileAtlas.grass;
                }

                if (generateCaves)
                {
                    if (caveNoiseTexture.GetPixel(x, y).r > 0.5f)
                    {
                        PlaceTile(tileClass, x, y, true);
                    }
                    else if(tileClass.wallVariant != null)
                    {
                        PlaceTile(tileClass.wallVariant, x, y, true);
                    }
                }
                else
                {
                    PlaceTile(tileClass, x, y, true);
                }
                if(y > height - 1)
                {
                    int t = Random.Range(0, curBiome.treeChance);
                    if (t == 1)
                    {
                        if (GetTileFromWorld(x, y))
                        {
                            if (curBiome.biomeName == "Desert")
                            {   //Tao xuong rong
                                GenerateCactus(curBiome.tileAtlas, Random.Range(curBiome.minTreeHeight, curBiome.maxTreeHeight), x, y + 1);
                            }
                            else
                            {   //Tao cay
                                GenerateTree(Random.Range(curBiome.minTreeHeight, curBiome.maxTreeHeight), x, y + 1);
                            }
                                        
                        }        
                    }
                    else
                    {
                        int i = Random.Range(0, curBiome.tallGrassChance);
                        //Tao co
                        if (i == 1)
                        {
                            if (GetTileFromWorld(x, y))
                            {
                                if(curBiome.tileAtlas.tallGrass != null)
                                    PlaceTile(curBiome.tileAtlas.tallGrass, x, y + 1, true);
                            }
                        }
                    }
                }
            }
        }
        //Tao bedrock
        for (int x = 0; x < worldSize; x++)
        {
            PlaceTile(tileAtlas.Bedrock, x, 0, true);  //Dưới cùng
            PlaceTile(tileAtlas.Bedrock, x, worldSize - 1, true); // Trên cùng
        }

        // Bao phủ cột trái và phải bằng bedrock
        for (int y = 0; y < worldSize; y++)
        {
            PlaceTile(tileAtlas.Bedrock, 0, y, true); // Bên trái
            PlaceTile(tileAtlas.Bedrock, worldSize - 1, y, true); // Bên phải
        }
    }

    void GenerateCactus(TileAtlas atlas ,int treeHeight, int x, int y)
    {

        //Than cay
        for (int i = 0; i < treeHeight; i++)
        {
            PlaceTile(atlas.log, x, y + i, true);
        }
    }
    void GenerateTree(int treeHeight,int x, int y)
    {
        
        //Than cay
        for(int i = 0; i < treeHeight; i++)
        {
            PlaceTile(tileAtlas.log, x, y + i, true);
        }
        //La cay
        PlaceTile(tileAtlas.leaf, x, y + treeHeight, true);
        PlaceTile(tileAtlas.leaf, x, y + treeHeight + 1, true);
        PlaceTile(tileAtlas.leaf, x, y + treeHeight + 2, true);

        PlaceTile(tileAtlas.leaf, x - 1, y + treeHeight, true);
        PlaceTile(tileAtlas.leaf, x - 1, y + treeHeight + 1, true);

        PlaceTile(tileAtlas.leaf, x + 1, y + treeHeight, true);
        PlaceTile(tileAtlas.leaf, x + 1, y + treeHeight + 1, true);
    }

    public bool BreakTile(int x, int y, ItemClass item)
    {
        if (GetTileFromWorld(x, y) && x <= worldSize && x >= 0 && y >= 0 && y <= worldSize)
        {
            TileClass tile = GetTileFromWorld(x, y);
            if (tile.toolToBreak == ItemClass.ToolType.none)
            {
                RemoveTile(x, y);
                //auodio
                //audioManager.PlayDestroyClip();
                return true;
            }
            else
            {
                if (item != null)
                {
                    if (item.itemType == ItemClass.ItemType.tool)
                    {
                        if (tile.toolToBreak == item.toolType)
                        {
                            RemoveTile(x, y);
                            //audio
                            //audioManager.PlayDestroyClip();
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public void RemoveTile(int x, int y)
    {

        if (GetTileFromWorld(x, y) && x <= worldSize && x >= 0 && y >= 0 && y <= worldSize)
        {
            TileClass tile = GetTileFromWorld(x, y);
            if (tile == null) return;

            
            if (tile.tileDrop != null)
            {
                GameObject newTileDrop = Instantiate(tileDrop, new Vector2(x, y + 0.5f), Quaternion.identity);
                newTileDrop.GetComponent<SpriteRenderer>().sprite = tile.tileDrop.tileSprites[0];
                ItemClass tileDropItem = new ItemClass(tile.tileDrop);
                newTileDrop.GetComponent<TileDropController>().item = tileDropItem;
                audioManager.PlayDestroyClip();
            }

            RemoveTileFromWorld(x, y);
            GameObject obj = GetObjectFromWorld(x, y);
            if (obj != null)
                Destroy(obj);

            RemoveObjectFromWorld(x, y);
            if (tile.wallVariant != null && tile.naturallyPlaced)
            {
                PlaceTile(tile.wallVariant, x, y, true);
            }
        }
    }

    public bool CheckTile(TileClass tile, int x, int y, bool isNaturallyPlaced)
    {
        //bool backgroundElement;
        if (x <= worldSize && x >= 0 && y >= 0 && y <= worldSize)
        {
            if (tile.inBackground)
            {
                if (GetTileFromWorld(x + 1, y) ||
                    GetTileFromWorld(x - 1, y) ||
                    GetTileFromWorld(x, y + 1) ||
                    GetTileFromWorld(x, y - 1))
                {//kiểm tra các khối xung quanh nếu tồn tại thì đặt được bên cạnh
                    if (!GetTileFromWorld(x, y))
                    {
                        PlaceTile(tile, x, y, isNaturallyPlaced);
                        //Debug.Log(tile.tileName);
                        return true;
                    }
                }
            }
            else
            {
                if (GetTileFromWorld(x + 1, y) ||
                    GetTileFromWorld(x - 1, y) ||
                    GetTileFromWorld(x, y + 1) ||
                    GetTileFromWorld(x, y - 1))
                {
                    if (!GetTileFromWorld(x, y))
                    {
                        PlaceTile(tile, x, y, isNaturallyPlaced);
                        Debug.Log(tile.tileName);
                        return true;
                    }
                    else
                    {
                        if (GetTileFromWorld(x, y).inBackground)
                        {
                            PlaceTile(tile, x, y, isNaturallyPlaced);
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
    public void PlaceTile(TileClass tile, int x, int y, bool isNatuarallyPlaced)
    {
        if (x <= worldSize && x >= 0 && y >= 0 && y <= worldSize)
        {
            GameObject newTile = new GameObject();

            int chunkCoord = Mathf.RoundToInt(Mathf.Round(x / chunkSize) * chunkSize);
            chunkCoord /= chunkSize;
            newTile.transform.parent = worldChuncks[chunkCoord].transform;

            newTile.AddComponent<SpriteRenderer>();

            int spriteIndex = Random.Range(0, tile.tileSprites.Length);
            newTile.GetComponent<SpriteRenderer>().sprite =  tile.tileSprites[spriteIndex];

            if (tile.inBackground)
            {
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -10;

                if (tile.name.ToLower().Contains("wall"))
                {
                    newTile.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.6f, 0.6f);
                    
                }
            }
            else
            {
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -5;
                newTile.AddComponent<BoxCollider2D>();
                newTile.GetComponent<BoxCollider2D>().size = Vector2.one;
                newTile.tag = "Ground";
                newTile.layer = LayerMask.NameToLayer("Ground");
            }

            newTile.name = tile.tileSprites[0].name;
            newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

            TileClass newTileClass = TileClass.CreateInstance(tile, isNatuarallyPlaced);

            AddObjectToWorld(x, y, newTile, newTileClass);
            AddTileToWorld(x, y, newTileClass);
        }
    }

    void AddTileToWorld(int x, int y, TileClass tile)
    {
        if (tile.inBackground)
        {
            world_BackgroundTiles[x, y] = tile;
        }
        else
        {
            world_ForegroundTiles[x, y] = tile;
        }
    }
    void AddObjectToWorld(int x, int y, GameObject tileObject, TileClass tile)
    {
        if (tile.inBackground)
        {
            world_BackgroundObjects[x, y] = tileObject;
        }
        else
        {
            world_ForeGroundObjects[x, y] = tileObject;
        }
    }
    void RemoveTileFromWorld(int x, int y)
    {
        if (world_ForegroundTiles[x, y] != null)
        {
            world_ForegroundTiles[x, y] = null;
        }
        else if(world_BackgroundTiles[x, y] != null)
        {
            world_BackgroundTiles[x, y] = null;
        }

    }
    void RemoveObjectFromWorld(int x, int y)
    {
        if (world_ForeGroundObjects[x, y] != null)
        {
            world_ForeGroundObjects[x, y] = null;
        }
        else if (world_BackgroundObjects[x, y] != null)
        {
            world_BackgroundObjects[x, y] = null;
        }
    }
    GameObject GetObjectFromWorld(int x, int y)
    {
        if (world_ForeGroundObjects[x, y] != null)
        {
            return world_ForeGroundObjects[x, y];
        }
        else if (world_BackgroundObjects[x, y] != null)
        {
            return world_BackgroundObjects[x, y];
        }

        return null;
    }
    TileClass GetTileFromWorld(int x, int y)
    {
        if (world_ForegroundTiles[x, y] != null)
        {
            return world_ForegroundTiles[x, y];
        }
        else if (world_BackgroundTiles[x, y] != null)
        {
            return world_BackgroundTiles[x, y];
        }

        return null;
    }
}
