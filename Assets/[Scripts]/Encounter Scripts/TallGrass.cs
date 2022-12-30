using UnityEngine;

public class TallGrass : MonoBehaviour
{
    [Header("Tall Grass Properties")]
    [SerializeField]
    private int percentageChance;

    [Header("Game System Objects")]
    [SerializeField]
    private GameObject encounterWindow;
    [SerializeField]
    private GameObject battleSystemsGameObject;
    [SerializeField]
    private BattleSystems battleSystems;

    private int chanceOfEncounter;
    private bool hasEnteredEncounter;
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
