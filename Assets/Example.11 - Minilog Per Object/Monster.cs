// using Microsoft.Extensions.Logging;
// using UnityEngine;
//
// namespace Example11
// {
//     public class Monster : MonoBehaviour
//     {
//         public LoggerWithMinilogSupport Logger; 
//         private int _bulletCount;
//         
//         public void Start()
//         {
//             Logger.Log(LogLevel.Information, $"Initializing Monster");
//             Logger.Log(LogLevel.Warning, $"Initializing Monster problems");
//             InvokeRepeating(nameof(ShootBullet), 1f, 1f);
//         }
//         
//         void ShootBullet()
//         {
//             Logger.Log(LogLevel.Information, $"Shoot bullet{_bulletCount}");
//             _bulletCount++;
//         }
//     }
// }
