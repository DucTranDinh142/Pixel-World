using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int stackLimit = 64;
    public ToolClass start_Pickaxe;

    public ToolClass start_Axe;
    
    public ToolClass start_Hammer;

    public Vector2 inventoryOffset;
    public Vector2 hotbarOffset;
    public Vector2 multiplier;

    public GameObject inventoryUI;
    public GameObject hotbarUI;
    public GameObject inventorySlotPrefab;

    public int inventoryWidth;
    public int inventoryHeight;
    public InventorySlot[,] inventorySlots;
    public InventorySlot[] hotbarSlots;

    public GameObject[,] uiSlots;
    public GameObject[] hotbarUISlots;
    public void Start()
    {
        inventorySlots = new InventorySlot[inventoryWidth, inventoryHeight];
        uiSlots = new GameObject[inventoryWidth, inventoryHeight];
        hotbarSlots = new InventorySlot[inventoryWidth];
        hotbarUISlots = new GameObject[inventoryWidth];
        SetupUI();
        UpdateInventoryUI();

        Add(new ItemClass(start_Pickaxe));
        Add(new ItemClass(start_Axe));
        Add(new ItemClass(start_Hammer));
    }
    void SetupUI()
    {
        //thiet lap ruong do
        for(int x = 0; x < inventoryWidth; x++)
        {
            for(int y = 0; y < inventoryHeight; y++)
            {
                GameObject inventorySlot = Instantiate(inventorySlotPrefab, inventoryUI.transform.GetChild(0).transform);
                inventorySlot.GetComponent<RectTransform>().localPosition = new Vector3((x * multiplier.x) + inventoryOffset.x, (y * multiplier.y) + inventoryOffset.y);
                uiSlots[x, y] = inventorySlot;
                inventorySlots[x, y] = null;
            }
        }
        // thiet lap thanh cong cu
        for (int x = 0; x < inventoryWidth; x++)
        {
            GameObject hotbarSlot = Instantiate(inventorySlotPrefab, hotbarUI.transform.GetChild(0).transform);
            hotbarSlot.GetComponent<RectTransform>().localPosition = new Vector3((x * multiplier.x) + hotbarOffset.x, hotbarOffset.y);
            hotbarUISlots[x] = hotbarSlot;
            hotbarSlots[x] = null;
        }
    }
    void UpdateInventoryUI()
    {
        //Cap nhat ruong do
        for (int x = 0; x < inventoryWidth; x++)
        {
            for (int y = 0; y < inventoryHeight; y++)
            {
                if (inventorySlots[x, y] == null)
                {
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().sprite = null;
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().enabled= false;

                    uiSlots[x, y].transform.GetChild(1).GetComponent<Text>().text = "0";
                    uiSlots[x, y].transform.GetChild(1).GetComponent<Text>().enabled = false;
                }
                else
                {
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[x, y].item.sprite;
                    if(inventorySlots[x, y].item.itemType == ItemClass.ItemType.block)
                    {
                        if (inventorySlots[x, y].item.tile.inBackground)
                        {
                            uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, -.5f, 0.5f);
                        }
                        else
                        {
                            uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                        }
                    }

                    uiSlots[x, y].transform.GetChild(1).GetComponent<Text>().text = inventorySlots[x, y].quantity.ToString();
                    uiSlots[x, y].transform.GetChild(1).GetComponent<Text>().enabled = true;
                }
            }
        }
        //cap nhat thanh cong cu
        for (int x = 0; x < inventoryWidth; x++)
        {
            if (inventorySlots[x, inventoryHeight - 1] == null)
            {
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().sprite = null;
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().enabled = false;

                hotbarUISlots[x].transform.GetChild(1).GetComponent<Text>().text = "0";
                hotbarUISlots[x].transform.GetChild(1).GetComponent<Text>().enabled = false;
            }
            else
            {
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().enabled = true;
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[x, inventoryHeight - 1].item.sprite;

                if (inventorySlots[x, inventoryHeight - 1].item.itemType == ItemClass.ItemType.block)
                {
                    if (inventorySlots[x, inventoryHeight - 1].item.tile.inBackground)
                    {
                        hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, -.5f, 0.5f);
                    }
                    else
                    {
                        hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    }
                }

                hotbarUISlots[x].transform.GetChild(1).GetComponent<Text>().text = inventorySlots[x, inventoryHeight - 1].quantity.ToString();
                hotbarUISlots[x].transform.GetChild(1).GetComponent<Text>().enabled = true;
            }

        }
    }
    public bool Add(ItemClass item)
    {
        Vector2Int itemPos = Contains(item);
        bool added = false;
        if (Contains(item) != Vector2.one * -1)
        {
            inventorySlots[itemPos.x, itemPos.y].quantity += 1;
            added = true;
        }

        if (!added)
        {
            for (int y = inventoryHeight - 1; y >= 0; y--)
            {
                if (added)
                    break;
                for (int x = 0; x < inventoryWidth; x++)
                {
                    //neu day la slot trong
                    if (inventorySlots[x, y] == null)
                    {
                        //day la slot trong
                        inventorySlots[x, y] = new InventorySlot { item = item, position = new Vector2Int(x, y), quantity = 1 };
                        added = true;
                        break;
                    }
                }
            }
        }
        UpdateInventoryUI();
        return added;
    }

    public Vector2Int Contains(ItemClass item)
    {
        for(int y = inventoryHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if (inventorySlots[x, y] != null)
                {
                    if (inventorySlots[x, y].item.itemName == item.itemName)
                    {
                        //Debug.Log(item.isStackable);
                        if(item.isStackable && inventorySlots[x, y].quantity < stackLimit)
                            return new Vector2Int(x, y);
                    }
                }
            }
        }
        return Vector2Int.one * -1;
    }
    public bool Remove(ItemClass item)
    {
        for (int y = inventoryHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if (inventorySlots[x, y] != null)
                {
                    if (inventorySlots[x, y].item.itemName == item.itemName)
                    {
                        Debug.Log(item.itemName);
                        Debug.Log(inventorySlots[x, y].item.itemName);
                        inventorySlots[x, y].quantity -= 1;

                        if (inventorySlots[x, y].quantity == 0)
                            inventorySlots[x, y] = null;

                        UpdateInventoryUI();
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
