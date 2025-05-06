using UnityEngine;
using Assets.Scripts; // Make sure this points to where your Globals class is

public static class GameBootstrap
{
    private static bool initialized = false;

    // This runs automatically before any scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        if (!initialized)
        {
            Globals.LoadEverything(); // your custom loading logic
            initialized = true;
            Debug.Log("GameBootstrap: Globals.LoadEverything() called");
        }
    }
}
