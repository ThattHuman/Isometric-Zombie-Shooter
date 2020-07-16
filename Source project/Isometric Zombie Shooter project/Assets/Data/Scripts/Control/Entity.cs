using UnityEngine;

/// <summary> Unite general functions of entities </summary>
[RequireComponent(typeof(Rigidbody))]
public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected float health = 10f;
    [SerializeField] protected float speed = 2f;            // speed of movement

    protected Rigidbody rb = null;
    protected float maxHealth = 10f;
    protected float lookspeed = 10f;                        // speed of rotating to target

    private const float speed_modify = 0.01f;               // allow to use more pretty numbers for speed

    protected virtual void Awake()
    {
        maxHealth = health;
        rb = GetComponent<Rigidbody>();
    }
    
    /// <summary> Move entity in the direction </summary>
    protected void Move(Vector3 movementVector)
    {
        Vector3 movement = transform.position + movementVector.normalized * speed_modify * speed;
        rb.MovePosition(movement);
    }

    /// <summary> Rotate entity in the direction </summary>
    protected void LookOnTarget(Vector3 targetPosition)
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);  			
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookspeed * Time.deltaTime); 
    }
}
