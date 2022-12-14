using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Traveller : MonoBehaviour
{
    public UnityEvent<Scene> onTrasportToNewScene;

    [SerializeField]
    PlayerCharacterMovement playerCharacterMovement;

    private string lastSpawn = "";

    private void Awake()
    {
        playerCharacterMovement = GetComponent<PlayerCharacterMovement>();
    }

    private void Start()
    {
#if UNITY_EDITOR
        EditorKillClones();
#endif
        DontDestroyOnLoad(this); // this tells unitl that this game Ongect sholulf not be cleaned up with all

        SceneManager.sceneLoaded += OnSceneLoadedAction;
    }

    public void SetSpawn(string spawn)
    {
        lastSpawn = spawn;
    }

    void OnSceneLoadedAction(Scene scene, LoadSceneMode loadmode)
    {
        if (lastSpawn != "")
        {
            //Go through all the spawn locations to find the one given
            bool transportSuccessful = false;

            PortalSpawn[] spawnPoints = FindObjectsOfType<PortalSpawn>(); // find all possible spawn locations
            foreach (PortalSpawn spawn in spawnPoints)
            {
                if (spawn.name == lastSpawn)
                {
                    //go to that spawn
                    transform.position = spawn.transform.position;
                    transportSuccessful = true;
                }
                if (spawn.name == "DW-Portal End")
                {
                    playerCharacterMovement.isRaining = true;
                }
                if (spawn.name == "Inn-Portal End")
                {
                    playerCharacterMovement.isRaining = false;
                }
            }

            if (!transportSuccessful)
            {
                throw new System.Exception("Could not find Spawn Point: " + lastSpawn);
            }
        }

    }

    private void EditorKillClones()
    {
        if (PortalSpawn.Player != GetComponent<PlayerCharacterMovement>())
        {
            //if we are not the original, we must die!
            Destroy(gameObject);
        }
    }
}
