using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystems : MonoBehaviour
{
    [SerializeField]
    private Unit playerUnit;
    [SerializeField]
    private Unit enemyUnit;

    [Header("Encounter Units")]
    public GameObject playerPrefab;
    public GameObject[] enemyPrefabs;
    private GameObject currentEnemy;

    [Header("Platforms")]
    [SerializeField]
    private Transform playerPlatform;
    [SerializeField]
    private Transform enemyPlatform;

    [Header("Huds")]
    [SerializeField]
    private BattlePlatform playerHud;
    [SerializeField]
    private BattlePlatform enemyHud;

    [Header("Player Icons")]
    [SerializeField]
    public GameObject playerFireIcon;
    [SerializeField]
    public GameObject playerIceIcon;
    [SerializeField]
    public GameObject playerWindIcon;

    [Header("Enemy Icons")]
    [SerializeField]
    public GameObject enemyFireIcon;
    [SerializeField]
    public GameObject enemyIceIcon;
    [SerializeField]
    private GameObject enemyWindIcon;

    public bool inBattle;

    [SerializeField]
    private BattleState states;

    [Header("Encounter Window")]
    [SerializeField]
    private GameObject encounterwindow;
    [SerializeField]
    private TMP_Text encounterTxt;

    // Start is called before the first frame update
    public void Start()
    {
        inBattle = false;
    }
    public void Update()
    {
        if (encounterwindow.activeSelf == true)
        {
            if (inBattle == false)
            {
                states = BattleState.START; 
                StartCoroutine(BattleSetup());
            }
        }
    }

    IEnumerator BattleSetup()
    {
        inBattle = true;
        GameObject player = Instantiate(playerPrefab, playerPlatform);
        playerUnit = player.GetComponent<Unit>();

        var rndIndex = Random.Range(0, enemyPrefabs.Length -1);
        currentEnemy = enemyPrefabs[rndIndex];

        GameObject enemy = Instantiate(currentEnemy, enemyPlatform);
        enemyUnit = enemy.GetComponent<Unit>();

        // Create More of these and randomize it
        encounterTxt.text = "A Sneaky " + enemyUnit.Unitname + " Has Apreared!";

        playerHud.HudSet(playerUnit);
        enemyHud.HudSet(enemyUnit);

        yield return new WaitForSeconds(2.0f);

        states = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    // Player/Enemy Menu Coroutines
    public IEnumerator PlayerAttack()
    {
        playerUnit.basicAttack.UseBasicAttack(enemyUnit);
        encounterTxt.text = "The Attack Was Successfull!";

        yield return new WaitForSeconds(2.0f);
    }

    IEnumerator PlayerFireBlast()
    {
        playerUnit.fireBlast.UseFireBlast(enemyUnit);
        encounterTxt.text = "You Used Fire Blast Successfully!";
        enemyFireIcon.SetActive(true);

        if (enemyUnit.currentHp <= 0)
        {
            states = BattleState.WON;
            StartCoroutine(EndBattle());
        }

        yield return new WaitForSeconds(2.0f);
        states = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    IEnumerator EnemyFireBlast()
    {
        enemyUnit.fireBlast.UseFireBlast(playerUnit);
        encounterTxt.text = enemyUnit.name + " Used Fire Blast Successfully!";

        yield return new WaitForSeconds(2.0f);
        states = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerWindCannon()
    {
        playerUnit.windCannon.UseWindCannon(enemyUnit);
        encounterTxt.text = "You Used Wind Cannon Successfully!";

        yield return new WaitForSeconds(2.0f);
        states = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    IEnumerator EnemyWindCannon()
    {
        enemyUnit.windCannon.UseWindCannon(playerUnit);
        encounterTxt.text = enemyUnit.name + " Used Wind Cannon Successfully!";

        yield return new WaitForSeconds(2.0f);
        states = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerIcePistol()
    {
        playerUnit.icePistol.UseIcePistol(enemyUnit);
        encounterTxt.text = "You Used Wind Cannon Successfully!";

        yield return new WaitForSeconds(2.0f);
        states = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    IEnumerator EnemyIcePistol()
    {
        enemyUnit.icePistol.UseIcePistol(playerUnit);
        encounterTxt.text = enemyUnit.name + " Used Ice Pistol Successfully!";

        yield return new WaitForSeconds(2.0f);
        states = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    // Enemy Behavior Coroutines
    public IEnumerator EnemyTurn()
    {
        ////////// 1. Pre Round - Enemy Status Check //////////

        // No damage, but cant attack for one turn
        if (enemyUnit.isBlownAway) 
        {
            encounterTxt.text = enemyUnit.Unitname + " Has Being Blown Into The Air, They Cannot Attack";
            yield return new WaitForSeconds(2.0f);
            enemyUnit.isBlownAway = false;
            // 3. Changeing to players turn
            states = BattleState.PLAYERTURN;
            PlayerTurn();
            yield break;
        }

        // Where the enemy takes damage from being on fire
        if (enemyUnit.isOnFire) 
        {
            enemyUnit.currentHp -= playerUnit.fireBlast.lingeringDamage;
            encounterTxt.text = enemyUnit.Unitname + " Has Lost: " + playerUnit.fireBlast.lingeringDamage + " Damage";
            yield return new WaitForSeconds(2.0f);
        }

        // Where the enemy takes damage from being frozen
        if (enemyUnit.isFrozen)
        {
            enemyUnit.currentHp -= playerUnit.icePistol.lingeringDamage;
            encounterTxt.text = enemyUnit.Unitname + " Has Lost: " + playerUnit.icePistol.lingeringDamage + " Damage";
            yield return new WaitForSeconds(2.0f);
        }

        // If More Abilities Are Added, Make sure to add Pre Turn Damage "if's" Here

        // If no Ailments 
        if(!enemyUnit.isFrozen && !enemyUnit.isOnFire && !enemyUnit.isBlownAway)
        {
            encounterTxt.text = "No Ailments";
            yield return new WaitForSeconds(2.0f);
        }
        ////////// 2. Main Round - Enemy Logic //////////

        encounterTxt.text = "Enemy's Turn";
        yield return new WaitForSeconds(2.0f);


        //CHECKLIST:
        // a.Flee
        // b.Heal
        // c.Abilities
        // d.Attack/Defend

        // a.Flee - Enemy will attempt to flee if its HP is under 5 and MP under the cost of Healing
        if (enemyUnit.currentHp <= 5 && enemyUnit.currentMp < enemyUnit.flee.abilityCost)
        {
            //Enemy Attempts to flee
            states = BattleState.WON;
            encounterTxt.text = "The Enemy Has Defeated You";
            yield return new WaitForSeconds(2.0f);
            EndBattle();
        }

        // b.Heal - Enemy will attempt to Heal if its HP is under 5
        if (enemyUnit.currentHp <= 5)
        {
            //-Heal-
            encounterTxt.text = "The Enemy Used Healed and regained 25 Hp";
            yield return new WaitForSeconds(1.0f);
            enemyUnit.Heal(25);
            enemyHud.HpSet(enemyUnit.currentHp);
            yield return new WaitForSeconds(2.0f);
        }

        // c.Abilities

        // Randomize the Ability the Enemy Uses
        var num = Random.Range(0, 3);
        if (num != 0)
        {
            EnemyAbilities();
        }

        // d.Attack or Defend

        // If the enemy has no MP, it will decide whether to attack or defend
        else
        {
            StartCoroutine(EnemyAttackDefend());
        }
        PlayerTurn();
        states = BattleState.PLAYERTURN;
    }

    public IEnumerator EnemyAttackDefend()
    {
        var num3 = Random.Range(0, 9);
        //-Attack-
        if (num3 <= 7)
        {
            encounterTxt.text = enemyUnit.Unitname + " Attacks";
            bool isDead = playerUnit.TakeDamage(enemyUnit.dmg);
            playerHud.HpSet(playerUnit.currentHp);
        }

        //-Defend-
        else
        {
            enemyUnit.isDefending = true;
            encounterTxt.text = "Enemy Gets Into A Defencive Position!";
        }
        yield return new WaitForSeconds(2.0f);
    }

    public void EnemyAbilities()
    {
        var num2 = Random.Range(0, 5);

        if (num2 >= 4)
        {
            //Fire Blast
            if (enemyUnit.currentMp > enemyUnit.fireBlast.abilityCost)
            {
                playerFireIcon.SetActive(true);
                StartCoroutine(EnemyFireBlast());
            }
        }
        if (num2 >= 2 && num2 < 4)
        {
            //Wind Cannon
            if (enemyUnit.currentMp > enemyUnit.windCannon.abilityCost)
            {
                playerWindIcon.SetActive(true);
                StartCoroutine(EnemyWindCannon());
            }
        }
        if (num2 <= 1)
        {
            //Ice Pistol
            if (enemyUnit.currentMp > enemyUnit.icePistol.abilityCost)
            {
                playerIceIcon.SetActive(true);
                StartCoroutine(EnemyIcePistol());
            }
        }
    }

    public void PlayerTurn()
    {
        if (playerUnit.currentHp <= 0)
        {
            states = BattleState.LOST;
            EndBattle();
        }

        if (playerFireIcon.activeSelf)
        {
            playerUnit.currentHp -= enemyUnit.fireBlast.lingeringDamage;
            encounterTxt.text = "You Took Burning Damage";
            playerUnit.fireBlast.lingeringDamageTurnsLeft--;
            if (playerUnit.fireBlast.lingeringDamageTurnsLeft == 0)
            {
                playerFireIcon.SetActive(false);
            }
        }
        encounterTxt.text = "Please Choose An Action:";
    }

    public IEnumerator EndBattle()
    {
        if (states == BattleState.WON)
        {
            encounterTxt.text = "You Have Defeated The Enemy";
        }
        if (states == BattleState.LOST)
        {
            encounterTxt.text = "You Have Been Defeated";
        }
        yield return new WaitForSeconds(2.0f);
        Reset();
        encounterwindow.SetActive(false);
        StartCoroutine(BattleSetup());
    }


    //Resets Battle
    public void Reset()
    {
        //player reset
        playerUnit.currentHp = playerUnit.maxHp;
        playerUnit.currentMp = playerUnit.maxMp;

        playerUnit.isBlownAway = false;
        playerWindIcon.SetActive(false);

        playerUnit.isDefending = false;

        playerUnit.isFrozen = false;
        playerIceIcon.SetActive(false);

        playerUnit.isOnFire = false;
        playerFireIcon.SetActive(false);

        //enemy resets
        enemyUnit.currentHp = enemyUnit.maxHp;
        enemyUnit.currentHp = enemyUnit.maxMp;

        enemyUnit.isBlownAway = false;
        enemyWindIcon.SetActive(false);

        enemyUnit.isDefending = false;

        enemyUnit.isFrozen = false;
        enemyIceIcon.SetActive(false);

        enemyUnit.isOnFire = false;
        enemyFireIcon.SetActive(false);
    }


    // --Buttons--

    // Action Buttons
    public void OnAttackButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }
    public void OnHealButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        // Player Heals
    }
    public void OnDefendButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        // Player Defenends
    }

    // Ability Buttons
    public void OnFireBlastButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;
        enemyFireIcon.SetActive(true);
        StartCoroutine(PlayerFireBlast());
    }
    public void OnWindCannonButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;
        enemyWindIcon.SetActive(true);
        StartCoroutine(PlayerWindCannon());
    }
    public void OnIcePistolButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;
        enemyIceIcon.SetActive(true);
        StartCoroutine(PlayerIcePistol());
    }

}