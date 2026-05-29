using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string gameVer;
    public int saveVer;

    public PlayerData (SaveSystem player)
    {
        gameVer = player.gameVer;
        saveVer = player.saveVer;
    }
}
