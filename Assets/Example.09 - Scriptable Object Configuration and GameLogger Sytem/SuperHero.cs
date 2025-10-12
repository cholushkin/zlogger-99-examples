using UnityEngine;
using Logger = Example09.Logger;

public class SuperHero : MonoBehaviour
{
    public Logger Logger;

    public void Start()
    {
        Logger.Info(() => "Initializing SuperHero");
        Logger.Warn(() => "Hero spawned on the scene");
    }
}