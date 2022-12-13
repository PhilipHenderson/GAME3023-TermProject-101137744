using UnityEngine;

[ExecuteInEditMode]
public class PlayerEditor : MonoBehaviour
{
    public string playerName;
    public int playerExperience;
    public int playerSpeed;


    public string Name
    {
        get { return playerName; }
    }

    public int Experience
    {
        get {return playerExperience / 750;}
    }

    public int Speed
    {
        get { return playerSpeed; }
    }
}
