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
}
