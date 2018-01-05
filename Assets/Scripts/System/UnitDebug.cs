using UnityEngine;

[ExecuteInEditMode]
public class UnitDebug : MonoBehaviour
{
    [Range(1, 5)]
    public float unitSize;
    public Color drawColor;

    [Space]
    public Vector3 boundingBoxSize;
    public Vector3 boundingBoxPivot;
    public Color boundingBoxColor;

    void OnDrawGizmos()
    {
        DrawUnitCubes();
        DrawBoundingBox();
    }

    void DrawBoundingBox()
    {
        Gizmos.color = new Color(1, 0, 0);
        Gizmos.DrawSphere(boundingBoxPivot, 0.1f);

        Vector3 boundingBoxCenter = new Vector3(boundingBoxPivot.x + boundingBoxSize.x / 2, boundingBoxPivot.y + boundingBoxSize.y / 2, boundingBoxPivot.z + boundingBoxSize.z / 2);

        Gizmos.color = boundingBoxColor;
        Gizmos.DrawWireCube(boundingBoxCenter, boundingBoxSize);
    }

    void DrawUnitCubes()
    {
        for (float x = 0; x < boundingBoxSize.x; x += unitSize)
        {
            for (float y = 0; y < boundingBoxSize.y; y += unitSize)
            {
                for (float z = 0; z < boundingBoxSize.z; z += unitSize)
                {
                    Vector3 cubeCenter = new Vector3(boundingBoxPivot.x + unitSize / 2 + x, 
                                                     boundingBoxPivot.y + unitSize / 2 + y,
                                                     boundingBoxPivot.z + unitSize / 2 + z);

                    Gizmos.color = drawColor;
                    Gizmos.DrawWireCube(cubeCenter, new Vector3(unitSize, unitSize, unitSize));
                }
            }
        }
    }
}
