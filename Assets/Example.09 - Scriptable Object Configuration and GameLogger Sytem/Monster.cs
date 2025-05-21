using UnityEngine;
using Logger = Example09.Logger;

public class Monster : MonoBehaviour
{
    public Logger Logger; 

    public void Start()
    {
        Logger.Info(()=> "Initializing Monster");
    }
}
