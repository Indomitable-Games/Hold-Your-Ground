using Assets.Scripts;

using NUnit.Framework;

using System.Collections.Generic;

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

    public GameObject shop;

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
        InventoryGridDrawer[] x = shop.GetComponentsInChildren<InventoryGridDrawer>();
        int shopfirst = 1;
        if(x[0].name == "Shop")
            shopfirst = 0;
        List<GameObject> shopItems = new List<GameObject>();
        int name = 1;
        Dictionary<DraggableItem,Vector2Int> shopDict = new Dictionary<DraggableItem,Vector2Int>(); ;
        foreach (Vector2Int[] items in Globals.shop[Globals.shopIndex])
        {
            GameObject i = new GameObject($"test {name}");
            i.AddComponent<Image>();
            name++;
            //i.transform.SetParent(x[shopfirst].transform, false);
            DraggableItem d = i.AddComponent<DraggableItem>();
            d.Init(items[0], Color.blue);
            shopDict[d] = items[1];
        }
        x[shopfirst].Init(7, 7, true,shopDict);
        foreach (KeyValuePair<DraggableItem,Vector2Int> t in shopDict)
        {
            t.Key.ReInit();
        }
        if (shopfirst == 1)
            shopfirst = 0;
        else
            shopfirst = 1;
        x[shopfirst].Init(7, 7, false);
        shop.SetActive(true);
    }
}
