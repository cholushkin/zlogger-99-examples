using UnityEngine;

public class VisualDebugExamples : MonoBehaviour
{
    private void OnEnable()
    {
        var randomVector3 = Random.insideUnitSphere;
        Debug.Log($"Debug example of some random force: {randomVector3}");
        Debug.DrawRay(transform.position, transform.position+randomVector3, Color.red, 1f);
        Debug.Break();
        
        DrawPiramid(); // Draw only for one frame
    }

    private void DrawPiramid()
    {
        Vector3 p0 = transform.position + new Vector3(-1, 0, -1);
        Vector3 p1 = transform.position + new Vector3(1, 0, -1);
        Vector3 p2 = transform.position + new Vector3(1, 0, 1);
        Vector3 p3 = transform.position + new Vector3(-1, 0, 1);
        Vector3 top = transform.position + new Vector3(0, 2, 0);

        // Base
        Debug.DrawLine(p0, p1, Color.cyan);
        Debug.DrawLine(p1, p2, Color.cyan);
        Debug.DrawLine(p2, p3, Color.cyan);
        Debug.DrawLine(p3, p0, Color.cyan);

        // Edges
        Debug.DrawLine(p0, top, Color.magenta);
        Debug.DrawLine(p1, top, Color.magenta);
        Debug.DrawLine(p2, top, Color.magenta);
        Debug.DrawLine(p3, top, Color.magenta);
    }
}
