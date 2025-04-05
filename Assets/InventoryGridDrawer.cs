using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InventoryGridDrawer : MonoBehaviour
{
    private int width;
    private int height;
    public float thickness = 2f;
    private bool[,] occupiedSpaces;
    public Color lineColor = Color.black;
    private RectTransform rectTransform;
    public bool shop = false;

    GameObject Items;

    bool init = false;
    private Dictionary<DraggableItem, Vector2Int> itemSpaces;

    public void Init(int width, int height, bool shop, Dictionary<DraggableItem, Vector2Int> items = null)
    {
        this.width = width;
        this.height = height;
        
        rectTransform = GetComponent<RectTransform>();
        occupiedSpaces = new bool[width, height];

        itemSpaces = new Dictionary<DraggableItem, Vector2Int>();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                occupiedSpaces[x, y] = false;

        GameObject gridLinesContainer = new GameObject("Items");
        gridLinesContainer.transform.SetParent(transform, false);

        init = true;

        DrawGrid();


        if (items != null)
            foreach (KeyValuePair<DraggableItem, Vector2Int> T in items)
                if (!TryAddItem(T.Key, T.Value))
                    Debug.LogError($"Hey fuckass {T.Key.name} is in a bad postion, fix it");
    }

    public void DrawGrid()
    {
        if (!init)
            Debug.LogError("fuckass you are calling an inventory function without initing it first");
        

        GameObject gridLinesContainer = new GameObject("GridLines");
        gridLinesContainer.transform.SetParent(transform, false);

        float cellWidth = rectTransform.rect.width / (float)width;
        float cellHeight = rectTransform.rect.height / (float)height;
        Vector2 offset = new Vector2(-rectTransform.rect.width / 2, -rectTransform.rect.height / 2);

        for (int x = 0; x <= width; x++)
        {
            DrawLine(new Vector2(x * cellWidth, 0) + offset, new Vector2(x * cellWidth, rectTransform.rect.height) + offset, true, gridLinesContainer);
        }

        for (int y = 0; y <= height; y++)
        {
            DrawLine(new Vector2(0, y * cellHeight) + offset, new Vector2(rectTransform.rect.width, y * cellHeight) + offset, false, gridLinesContainer);
        }
    }

    void DrawLine(Vector2 start, Vector2 end, bool vertical, GameObject gridLinesContainer)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.SetParent(gridLinesContainer.transform, false);

        Image lineImage = lineObj.AddComponent<Image>();
        lineImage.color = lineColor;

        RectTransform lineRect = lineObj.GetComponent<RectTransform>();
        lineRect.anchorMin = lineRect.anchorMax = new Vector2(0, 1);
        lineRect.pivot = new Vector2(0.5f, 0.5f);

        Vector2 size = vertical ? new Vector2(thickness, rectTransform.rect.height) : new Vector2(rectTransform.rect.width, thickness);
        Vector2 position = (start + end) / 2;

        lineRect.sizeDelta = size;
        lineRect.anchoredPosition = position;
    }

    public Vector2Int? FindValidLocation(int x, int y, int boxWidth, int boxHeight)
    {
        // Push the starting coordinates to ensure they're within bounds
        while (x < 0) x++; // Push right if out of bounds left
        while (y < 0) y++; // Push up if below grid

        while (x + boxWidth > width) x--; // Push left if exceeding width
        while (y + boxHeight > height) y--; // Push down if exceeding height

        // Try nudging the box to find a valid location
        for (int offsetX = -width+boxWidth; offsetX < width-boxWidth; offsetX++)
        {
            for (int offsetY = -height+boxHeight; offsetY < height-boxHeight; offsetY++)
            {
                // Check if nudging the box to this position results in a valid space
                int newX = x + offsetX;
                int newY = y + offsetY;

                // Make sure the new position is within bounds
                if (newX < 0 || newY < 0 || newX + boxWidth > this.width || newY + boxHeight > this.height)
                {
                    continue;
                }

                // Now check if the space is occupied
                bool isValid = true;
                for (int i = newX; i < newX + boxWidth; i++)
                {
                    for (int j = newY; j < newY + boxHeight; j++)
                    {
                        if (occupiedSpaces[i, j]) // If any part is occupied, it's not a valid position
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (!isValid) break;
                }

                // If a valid position is found, return the new position
                if (isValid)
                {
                    return new Vector2Int(newX, newY);
                }
            }
        }

        // If no valid location is found, return null
        return null;
    }

    public bool TryAddItem(DraggableItem item)
    {
        item.transform.SetParent(transform.GetChild(0));//make the item a child of the item list


        float cellWidth = rectTransform.rect.width / (float)width;
        float cellHeight = rectTransform.rect.height / (float)height;

        Vector2 parentBottomLeft = new Vector2(-rectTransform.rect.width / 2, -rectTransform.rect.height / 2); // Bottom-left of parent


        RectTransform itemRect = item.GetComponent<RectTransform>();

        // Scale to fit the grid cells
        float scaleX = (item.width * cellWidth) / itemRect.sizeDelta.x;
        float scaleY = (item.height * cellHeight) / itemRect.sizeDelta.y;
        item.transform.localScale = new Vector3(scaleX, scaleY, 1);

        // Convert object's position from center-based to bottom-left-based
        Vector2 bottomLeft = (Vector2)item.transform.localPosition - new Vector2(itemRect.sizeDelta.x * scaleX / 2, itemRect.sizeDelta.y * scaleY / 2);

        // Convert to grid coordinates relative to bottom-left of parent
        int gridX = Mathf.RoundToInt((bottomLeft.x - parentBottomLeft.x) / cellWidth);
        int gridY = Mathf.RoundToInt((bottomLeft.y - parentBottomLeft.y) / cellHeight);

        // Try to push into bounds
        Vector2Int? pos = FindValidLocation(gridX, gridY, item.width, item.height);



        if (pos != null)//valid location found
        {
            gridY = pos.Value.y;
            gridX = pos.Value.x;
        }
        else
        {
            return false; //the item will handle going home and delte itself
        }

        // Convert adjusted grid coordinates back to world position
        Vector2 newBottomLeft = parentBottomLeft + new Vector2(gridX * cellWidth, gridY * cellHeight);
        Vector2 newCenterPos = newBottomLeft + new Vector2((item.width * cellWidth) / 2, (item.height * cellHeight) / 2);

        // Move the item to the corrected position
        item.transform.localPosition = newCenterPos;
        item.home = new Vector2Int(gridX, gridY);
        Debug.Log($"Item {item.name} placed at grid cell: ({gridX}, {gridY}) at position {newCenterPos}");



        Vector2Int setSpaces = new Vector2Int(gridX, gridY);
        for (int x = setSpaces.x; x < setSpaces.x + item.width; x++)
            for (int y = setSpaces.y; y < setSpaces.y + item.height; y++)
                occupiedSpaces[x, y] = true; //occupi grid


        item.home = setSpaces;
        itemSpaces[item] = setSpaces;//add item to dictionary of items in inventory

        return true;
    }
    public bool TryAddItem(DraggableItem item, Vector2Int posStart)
    {
        item.transform.SetParent(transform.GetChild(0));//make the item a child of the item list


        float cellWidth = rectTransform.rect.width / (float)width;
        float cellHeight = rectTransform.rect.height / (float)height;

        Vector2 parentBottomLeft = new Vector2(-rectTransform.rect.width / 2, -rectTransform.rect.height / 2); // Bottom-left of parent


        RectTransform itemRect = item.GetComponent<RectTransform>();

        // Scale to fit the grid cells
        float scaleX = (item.width * cellWidth) / itemRect.sizeDelta.x;
        float scaleY = (item.height * cellHeight) / itemRect.sizeDelta.y;
        item.transform.localScale = new Vector3(scaleX, scaleY, 1);


        // Convert to grid coordinates relative to bottom-left of parent
        int gridX = posStart.x;
        int gridY = posStart.y;

        // Try to push into bounds
        Vector2Int? pos = FindValidLocation(gridX, gridY, item.width, item.height);



        if (pos != null)//valid location found
        {
            gridY = pos.Value.y;
            gridX = pos.Value.x;
        }
        else
        {
            return false; //the item will handle going home and delte itself
        }

        // Convert adjusted grid coordinates back to world position
        Vector2 newBottomLeft = parentBottomLeft + new Vector2(gridX * cellWidth, gridY * cellHeight);
        Vector2 newCenterPos = newBottomLeft + new Vector2((item.width * cellWidth) / 2, (item.height * cellHeight) / 2);

        // Move the item to the corrected position
        item.transform.localPosition = newCenterPos;
        item.home = new Vector2Int(gridX, gridY);
        Debug.Log($"Item {item.name} placed at grid cell: ({gridX}, {gridY}) at position {newCenterPos}");



        Vector2Int setSpaces = new Vector2Int(gridX, gridY);
        for (int x = setSpaces.x; x < setSpaces.x + item.width; x++)
            for (int y = setSpaces.y; y < setSpaces.y + item.height; y++)
                occupiedSpaces[x, y] = true; //occupi grid


        item.home = setSpaces;
        itemSpaces[item] = setSpaces;//add item to dictionary of items in inventory

        return true;
    }

    public void RemoveItem(DraggableItem item)
    {
        if (itemSpaces.ContainsKey(item))
        {
            Vector2Int clearSpaces = itemSpaces[item];
            for (int x = clearSpaces.x; x < clearSpaces.x + item.width; x++)
            {
                for (int y = clearSpaces.y; y < clearSpaces.y + item.height; y++)
                {
                    try
                    {
                        occupiedSpaces[x, y] = false;
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"item {item.name} {e.Message}");
                        Debug.LogException(e);
                    }
                }
            }

            itemSpaces.Remove(item);
        }
        else
        {
            Debug.LogError("trying to remove item not in this inventory. How did we get here?");
        }
    }
}
