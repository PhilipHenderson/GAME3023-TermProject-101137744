using UnityEngine;

public class TallGrass : MonoBehaviour
{
    [Header("Tall Grass Properties")]
    public int percentageChance;
    public bool hasEnteredEncounter;

    [Header("Game System Objects")]
    public GameObject encounterWindow;
    public GameObject battleSystemsGameObject;

    BattleSystems battleSystems;

    private int chanceOfEncounter;
    private bool battleSystemsBool;

    private void Awake()
    {
        battleSystems = battleSystemsGameObject.GetComponent<BattleSystems>();
    }

    private void Start()
    {
        battleSystemsBool = battleSystems.inBattle;
        encounterWindow.SetActive(false);
    }

    public void Update()
    {
        if (battleSystemsBool)
        {
            Debug.Log("battleSystemsBool =" + battleSystemsBool);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            chanceOfEncounter = Random.Range(0, 100);
            if (chanceOfEncounter < percentageChance)
            {
                encounterWindow.SetActive(true);
                Debug.Log("hasEnteredEncounter: " + hasEnteredEncounter);
            }
        }
    }
}
