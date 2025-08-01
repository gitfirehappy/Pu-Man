using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InitialLoad : MonoBehaviour
{
    public AssetReference MainScene;

    private void Awake()
    {
        Addressables.LoadSceneAsync(MainScene);
    }
}
