using Microsoft.Extensions.Logging;
using UnityEngine;

public class SuperHero : MonoBehaviour
{
    public Example09.Logger Logger;

    public void Start()
    {
        // Logger.Log(LogLevel.Information,$"Initializing SuperHero");
        // Logger.Log(LogLevel.Warning,$"Hero has some init problems");
    }
}