using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    private int mySaveVer = 1000;

    public string gameVer;
    public int saveVer;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("[SaveSystem] SaveSystem Online");
        if (SaveManager.fileExists("save.dat"))
        {
            PlayerData player = SaveManager.loadData("save.dat");

            gameVer = player.gameVer;
            saveVer = player.saveVer;

            if (player.saveVer > mySaveVer)
            {
                SceneManager.LoadScene("Error");
            }
        }
        else
        {
            SaveManager.SaveData(this, "save.dat");
        }
    }

    [ContextMenu("Save")]
    public void save()
    {
        if (SaveManager.fileExists("save.dat"))
        {
            Debug.Log($"[SaveSystem] Saving the save file");

            saveVer = mySaveVer;
            gameVer = Application.version;

            SaveManager.SaveData(this, "save.dat");
        }
    }

    void OnApplicationQuit()
    {
        save();
    }
}
