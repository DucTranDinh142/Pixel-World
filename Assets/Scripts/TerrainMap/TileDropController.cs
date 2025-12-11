using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDropController : MonoBehaviour
{
    public ItemClass item;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            //them vao tui do
            if(col.GetComponent<Inventory>().Add(item))
                Destroy(this.gameObject);
            //Xoa
        }
    }
}
