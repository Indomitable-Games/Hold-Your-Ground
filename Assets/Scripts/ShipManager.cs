using Assets.Scripts;

using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShipManager : MonoBehaviour
{
    public float bobSpeed = 1f; // Speed of the bobbing motion
    public float bobHeight = 10f; // Maximum height variation
    public float maxOffset = 20f; // Maximum height above the parent

    private RectTransform rectTransform;
    private Vector3 startPos;
    private float randomBobHeight;
    private float timeOffset;

    public GameObject DropShipInvetory;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
        GenerateNewBobHeight();
        timeOffset = Random.Range(0f, Mathf.PI * 2f); // Randomize start phase
    }

    void Update()
    {
        float bobAmount = Mathf.Sin(Time.time * bobSpeed + timeOffset) * randomBobHeight;
        bobAmount = Mathf.Clamp(bobAmount, -bobHeight, maxOffset);

        rectTransform.anchoredPosition = startPos + new Vector3(0, bobAmount, 0);

        // Change bob height every full cycle (when sin curve resets)
        if (Mathf.Abs(Mathf.Sin(Time.time * bobSpeed + timeOffset)) < 0.1f)
        {
            GenerateNewBobHeight();
        }
    }

    void GenerateNewBobHeight()
    {
        randomBobHeight = Random.Range(bobHeight * 0.5f, bobHeight);
    }

    public void LaunchShip()
    {
        SceneManager.LoadScene("Level");
    }

    public void LoadShop()
    {
        DropShipInvetory.GetComponentsInChildren<InventoryGridDrawer>().FirstOrDefault(x => x.name != "Shop").Init(7, 7, false); //init the player inventory

        InventoryGridDrawer shop = DropShipInvetory.GetComponentsInChildren<InventoryGridDrawer>().FirstOrDefault(x => x.name == "Shop"); //get the shop

        shop.Init(7, 7, true); //init the shop


        int name = 1;

        /*foreach (Vector2Int[] item in Globals.shop[Globals.shopIndex]) //for each intem in shop at shopindex
        {
            GameObject i = new GameObject($"test {name}"); //make a gameobject for item
            i.AddComponent<Image>(); //give it an image object (set to sprite, if preab wont need to do this)
            name++;

            DraggableItem d = i.AddComponent<DraggableItem>();

            d.Init(item[0], Color.blue, item[1]);

            var check = shop.TryAddItem(d, item[1]);
            if (check == null || check != item[1])
                Debug.LogError("didnt place item at correct spot in shop");
            
            d.Init(item[0], Color.blue, item[1]);

        }*/


        this.DropShipInvetory.SetActive(true);
    }
}
