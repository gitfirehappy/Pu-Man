using UnityEngine;

[ExecuteInEditMode]
public class MultiRangeVisualizer : MonoBehaviour
{
    [Header("范围1")]
    public float radius1 = 1f;

    public Color color1 = Color.red;
    public bool showRange1 = true;

    [Header("范围2")]
    public float radius2 = 2f;

    public Color color2 = Color.blue;
    public bool showRange2 = true;

    [Header("范围3")]
    public float radius3 = 2f;

    public Color color3 = Color.green;
    public bool showRange3 = true;

    private void OnDrawGizmos()
    {
        if (showRange1)
        {
            Gizmos.color = color1;
            Gizmos.DrawWireSphere(transform.position, radius1);
        }

        if (showRange2)
        {
            Gizmos.color = color2;
            Gizmos.DrawWireSphere(transform.position, radius2);
        }

        if (showRange3)
        {
            Gizmos.color = color3;
            Gizmos.DrawWireSphere(transform.position, radius3);
        }
    }
}