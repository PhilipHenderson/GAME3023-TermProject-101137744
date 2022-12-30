using UnityEngine;

// Add More Enemies Types Here
public enum EnemyType { Baby, Teen, FullGrown }
// Add More Element Types Here
public enum ElementType { Fire, Ice, Wind }

public class Unit : MonoBehaviour
{
    protected Unit playerUnit;
    protected Unit enemyUnit;

    public FireBlast fireBlast;
    public WindCannon windCannon;
    public IcePistol icePistol;
    public BasicAttack basicAttack;
    public Flee flee;

    [Header("Unit Info")]
    public string Unitname;
    public int Unitlvl;
    public EnemyType enemyType;
    public ElementType elementType;
    public int dmg;
    public int defendedDmg;
    public int maxHp;
    public int currentHp;
    public int maxMp;
    public int currentMp;

    public BattleSystems battleSystems;

    [SerializeField]
    public GameObject slider;

    public int lingeringDamageTurns = 0;
    public bool isDefending = false;
    public bool isOnFire = false;
    public bool isBlownAway = false;
    public bool isFrozen = false;

    private void Awake()
    {
        fireBlast = FindObjectOfType<FireBlast>();
        windCannon = FindObjectOfType<WindCannon>();
        icePistol = FindObjectOfType<IcePistol>();
        basicAttack = FindObjectOfType<BasicAttack>();
        flee = FindObjectOfType<Flee>();
    }

    private void Update()
    {
    }

    public bool TakeDamage(int dmg)
    {
        currentHp -= dmg;
        Debug.Log("currentHP: " + currentHp);
        if (currentHp <= 0) return true;
        else return false;
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        if(currentHp > maxHp)
            currentHp = maxHp;
    }
}