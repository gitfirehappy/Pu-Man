using UnityEngine;
using System.Runtime.InteropServices;

public class versionCheck : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"Unity版本: {Application.unityVersion}");
        Debug.Log($"运行框架: {RuntimeInformation.FrameworkDescription}");
    }
}
