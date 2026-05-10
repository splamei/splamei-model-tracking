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

using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using SFB;

public class AviModelImporterSpawner : MonoBehaviour
{
    private GameObject currentInstance;
    public GameObject avatarRoot;
    public ModelPointMapper modelPointMapper;
    public RuntimeAnimatorController animatorController;

    private bool isLoading = false;

    // Start is called before the first frame update
    void Start()
    {
        triggerModelSwap(true);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}

    public void triggerModelSwap(bool assignAnimators)
    {
        if (!isLoading)
        {
            StartCoroutine(loadModelAndSwap(assignAnimators));
        }
    }

    IEnumerator loadModelAndSwap(bool assignAnimators)
    {
        var extensions = new [] {
            new ExtensionFilter("Splamei Model Tracking Avatar File", "splameimodeltrackavi")
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        if (paths.Length != 1)
        {
            yield break;
        }

        isLoading = true;

        if (currentInstance != null)
        {
            Destroy(currentInstance);
            yield return Resources.UnloadUnusedAssets();
        }

        // --- Extract file + manifest ---

        string tempDir = "";
        var manifest = new ModelBuildManifest();

        try
        {
            tempDir = Path.Combine(Application.temporaryCachePath, "extractedAvatar");
            string filePath = Path.Combine(Application.streamingAssetsPath, paths[0]);

            if (Directory.Exists(tempDir)) { Directory.Delete(tempDir, true); }
            Directory.CreateDirectory(tempDir);

            ZipFile.ExtractToDirectory(filePath, tempDir);

            string manifestPath = Path.Combine(tempDir, "manifest.json");
            string json = File.ReadAllText(manifestPath);
            manifest = JsonUtility.FromJson<ModelBuildManifest>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[AviModelImporterSpawner] Unable to init load the model! - {e}");
            isLoading = false;
            yield break;
        }

        // --- Load bundle ---

        if (manifest.version != "1.0" || manifest.type != "Humanoid")
        {
            Debug.LogError($"[AviModelImporterSpawner] Unable to load model as it's verion is '{manifest.version}' and type is '{manifest.type}'!");
            isLoading = false;
            yield break;
        }

        string bundlePath = Path.Combine(tempDir, manifest.bundleName);

        if (!File.Exists(bundlePath))
        {
            Debug.LogError($"[AviModelImporterSpawner] Unable to load model as the bundle file doesn't exist! Path: {bundlePath}");
            isLoading = false;
            yield break;
        }

        AssetBundle.UnloadAllAssetBundles(false);

        AssetBundleCreateRequest bundleCreateRequest = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return bundleCreateRequest;

        AssetBundle assetBundle = bundleCreateRequest.assetBundle;

        if (assetBundle == null)
        {
            Debug.LogError("[AviModelImporterSpawner] Failed to load the AssetBundle! Path: " + bundlePath);
            isLoading = false;
            yield break;
        }

        AssetBundleRequest assetRequest = assetBundle.LoadAssetAsync<GameObject>(manifest.prefabName);
        yield return assetRequest;

        GameObject prefab = assetRequest.asset as GameObject;

        if (prefab != null)
        {
            currentInstance = Instantiate(prefab);
            currentInstance.transform.SetParent(avatarRoot.transform, false);
            currentInstance.transform.localPosition = Vector3.zero;

            var animator = currentInstance.GetComponent<Animator>();

            if (assignAnimators)
            {
                if (animator != null)
                {
                    animator.runtimeAnimatorController = animatorController;
                }

                var obj = currentInstance.AddComponent<ModelAvatarDriver>();
                obj.modelPointMapper = modelPointMapper;
            }

            modelPointMapper.modelRoot = currentInstance;
            modelPointMapper.modelAni = animator;

            Debug.Log("[AviModelImporterSpawner] Swapped to the new model!");
        }
        else
        {
            Debug.LogError("[AviModelImporterSpawner] Unable to load model as the bundle GO is null!");
            isLoading = false;
            yield break;
        }

        assetBundle.Unload(false);

        isLoading = false;
    }

    void OnDisable()
    {
        AssetBundle.UnloadAllAssetBundles(false);
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