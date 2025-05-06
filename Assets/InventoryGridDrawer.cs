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

    float cellWidth;
    float cellHeight;

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


        cellWidth = rectTransform.rect.width / (float)width;
        cellHeight = rectTransform.rect.height / (float)height;

        DrawGrid();
    }
    #region Draw
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
    #endregion
    public Vector2Int? FindValidLocation(int x, int y, int boxWidth, int boxHeight)
    {
        if (CheckSpot(x, y, boxWidth, boxHeight))
            return new Vector2Int(x, y);

        int maxRadius = Mathf.Max(width, height);

        for (int r = 1; r <= maxRadius; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                int dy = r - Mathf.Abs(dx);
                foreach (int sign in new int[] { 1, -1 })
                {
                    int newX = x + dx;
                    int newY = y + dy * sign;

                    if (CheckSpot(newX, newY, boxWidth, boxHeight))
                        return new Vector2Int(newX, newY);
                }
            }
        }

        return null; // No valid position found
    }
    private bool CheckSpot(int x, int y, int boxWidth, int boxHeight)
    {
        if (x < 0 || y < 0 || x + boxWidth > width || y + boxHeight > height)
            return false;

        for (int i = x; i < x + boxWidth; i++)
            for (int j = y; j < y + boxHeight; j++)
                if (occupiedSpaces[i, j])
                    return false;

        return true;
    }


    public Vector2Int? TryAddItem(DraggableItem item, Vector2Int posStart)
    {
        Debug.LogWarning("you used this 2nd function");

        item.transform.SetParent(transform.GetChild(0));//make the item a child of the item list



        Vector2 parentBottomLeft = new Vector2(-rectTransform.rect.width / 2, -rectTransform.rect.height / 2); // Bottom-left of parent


        RectTransform itemRect = item.GetComponent<RectTransform>();

        // Scale to fit the grid cells
        float scaleX = (item.width * cellWidth) / itemRect.sizeDelta.x;
        float scaleY = (item.height * cellHeight) / itemRect.sizeDelta.y;
        item.transform.localScale = new Vector3(scaleX, scaleY, 1);

        // Try to push into bounds
        Vector2Int? pos = FindValidLocation(posStart.x, posStart.y, item.width, item.height); //TODO, just change to an item class when you update the globals



        if (pos == null)
            return null;

        Vector2Int setSpaces = (Vector2Int)pos;

        // Convert adjusted grid coordinates back to world position
        Vector2 anchoredPosition = new Vector2(
            (setSpaces.x + item.width / 2f) * cellWidth,
            (setSpaces.y + item.height / 2f) * cellHeight
        );

        // Convert to local position relative to anchor/pivot
        item.transform.localPosition = parentBottomLeft + anchoredPosition;


        Debug.Log($"Item {item.name} placed at grid cell: ({setSpaces.x}, {setSpaces.y}) at position {item.transform.localPosition}");


        for (int x = setSpaces.x; x < setSpaces.x + item.width; x++)
            for (int y = setSpaces.y; y < setSpaces.y + item.height; y++)
                occupiedSpaces[x, y] = true; //occupi grid


        itemSpaces[item] = setSpaces;//add item to dictionary of items in inventory

        return setSpaces;
    }

    public void RemoveItem(DraggableItem item)
    {
        Debug.LogWarning("This fuction was also called");

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

    public Vector2Int GetGridPositionFromLocal(Vector2 localPosition)
    {
        // Shift localPosition from center-based to bottom-left-based
        float adjustedX = localPosition.x + rectTransform.rect.width / 2f;
        float adjustedY = localPosition.y + rectTransform.rect.height / 2f;

        int x = Mathf.FloorToInt(adjustedX / cellWidth);
        int y = Mathf.FloorToInt(adjustedY / cellHeight);

        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);

        return new Vector2Int(x, y);
    }


}
