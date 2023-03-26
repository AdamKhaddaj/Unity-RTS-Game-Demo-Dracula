using UnityEngine;
using UnityEngine.AI;

public abstract class PlayerUnit : Unit
{
    [SerializeField] public PlayerUnitBlueprint blueprint = null;

    private bool selected;

    protected Vector3 destination;

    protected new Rigidbody rigidbody;

    protected Animator animator;

    private Color orig;

    private void Awake()
    {
        // unit setup
        Setup(blueprint.health);

        // deselected by default
        Deselect();

        destination = transform.position;

        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        agent.autoBraking = false;
        agent.acceleration = 90;
        agent.angularSpeed = 500;

        orig = transform.GetChild(0).GetComponent<Renderer>().material.color;
    }

    protected void Start()
    {
        // temporary unit adding
        PlayerManager.instance.AddUnit(this);

        agent.speed = 10.0f;
    }

    protected void Update()
    {

        //temp fix for while PlayerUnit contains both dynamic and static units
        if ((GetComponent<PlayerPylon>() != null) || (GetComponent<PlayerDie>() != null))
        {
            return;
        }

        // NavMeshCode
        if (blueprint.movable && Vector3.Distance(transform.position, destination) > 1f)
        {
            agent.isStopped = false;
            agent.SetDestination(destination);
        }
        else
        {
            agent.isStopped = true;
            agent.SetDestination(transform.position);
        }

    }

    public void Select()
    {
        selected = true;
        transform.GetChild(0).GetComponent<Renderer>().material.color = blueprint.color * 1.1f;
    }

    public void Deselect()
    {
        selected = false;
        transform.GetChild(0).GetComponent<Renderer>().material.color = blueprint.color * 0.9f;
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
    }

    public abstract void Action1();
    public abstract void Action2();
    public abstract void Action3();
    public abstract void Action4();
    public abstract void Action5();

    // level up unit
    public new bool LevelUp()
    {
        if (PlayerManager.instance.GetCrystals() >= blueprint.levelUpCost)
        {
            PlayerManager.instance.RemoveCrystals(blueprint.levelUpCost);

            base.LevelUp();

            Debug.Log("LEVEL UP!");

            return true;
        }

        return false;
    }

    // harvest unit for crystals
    public void Harvest()
    {
        PlayerManager.instance.RemoveUnit(GetID());
        PlayerManager.instance.AddCrystals(blueprint.cost);

    }
}