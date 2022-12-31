using System.Collections;
using UnityEngine;

// Add More Enemies Types Here
public enum EnemyType { Baby, Teen, FullGrown }
// Add More Element Types Here
public enum ElementType { Fire, Ice, Wind }

public class Unit : MonoBehaviour
{
    protected Unit playerUnit;
    protected Unit enemyUnit;

    [Header("Elemental Abilities")]
    public AbilitiesScript fireBlast;
    public AbilitiesScript windCannon;
    public AbilitiesScript icePistol;

    [Header("Basic Abilities")]
    public AbilitiesScript attack;
    public AbilitiesScript heal;
    public AbilitiesScript flea;

    [Header("Unit Info")]
    public string Unitname;
    public int Unitlvl;
    public EnemyType enemyType;
    public ElementType elementType;
    public int basicAttackDmg;
    public int maxHp;
    public int currentHp;
    public int hpRecoverAmount;
    public int maxMp;
    public int currentMp;
    public int mpRecoverAmount;

    public BattleSystems battleSystems;

    [SerializeField]
    public GameObject slider;

    public int lingeringDamageTurns = 0;
    public int lingeringDamage = 0;
    public bool isDefending = false;
    public bool isOnFire = false;
    public bool isBlownAway = false;
    public bool isFrozen = false;

    private void Awake()
    {
        // Abilities
        fireBlast = FindObjectOfType<AbilitiesScript>();
        windCannon = FindObjectOfType< AbilitiesScript>();
        icePistol = FindObjectOfType<AbilitiesScript>();

        // Regular Attack, Heal, Flea
        attack = FindObjectOfType<AbilitiesScript>();
        flea = FindObjectOfType<AbilitiesScript>();
        heal = FindObjectOfType<AbilitiesScript>();
    }

    public IEnumerator Attack(Unit target, int dmg)
    {
        target.currentHp -= dmg;

        yield return new WaitForSeconds(2.0f);
    }
    public IEnumerator Flea(int chance)
    {
        if (chance < 3)
        {
            //Flea is successfull
            battleSystems.encounterTxt.text = "Flea Attempt Was Successfull!";

            yield return new WaitForSeconds(2.0f);
        }
        if (chance >= 3)
        {
            //Flea is Unsuccessfull
            battleSystems.encounterTxt.text = "Flea Attempt Was UnSuccessfull!";
            yield return new WaitForSeconds(2.0f);
        }
    }
    public void Heal()
    {
        currentHp += hpRecoverAmount;
        if(currentHp > maxHp)
            currentHp = maxHp;
    }

    public void UseFireBlast(Unit target)
    {
        // Use the ability's damage and lingering damage values to reduce the target's HP
        target.currentHp -= fireBlast.damage;
        target.lingeringDamage += fireBlast.lingeringDamage;
        target.lingeringDamageTurns = fireBlast.lingeringDamageTurnsLeft;
    }

    public void UseIcePistol(Unit target)
    {
        // Use the ability's damage and lingering damage values to reduce the target's HP
        target.currentHp -= icePistol.damage;
        target.lingeringDamage += icePistol.lingeringDamage;
        target.lingeringDamageTurns = icePistol.lingeringDamageTurnsLeft;
    }

    public IEnumerator UseWindCannon(Unit target)
    {
        // Use the ability's to disable the opponent by blowing them into the air
        target.lingeringDamageTurns = windCannon.lingeringDamageTurnsLeft;
        yield return new WaitForSeconds(2.0f);
    }

}