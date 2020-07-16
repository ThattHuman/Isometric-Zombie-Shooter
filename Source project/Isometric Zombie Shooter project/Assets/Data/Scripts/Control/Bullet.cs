using UnityEngine;

/// <summary> Bullet control </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 2.4f;        // distance per FixedUpdate

    private int id = 0;                                 // id for object pool
    private float damage = 0f;                          // keep it to damage target
    private Vector3 direction = Vector3.zero;           // direction of movement
    private float distanceLimit = 100f;                 // distance after which bullet will be deactivated 
    private bool isReady = false;                       // prevent movement and raycasting before setting up

    private Ray ray;                                
    private RaycastHit hit;
    private const float rayLengthModifier = 1.2f;

    public delegate void BulletDestroyHandler(int id);
    public event BulletDestroyHandler Destruction;       // notice that bullet is destructed (hit ot move from limit)

    private void FixedUpdate()
    {
        if(Mathf.Abs(transform.position.x) > distanceLimit ||
            Mathf.Abs(transform.position.y) > distanceLimit ||
            Mathf.Abs(transform.position.z) > distanceLimit)    // check for reach limit
        {
            Destruction(id);
            return;
        }
        
        if(isReady) // prevent movement and raycasting before setting up
        {
            Vector3 lastPos = transform.position;                   // keep last position
            transform.position += direction.normalized * speed;     // move bullet

            ray = new Ray(transform.position, (lastPos - transform.position));   // ray from bullet to last position                                //transform.position - lastPos
            Physics.Raycast(ray, out hit, (lastPos - transform.position).magnitude * rayLengthModifier);

            if(hit.collider?.gameObject.GetComponent<Agent>() != null)  // if hit in Agent - damage him and deactivate
            {
                isReady = false;
                Agent target = hit.collider.gameObject.GetComponent<Agent>();
                target?.GetShoot(damage);

                Destruction(id);
            }
        }
    }

    /// <summary> Set up bullet parameters </summary>
    public void SetUp(int _id, float dmg, Vector3 dir, float distance_limit)
    {
        id = _id;
        damage = dmg;
        direction = dir;
        distanceLimit = distance_limit;
        isReady = true;
    }
}
