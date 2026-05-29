using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveManager
{
    public static void SaveData(SaveSystem player, string pathName)
    {
        try
        {
            string path = GlobalData.getSavePath(pathName);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path).ToString());
            }

            string json = JsonUtility.ToJson(player);
            byte[] plainBytes = Encoding.UTF8.GetBytes(json);

            using (var fs = new FileStream(path, FileMode.Create))
            {
                fs.Write(plainBytes, 0, plainBytes.Length);
            }

        }
        catch (Exception ex)
        {
            PlayerPrefs.SetString("err", "Unknown error saving the save file");
            Debug.LogError("[SaveManager] Error saving the save file! - " + ex);
            SceneManager.LoadScene("Error");
        }
    }

    public static PlayerData loadData(string pathName)
    {
        try
        {
            string path = GlobalData.getSavePath(pathName);

            if (!File.Exists(path))
            {
                PlayerPrefs.SetString("err", "Attempted to load a non-existant save file");
                Debug.LogError("[SaveManager] Save file not found");
                SceneManager.LoadScene("Error");
                return null;
            }
            else
            {
                byte[] fileBytes = File.ReadAllBytes(path);

                string json = Encoding.UTF8.GetString(fileBytes);

                return JsonUtility.FromJson<PlayerData>(json);
            }
        }
        catch
        {
            PlayerPrefs.SetString("err", "Attempted to load a corrupted save file");
            Debug.LogError("[SaveManager] Corrupt Save File");
            SceneManager.LoadScene("Error");
            return null;
        }
    }

    public static bool fileExists(string pathName)
    {
        try
        {
            string path = GlobalData.getSavePath(pathName);

            return File.Exists(path);
        }
        catch
        {
            PlayerPrefs.SetString("err", "Unknown error checking if the save file exists");
            Debug.LogError("[SaveManager] Unable to see if save file exists");
            SceneManager.LoadScene("Error");
            return false;
        }
    }
}
