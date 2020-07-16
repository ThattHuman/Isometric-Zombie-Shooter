using UnityEngine;

/// <summary> Spawn bullets </summary>
public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab = null;    // prefab for object pool
    [SerializeField] private int poolCapacity = 30;             
    [SerializeField] private float bulletDistanceLimit = 100f;  // distance after which bullet will be deactivated

    private ObjectPool bullets = null;  // object pool with bullets

    private void Awake()
    {
        bullets = new ObjectPool(bulletPrefab, poolCapacity, GetComponent<Transform>());
    }

    /// <summary> Spawn new bullet and send it in direction </summary>
    public void SpawnBullet(Vector3 position, Vector3 direction, float damage)
    {
        int id = bullets.ActivateObject(position);
        Bullet spawnedBullet = bullets[id].GetComponent<Bullet>();
        spawnedBullet.Destruction += RemoveBullet;                          // subscribe on event of bullet's destruction
        spawnedBullet.SetUp(id, damage, direction, bulletDistanceLimit);
    }

    /// <summary> Deactivate bullet by ID </summary>
    public void RemoveBullet(int id)
    {
        bullets[id].GetComponent<Bullet>().Destruction -= RemoveBullet;     // unsubscribe on event of bullet's destruction
        bullets.DeactivateObject(id);
    }
}
