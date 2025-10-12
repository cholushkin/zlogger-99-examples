using UnityEngine;

namespace Example11
{
    public class Monster : MonoBehaviour
    {
        public Logger Logger; 
        private int _bulletCount;
        
        public void Start()
        {
            Logger.Info(()=> "Initializing Monster");
            Logger.Warn(()=> "Some monster warning");
            InvokeRepeating(nameof(ShootBullet), 1f, 1f);
        }
        
        void ShootBullet()
        {
            Logger.Info(() => $"shoot bullet {_bulletCount++}");
        }
    }
}
