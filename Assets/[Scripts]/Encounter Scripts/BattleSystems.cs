using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

/* Usefull Scripts
 * 
 * Sets the Slider InGame - must be done after ever action that effects health
 * "playerHud.HpSet(playerUnit.currentHp);"
 * 
 * 
 */

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, FINISH }

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
    public BattleState states;

    [Header("Encounter Window")]
    [SerializeField]
    public GameObject encounterwindow;
    [SerializeField]
    public TMP_Text encounterTxt;

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

    // BattleSetup Coroutine
    IEnumerator BattleSetup()
    {
        inBattle = true;
        GameObject player = Instantiate(playerPrefab, playerPlatform);
        playerUnit = player.GetComponent<Unit>();

        var rndIndex = Random.Range(0, enemyPrefabs.Length - 1);
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
    IEnumerator EnemyFireBlast()
    {

        enemyUnit.UseFireBlast(playerUnit);
        encounterTxt.text = enemyUnit.name + " Used Fire Blast!";

        yield return new WaitForSeconds(2.0f);
        states = BattleState.PLAYERTURN;
        PlayerTurn();
    }
    IEnumerator EnemyIcePistol()
    {
        enemyUnit.UseIcePistol(playerUnit);
        encounterTxt.text = enemyUnit.name + " Used Ice Pistol!";

        yield return new WaitForSeconds(2.0f);
        states = BattleState.PLAYERTURN;
        PlayerTurn();
    }
    IEnumerator EnemyWindCannon()
    {
        enemyUnit.UseWindCannon(playerUnit);
        encounterTxt.text = enemyUnit.name + " Used Wind Cannon!";

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
            // 3. Changing to players turn
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
        if (!enemyUnit.isFrozen && !enemyUnit.isOnFire && !enemyUnit.isBlownAway)
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
        if (enemyUnit.currentHp <= 5 && enemyUnit.currentMp < enemyUnit.flea.abilityCost)
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
            enemyUnit.Heal();
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
            enemyUnit.Attack(playerUnit, enemyUnit.basicAttackDmg);
            encounterTxt.text = enemyUnit.Unitname + " Attacks";
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
        // Check for Ailments


        // Waiting for Player to Select An Action
        encounterTxt.text = "Please Choose An Action:";
    }

    public void EndBattle()
    {
        Reset();
        encounterwindow.SetActive(false);
    }
    // -- Player Buttons and Coroutines --

    // Action Buttons
    public void OnAttackButton()
    {
        playerUnit.Attack(enemyUnit, playerUnit.basicAttackDmg);
        encounterTxt.text = playerUnit.Unitname + " Used FireBlast";

        ChangeGameState();
    }
    public void OnHealButton()
    {
        playerUnit.Heal();

        ChangeGameState();
    }
    public void OnDefendButton()
    {
        //TODO: insert defending function from the unit script here
        encounterTxt.text = playerUnit.Unitname + " Used Defend, Reduced Incoming Dmg";

        ChangeGameState();
    }

    // Ability Buttons
    public void OnFireBlastButton()
    {
        //TODO: insert "Chance Of Hitting Target"
        enemyFireIcon.SetActive(true);
        playerUnit.UseFireBlast(enemyUnit);
        encounterTxt.text = playerUnit.Unitname + " Used FireBlast";

        ChangeGameState();

    }
    public void OnIcePistolButton()
    {
        //TODO: insert "Chance Of Hitting Target"
        enemyIceIcon.SetActive(true);
        playerUnit.UseIcePistol(enemyUnit);
        encounterTxt.text = playerUnit.Unitname + " Used IcePistol";

        ChangeGameState();
    }
    public void OnWindCannonButton()
    {
        //TODO: insert "Chance Of Hitting Target"
        enemyWindIcon.SetActive(true);
        playerUnit.UseWindCannon(enemyUnit);
        encounterTxt.text = playerUnit.Unitname + " Used WindCannon";

        StartCoroutine(ChangeGameState());
    }



    // BattleScript Function Library
    // Reset Battle System
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
    // Couples together the two functions CheckForEndOfGame and ChangeTurns
    public IEnumerator ChangeGameState()
    {
        yield return new WaitForSeconds(2.0f);

        CheckForEndGame();

        yield return new WaitForSeconds(2.0f);

        ChangeTurns();
    }
    public IEnumerator CheckForEndGame()
    {
        //Checks to see if enemy is still alive
        if (enemyUnit.currentHp <= 0)
        {
            encounterTxt.text = "You Have Defeated the Enemy";
            yield return new WaitForSeconds(2.0f);
            states = BattleState.FINISH;
            EndBattle();
        }
        //Checks to see if player is still alive
        if (playerUnit.currentHp <= 0)
        {
            encounterTxt.text = "You Have Been Defeated";
            yield return new WaitForSeconds(2.0f);
            states = BattleState.FINISH;
            EndBattle();
        }
        // If player and enemy are still alive, the Game Continues
    }
    public void ChangeTurns()
    {
        // Change Turns
        if (states != BattleState.ENEMYTURN)
        {
            states = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            states = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }
}
