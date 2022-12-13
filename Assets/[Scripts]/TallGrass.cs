using UnityEngine;

public class TallGrass : MonoBehaviour
{
    public int percentageChance;
    public bool hasEnteredEncounter;
    public GameObject encounter;

    private int chanceOfEncounter;

    public void Update()
    {
        if (hasEnteredEncounter)
        {
            //Start Encounter
            encounter.SetActive(true);
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            chanceOfEncounter = Random.Range(0, 100);
            if (chanceOfEncounter < percentageChance)
            {
                hasEnteredEncounter = true;
                Debug.Log("hasEnteredEncounter: " + hasEnteredEncounter);
            }
        }
    }
}
