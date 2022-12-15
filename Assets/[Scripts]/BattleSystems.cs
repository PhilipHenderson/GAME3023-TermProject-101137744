using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystems : MonoBehaviour
{
    [SerializeField]
    Unit playerUnit;
    [SerializeField]
    Unit enemyUnit;

    [Header("Encounter Units")]
    public GameObject playerPrefab;
    public GameObject[] enemyPrefabs;
    private GameObject currentEnemy;

    [Header("Icons")]
    public GameObject playerFlameIcon;
    public GameObject playerIceIcon;
    public GameObject playerWindIcon;
    public GameObject enemyFlameIcon;
    public GameObject enemyIceIcon;
    public GameObject enemyWindIcon;

    [Header("Platforms")]
    public Transform playerPlatform;
    public Transform enemyPlatform;

    [Header("Huds")]
    public BattlePlatform playerHud;
    public BattlePlatform enemyHud;

    public bool inBattle;
    [SerializeField]
    public BattleState states;

    [Header("Encounter Window")]
    public GameObject encounterwindow;
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

    // Player Menu Coroutines
    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.dmg);

        enemyHud.HpSet(enemyUnit.currentHp);
        encounterTxt.text = "The Attack Was Successfull!";

        yield return new WaitForSeconds(2.0f);

        if (isDead)
        {
            states = BattleState.WON;
            EndBattle();
        }
        else
        {
            states = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(25);

        playerHud.HpSet(playerUnit.currentHp);
        encounterTxt.text = "The Player Healed!";

        yield return new WaitForSeconds(2.0f);

        states= BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    IEnumerator PlayerDefend()
    {
        playerUnit.isDefending = true;
        encounterTxt.text = "Player Gets Into A Defencive Position!";
        yield return new WaitForSeconds(2.0f);
        states = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    IEnumerator PlayerFireBlast()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.fireBlastDmg);

        enemyUnit.isOnFire = true;
        enemyFlameIcon.SetActive(true);
        enemyUnit.burningTurnsLeft = 2;
        enemyHud.HpSet(enemyUnit.currentHp);
        encounterTxt.text = "The Fire Blast Was Successful!";

        yield return new WaitForSeconds(2.0f);
        
        if (isDead)
        {
            states = BattleState.WON;
            EndBattle();
        }
        else
        {
            states = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    IEnumerator PlayerWindCannon()
    {
        encounterTxt.text = "The Wind Cannon Was Successful!";
        enemyWindIcon.SetActive(true);
        enemyUnit.isBlownAway = true;

        yield return new WaitForSeconds(2.0f);

        states = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    IEnumerator PlayerIcePistol()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.icePistolDmg);

        enemyUnit.isFrozen = true;
        enemyIceIcon.SetActive(true);
        enemyUnit.freezingTurnsLeft = 2;
        enemyHud.HpSet(enemyUnit.currentHp);
        encounterTxt.text = "The Ice Pistol Was Successfull!";

        yield return new WaitForSeconds(2.0f);

        if (isDead)
        {
            states = BattleState.WON;
            EndBattle();
        }
        else
        {
            states = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyFireBlast()
    {

        bool isDead = playerUnit.TakeDamage(enemyUnit.fireBlastDmg);

        playerUnit.isOnFire = true;
        playerFlameIcon.SetActive(true);
        playerUnit.burningTurnsLeft = 2;
        playerHud.HpSet(playerUnit.currentHp);
        encounterTxt.text = "The Fire Blast Was Successful!";

        yield return new WaitForSeconds(2.0f);

        if (isDead)
        {
            states = BattleState.LOST;
            EndBattle();
        }
    }
    IEnumerator EnemyWindCannon()
    {
        encounterTxt.text = "The Enemy Used Wind Cannon Successfully!";
        playerWindIcon.SetActive(true);
        playerUnit.isBlownAway = true;

        yield return new WaitForSeconds(2.0f);
    }
    IEnumerator EnemyIcePistol()
    {
        bool isDead = playerUnit.TakeDamage(enemyUnit.icePistolDmg);

        playerUnit.isFrozen = true;
        playerIceIcon.SetActive(true);
        playerUnit.freezingTurnsLeft = 2;
        playerHud.HpSet(playerUnit.currentHp);
        encounterTxt.text = "The Enemy Used Ice Pistol Successfully!";

        yield return new WaitForSeconds(2.0f);

        if (isDead)
        {
            states = BattleState.LOST;
            EndBattle();
        }
    }

    // Enemy Behavior Coroutines
    IEnumerator EnemyTurn()
    {
        // 1. Pre Round - Enemy Status Check
        if (enemyUnit.isBlownAway) // no damage, but cant attack for one turn
        {
            encounterTxt.text = enemyUnit.Unitname + " Has Being Blown Over, They Cannot Attack";
            enemyUnit.isBlownAway = false;
            enemyWindIcon.SetActive(false);
            // 3. Changeing to players turn
            states = BattleState.PLAYERTURN;
            PlayerTurn();
            yield break;
        }
        if (enemyUnit.isOnFire) // where the enemy takes damage from being on fire
        {
            encounterTxt.text = enemyUnit.Unitname + " is being burnt by fire";
            enemyUnit.TakeDamage(playerUnit.burningDmg);
            if (enemyUnit.burningTurnsLeft > 0)
            {
                enemyUnit.burningTurnsLeft--;
                if (enemyUnit.burningTurnsLeft == 0)
                {
                    enemyUnit.isOnFire = false;
                    enemyFlameIcon.SetActive(false);
                }
            }
        }
        if (enemyUnit.isFrozen) // where the enemy takes damage from being frozen
        {
            encounterTxt.text = enemyUnit.Unitname + " is being Frozen By Ice";
            enemyUnit.TakeDamage(playerUnit.freezingDmg);

            yield return new WaitForSeconds(2.0f);

            if (enemyUnit.freezingTurnsLeft > 0)
            {
                enemyUnit.freezingTurnsLeft--;
                if (enemyUnit.freezingTurnsLeft == 0)
                {
                    enemyUnit.isFrozen = false;
                    enemyIceIcon.SetActive(false);
                }
            }
        }
        else
            encounterTxt.text = "No Alements";

        // 2. Main Round - Enemy Logic

        encounterTxt.text = "Enemy's Turn";

        yield return new WaitForSeconds(2.0f);

        // --AI Section--
        //CHECKLIST:

        // 1.Flee
        if (enemyUnit.currentHp <= 5 && enemyUnit.currentMp < enemyUnit.healMpCost)
        {
            //Enemy Attempts to flee
            states = BattleState.WON;
            encounterTxt.text = "The Enemy Has Defeated You";
            yield return new WaitForSeconds(2.0f);
            EndBattle();
        }

        // 2.Heal
        if (enemyUnit.currentHp <= 5)
        {
            //-Heal-
            encounterTxt.text = "The Enemy Used Healed and regained 25 Hp";
            yield return new WaitForSeconds(1.0f);
            enemyUnit.Heal(25);
            enemyHud.HpSet(enemyUnit.currentHp);
            yield return new WaitForSeconds(2.0f);
        }


        var num = Random.Range(0, 3);
        // 3. Abilities
        if (num != 0)
        {
            EnemyAbilities();
        }
        // 4. Attack or Defend
        else
        {
            EnemyAttackDefend();
        }
        PlayerTurn();
        states = BattleState.PLAYERTURN;
    }
        

    public void EnemyAttackDefend()
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
    }

    public void EnemyAbilities()
    {
        var num2 = Random.Range(0, 5);

        if (num2 >= 4)
        {
            //Fire Blast
            if (enemyUnit.currentMp > enemyUnit.fireMpCost)
            {
                encounterTxt.text = "Enemy Used Fire Blast Successfully";
                StartCoroutine(EnemyFireBlast());
            }
        }
        if (num2 >= 2 && num2 < 4)
        {
            //Wind Cannon
            if (enemyUnit.currentMp > enemyUnit.windMpCost)
            {
                encounterTxt.text = "Enemy Used Wind Cannon Successfully";
                StartCoroutine(EnemyWindCannon());
            }
        }
        if (num2 <= 1)
        {
            //Ice Pistol
            if (enemyUnit.currentMp > enemyUnit.iceMpCost)
            {
                encounterTxt.text = "Enemy Used Ice Pistol Successfully";
                StartCoroutine(EnemyIcePistol());
            }
        }
    }

    void PlayerTurn()
    {
        if (playerUnit.currentHp == 0)
        {
            EndBattle();
        }
        encounterTxt.text = "Please Choose An Action:";
    }

    void EndBattle()
    {
        if (states == BattleState.WON)
        {
            Reset();
            encounterTxt.text = "You Have Defeated The Enemy";
            encounterwindow.SetActive(false);
            StartCoroutine(BattleSetup()); 
        }
        else if (states == BattleState.LOST)
        {
            Reset();
            encounterTxt.text = "The Enemy Has Defeated You";
            encounterwindow.SetActive(false);
            StartCoroutine(BattleSetup());
        }
    }

    public void Reset()
    {
        //player reset
        playerUnit.currentHp = playerUnit.maxHp;
        playerUnit.currentMp = playerUnit.maxMp;
        playerUnit.isBlownAway = false;
        playerUnit.isDefending = false;
        playerUnit.isFrozen = false;
        playerUnit.isOnFire = false;

        //enemy resets
        enemyUnit.currentHp = enemyUnit.maxHp;
        enemyUnit.currentHp = enemyUnit.maxMp;
        enemyUnit.isBlownAway = false;
        enemyUnit.isDefending = false;
        enemyUnit.isFrozen = false;
        enemyUnit.isOnFire = false;
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

        StartCoroutine(PlayerHeal());
    }
    public void OnDefendButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerDefend());
    }

    // Ability Buttons
    public void OnFireBlastButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerFireBlast());
    }
    public void OnWindCannonButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerWindCannon());
    }
    public void OnIcePistolButton()
    {
        if (states != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerIcePistol());
    }

}
