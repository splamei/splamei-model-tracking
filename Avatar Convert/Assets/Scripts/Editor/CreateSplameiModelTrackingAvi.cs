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
using UnityEditor;
using System.IO;
using System.IO.Compression;
using System;

public class CreateSplameiModelTrackingAvi : EditorWindow
{
    private GameObject selectedPrefab;
    private string validationMessage = "";
    private bool isValidModel = false;

    [MenuItem("Splamei Model Tracking/Build Avatar")]
    public static void showUiWindow()
    {
        var window = GetWindow<CreateSplameiModelTrackingAvi>("Splamei Model Tracking Builder");
        Vector2 fixedSize = new Vector2(460, 700);
        window.minSize = fixedSize;
        window.maxSize = fixedSize;
    }

    private void OnGUI()
    {
        int ogSize = GUI.skin.label.fontSize;
        GUI.skin.label.fontSize = 30;

        Texture2D image;
        image = Resources.Load("Splamei/Model Tracking/Banner") as Texture2D;
        GUILayout.Label(image, GUILayout.MaxWidth(450f), GUILayout.MaxHeight(200f));

        GUILayout.Label("Splamei Model Tracking Builder", GUILayout.Width(500), GUILayout.Height(75));
        GUI.skin.label.fontSize = ogSize;
        GUILayout.Label("Avatar builder version 1.0.0.0");

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selectedPrefab, typeof(GameObject), false);

        if (EditorGUI.EndChangeCheck())
        {
            checkPrefab();
        }

        EditorGUILayout.Space();

        if (!string.IsNullOrEmpty(validationMessage))
        {
            EditorGUILayout.HelpBox(validationMessage, isValidModel ? MessageType.Info : MessageType.Error);
        }

        EditorGUILayout.Space();

        GUI.enabled = isValidModel;

        if (GUILayout.Button("Build the model", GUILayout.Height(40)))
        {
            buildModel();
        }

        GUI.enabled = true;

        GUI.enabled = false;
        if (GUILayout.Button("Documentation (Work in progress)"))
        {
            Application.OpenURL("https://docs.veemo.uk");
        }

        GUI.enabled = true;
        if (GUILayout.Button("GitHub"))
        {
            Application.OpenURL("https://github.com/splamei/splamei-model-tracking");
        }

        GUILayout.Label("Made with <3 by Splamei");
    }

    private void checkPrefab()
    {
        isValidModel = false;
        validationMessage = "";

        if (selectedPrefab == null)
        {
            validationMessage = "You need to add a prefab of your model from the project view";
            return;
        }

        string path = AssetDatabase.GetAssetPath(selectedPrefab);
        if (string.IsNullOrEmpty(path) || !path.EndsWith(".prefab"))
        {
            validationMessage = "You didn't select a prefab! It needs to be a prefab ending with '.prefab'";
            return;
        }

        Animator animator = selectedPrefab.GetComponent<Animator>();
        if (animator == null)
        {
            validationMessage = "The root of the model must have an animator assigned!";
            return;
        }

        if (animator.avatar == null)
        {
            validationMessage = "The animator doesn't have an avatar assigned! It must have a humanoid avatar assigned";
            return;
        }

        if (!animator.avatar.isHuman)
        {
            validationMessage = "The animator doesn't have a humanoid avatar assigned! It must be humanoid!";
            return;
        }

        isValidModel = true;
        validationMessage = "This model looks ready to go!";
    }

    private void buildModel()
    {
        try
        {
            string assetPath = AssetDatabase.GetAssetPath(selectedPrefab);

            string tempOutput = "Assets/TempBundles";
            string finalOutput = EditorUtility.OpenFolderPanel("Pick the target folder", "", "");

            if (string.IsNullOrEmpty(finalOutput))
            {
                Debug.LogWarning("Not building avatar as the path was null or empty");
                return;
            }

            if (!Directory.Exists(tempOutput))
            {
                Directory.CreateDirectory(tempOutput);
            }

            string bundleName = selectedPrefab.name.ToLower() + ".bundle";

            // --- Building the bundle itself lol ---

            AssetBundleBuild buildMap = new AssetBundleBuild
            {
                assetBundleName = bundleName,
                assetNames = new[] { assetPath }
            };

            BuildPipeline.BuildAssetBundles(
                tempOutput,
                new [] { buildMap },
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows64
            );

            // --- Create the manifest file ---

            var manifest = new ModelBuildManifest
            {
                prefabName = selectedPrefab.name,
                bundleName = bundleName,
                version = "1.0",
                type = "Humanoid"
            };

            string json = JsonUtility.ToJson(manifest, true);
            string jsonPath = Path.Combine(tempOutput, "manifest.json");
            File.WriteAllText(jsonPath, json);

            // --- Path the stuff yeah ummmmm yeah ---

            string finalFilePath = Path.Combine(finalOutput, selectedPrefab.name + ".splameimodeltrackavi");

            if (File.Exists(finalFilePath)) { File.Delete(finalFilePath); }

            ZipFile.CreateFromDirectory(tempOutput, finalFilePath);

            // --- Clean up ---

            Directory.Delete(tempOutput, true);

            AssetDatabase.Refresh();
            
            Debug.Log($"Built the model {selectedPrefab.name} into {finalFilePath}!");
            EditorUtility.RevealInFinder(finalFilePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to convert a model! - " + ex);
            EditorUtility.DisplayDialog("Failed to convert the model", "Something went wrong when converting that model. Please try again and validate the model is valid.\n\nIf you see this message again, please report the issue on GitHub. The error is in the console", "OK");
        }
    }
}

[Serializable]
public class ModelBuildManifest
{
    public string version;
    public string type;

    public string prefabName;
    public string bundleName;
}