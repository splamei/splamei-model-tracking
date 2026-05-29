/*  Copyright 2026 Splamei
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

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
