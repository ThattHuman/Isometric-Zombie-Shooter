using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Player control </summary>
public class Player : Entity
{
    [SerializeField] private GameObject UIHealthBar = null;
    [SerializeField] private float slowdownForce = 4f;          // slowdown when get damage
    [SerializeField] private float slowdownTime = 2f;
    [SerializeField] private bool TestGizmos = false;
    
    private Image hBarImage;
    private Equipment equipment;
    
    private IPlayerObserver scenarioObserver = null;            // need to feedback via Scenario

    #region Properties
        /// <summary> Returns movement direction taken from Input </summary>
        private Vector3 MovementVector 
        {
            get
            {
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");

                return new Vector3(horizontal, 0f, vertical);
            }
        }
    #endregion

    // initialize personal fields
    protected override void Awake() 
    {
        base.Awake();                                       // initialize general fields
        equipment = GetComponentInChildren<Equipment>();
        hBarImage = UIHealthBar.GetComponent<Image>();
    }

    // check for Shot, Weapon change; update UI
    private void Update()
    {
        if(Input.GetButton("Fire1"))                    // pool the trigger of active weapon from equipmnet
        {
            equipment.GetCurrentWeapon().PoolTheTrigger(equipment);
        }

        // weapon change
        if(Input.mouseScrollDelta.y > 0)        // wheel up
            equipment.GetNextWeapon();
        else if(Input.mouseScrollDelta.y < 0)   // wheel down
            equipment.GetPreviousWeapon();

        UIUpdate();
    }

    // movement and rotation
    private void FixedUpdate()
    {
        Move(MovementVector);
        LookOnCursor();
    }

    /// <summary> Gradually turns object towards the cursor </summary>
    private void LookOnCursor()
    { 		
        Plane playerPlane = new Plane(Vector3.up, transform.position); 		
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 		
        float hitdist = 0;  		
        if(playerPlane.Raycast(ray, out hitdist)) 
        { 			
            LookOnTarget(ray.GetPoint(hitdist));
        } 	
    }
    
    /// <summary> Reaction to recieving melee attack </summary>
    public void GetMeleeDamage(float damage)
    {
        health -= damage;
        StartCoroutine(Slowdown());             // temporarily slows down the Player

        if(health <= 0)                         // try to stop game via Scenario, if player has died
        {
            scenarioObserver?.PlayerDied();
        }
    }

    /// <summary> Temporarily slows down the Player </summary>
    public IEnumerator Slowdown()
    {
        speed -= slowdownForce;
        yield return new WaitForSeconds(slowdownTime);
        speed += slowdownForce;
    }

    /// <summary> Set up scenario observer for feedback about status and events </summary>
    public void SetScenarioObserver(IPlayerObserver observer)
    {
        scenarioObserver = observer;
    }

    /// <summary> Updates UI linked to Player </summary>
    private void UIUpdate()
    {
        hBarImage.fillAmount = health / maxHealth;
    }

    /// <summary> Draw Gizmos for scene view in Unity Editor </summary>
    private void OnDrawGizmos() 
    {
        if(TestGizmos)
        {
            Gizmos.color = Color.red;
            Vector3 direction = transform.forward * 20;
            Gizmos.DrawRay(transform.position, direction);
        }    
    }
}