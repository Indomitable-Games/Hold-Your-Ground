using Assets.Scripts;
using Assets.Scripts.Objects;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    //This class udpated on-screen components
    public GameObject resourcePreFab;
    private Vector3 startPos = new Vector3(-1260, 900, 0);
    
    public List<GameObject> _resources;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos.y -= (resourcePreFab.transform as RectTransform).rect.height * .75f;
        foreach (Resource resource in Globals.Player.PlayerResources.Keys)
        {
            _resources.Add(Instantiate(resourcePreFab));
            _resources[^1].transform.SetParent(this.transform);

            _resources[^1].transform.localPosition = startPos;
            startPos.y -= 2.5f * (_resources[^1].transform as RectTransform).rect.height;
            _resources[^1].GetComponentInChildren<Image>().sprite = resource.tile.sprite;

            _resources[^1].name = resource.Name;
            foreach (TextMeshProUGUI text in _resources[^1].GetComponentsInChildren<TextMeshProUGUI>())
            {
                if(text.name.Equals("Name"))
                {
                    text.text = resource.Name + ":";
                }
            }
        }

    }

    public void UpdateResource(Resource resource)
    {
        _resources.First(x => x.name.Equals(resource.Name)).GetComponentsInChildren<TextMeshProUGUI>().First(x => x.name.Equals("Number")).text = resource.Total.ToString();
    }
}
