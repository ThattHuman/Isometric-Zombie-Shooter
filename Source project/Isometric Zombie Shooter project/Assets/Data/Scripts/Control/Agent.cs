using System.Collections;
using UnityEngine;

/// <summary> Agent control </summary>
public class Agent : Entity
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float kickbackForce = 2f;
    [SerializeField] private float deadKickbackForce = 10f;
    [SerializeField] private float dieTime = 4f;
    [SerializeField] private Color aliveColor = new Color(190, 250, 0);
    [SerializeField] private Color deadColor = new Color(145, 164, 87);

    private Material agentMaterial = null;          // need to changing color
    private Transform targetTransform = null;
    private int id = 0;
    private IAgentSpawnMessenger observer = null;   // need to feedback via AgentSpawner

    private bool isReadyToAttack = true;            // prevent from attack, when cooldown is active
    private bool died = false;                      // prevent from repeat "die-actions"

    #region Properties
    /// <summary> Returns position of agents's Target </summary>
        public Vector3 TargetPosition
        {
            get
            {
                return targetTransform.position;
            }
        }
    #endregion

    // initialize personal fields
    protected override void Awake() 
    {
        base.Awake();                                       // initialize general fields
        agentMaterial = GetComponent<Renderer>().material;
    }

    // movement and rotation
    private void FixedUpdate() 
    {
        if(health > 0)                      // prevents the movement of dead agents
        {
            Chase();
            LookOnTarget(TargetPosition);
        }
    }

    /// <summary> Move the Agent towards its Target </summary>
    private void Chase()
    {
        Move(TargetPosition - transform.position);
    }

    /// <summary> Reaction to recieving shoot </summary>
    public void GetShoot(float damage)
    {
        health -= damage;

        if(health <= 0)         // start "die"-actions
        {
            if(died)                        // if already have "die"-coroutine, than just kickback
                rb.AddForce((transform.position - TargetPosition).normalized * deadKickbackForce, ForceMode.Impulse);
            else
                StartCoroutine(Die());         
        }
        else                    // if agent don't die after shoot, it's a bit kickback
        {
            rb.AddForce((transform.position - TargetPosition).normalized * kickbackForce, ForceMode.Impulse);
        }
    }

    /// <summary> Deactivate agent and after some delay remove it </summary>
    private IEnumerator Die()
    {
        died = true;
        isReadyToAttack = false;    // prevent from attack, when cooldown is active
        ChangeColor(deadColor);
        rb.constraints = RigidbodyConstraints.None;     // turn off lock on X and Z rotation
        rb.AddForce((transform.position - TargetPosition).normalized * deadKickbackForce, ForceMode.Impulse);
        yield return new WaitForSeconds(dieTime);
        observer.RemoveAgent(id);
    }

    /// <summary> Set up Agent parammeters for his respawn </summary>
    public void SetUp(int _id, IAgentSpawnMessenger spawnObserver)
    {
        ChangeColor(aliveColor);
        rb.constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationZ; // turn on lock on X and Z rotation
        id = _id;                   // id for removing
        health = maxHealth;         // returns full HP
        observer = spawnObserver;   // set up observer for feedback
        died = false;
        isReadyToAttack = true;     // prevent from attack, when cooldown is active
    }

    /// <summary> Set up Agent's Target </summary>
    public void SetTarget(Transform _transform)
    {
        targetTransform = _transform;
    }

    /// <summary> Change Agent's color </summary>
    private void ChangeColor(Color color)
    {
        agentMaterial.color = color;
    }

    /// <summary> Try to attack Player </summary>
    private void OnCollisionStay(Collision other) 
    {
        if(isReadyToAttack)         // prevent from attack, when cooldown is active
        {
            Player player = other.gameObject.GetComponent<Player>();
            if(player != null)      // attack only if it's Player's collider
            {
                player.GetMeleeDamage(damage);
                StartCoroutine(WaitCooldown());
            }
        }
    }

    /// <summary> Start attack cooldown </summary>
    private IEnumerator WaitCooldown()
    {
        isReadyToAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        isReadyToAttack = true;
    }
}
