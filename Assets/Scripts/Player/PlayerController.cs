using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int selectedSlotIndex = 0;
    public GameObject hotbarSelector;

    public GameObject handHolder;

    public Inventory inventory;
    public bool inventoryShowing = false;

    public ItemClass selectedItem;

    public int playerRange;
    public GameObject Cusor;
    public Vector2Int mousePos;


    public float moveSpeed;
    public float jumpForce;
    public bool onGround;

    public bool hit;
    public bool place;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    [HideInInspector]
    public Vector2 spawnPos;
    public TerrainGeneration terrainGenerator;
    private AudioManager audioManager;
    private void Awake()
    {
        terrainGenerator = FindObjectOfType<TerrainGeneration>();
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
    }

    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;
        //GetComponent<Transform>().position = new Vector3(0, 0, 0);
    }
    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float jump = Input.GetAxis("Jump");
        float vertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        if (horizontal > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (horizontal < 0)
            transform.localScale = new Vector3(1, 1, 1);

        if (vertical > 0.1f || jump > 0.1f)
        {
            if (onGround)
                movement.y = jumpForce;
        }
        rb.velocity = movement;

        hit = Input.GetMouseButton(0);
        place = Input.GetMouseButton(1);

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //cuon len
            if (selectedSlotIndex < inventory.inventoryWidth - 1)
                selectedSlotIndex += 1;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //cuon xuong
            if (selectedSlotIndex > 0)
                selectedSlotIndex -= 1;
        }
        //thiet lap o dc chon UI
        hotbarSelector.transform.position = inventory.hotbarUISlots[selectedSlotIndex].transform.position;
        if (selectedItem != null)
        {
            handHolder.GetComponent<SpriteRenderer>().sprite = selectedItem.sprite;
            if (selectedItem.itemType == ItemClass.ItemType.block)
                handHolder.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            else
                handHolder.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            handHolder.GetComponent<SpriteRenderer>().sprite = null;
        }

        //thiet lap item chon
        if (inventory.inventorySlots[selectedSlotIndex, inventory.inventoryHeight - 1] != null)
            selectedItem = inventory.inventorySlots[selectedSlotIndex, inventory.inventoryHeight - 1].item;
        else
            selectedItem = null;

        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryShowing = !inventoryShowing;
        }

        if (Vector2.Distance(transform.position, mousePos) <= playerRange &&
            Vector2.Distance(transform.position, mousePos) > 0.7f)
        {

            if (place)
            {
                if (selectedItem != null)
                {
                    if (selectedItem.itemType == ItemClass.ItemType.block)
                    {
                        if (terrainGenerator.CheckTile(selectedItem.tile, mousePos.x, mousePos.y, false))
                        {
                            inventory.Remove(selectedItem);
                            audioManager.PlayPlaceClip();
                        }
                    }

                }
            }
        }
        if(Vector2.Distance(transform.position, mousePos) <= playerRange)
        {
            if (hit)
            {
                terrainGenerator.BreakTile(mousePos.x, mousePos.y, selectedItem);
            }
        }
        //UI selector
        if (Vector2.Distance(transform.position, mousePos) <= playerRange)
        {
            Cusor.transform.position = new Vector3(mousePos.x + 0.5f, mousePos.y + 0.5f, Cusor.transform.position.z);
            Cusor.SetActive(true);
        }
        else
        {
            Cusor.SetActive(false);
        }
        mousePos.x = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - 0.5f);
        mousePos.y = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - 0.5f);
        inventory.inventoryUI.SetActive(inventoryShowing);

        animator.SetBool("hit", hit || place);
        animator.SetFloat("Walk", Mathf.Abs(horizontal));
    }
}
