using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGameManager : MonoBehaviour
{
    public GameObject leftPlayer;
    public GameObject rightPlayer;
    public GameObject player3;
    public GameObject player4;
    private PlayerPVP leftPlayerScript;
    private PlayerPVP rightPlayerScript;
    private PlayerPVP player3Script;
    private PlayerPVP player4Script;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Setting each script to their roles(enum values) in the script
            leftPlayerScript = leftPlayer.GetComponent<PlayerPVP>();
            rightPlayerScript = rightPlayer.GetComponent<PlayerPVP>();
            player3Script = player3.GetComponent<PlayerPVP>();
            player4Script = player4.GetComponent<PlayerPVP>();

            SetPlayerScriptPosition();
        }
    }

    private void SetPlayerScriptPosition()
    {
        //Set player position in world and code
        leftPlayerScript.playerIDCurrentSet = EntityBase.playerID.playerLeft;
        rightPlayerScript.playerIDCurrentSet = EntityBase.playerID.playerRight;
        player3Script.playerIDCurrentSet = EntityBase.playerID.none;
        player4Script.playerIDCurrentSet = EntityBase.playerID.none;
        leftPlayerScript.InitialisePlayerNBots();
        rightPlayerScript.InitialisePlayerNBots();
        player3Script.InitialisePlayerNBots();
        player4Script.InitialisePlayerNBots();
        Debug.Log("Finished setting the players");
    }
}
