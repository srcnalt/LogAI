using UnityEngine;

[ExecuteInEditMode]
public class UnitDebug : MonoBehaviour
{
    public DrawMode drawMode = DrawMode.on;
    public Transform player;

    [Space(20)]
    [Range(1, 5)]
    public float unitSize;
    public Color drawColor;
    public Color activeCubeColor;

    [Space(20)]
    public Vector3 boundingBoxSize;
    public Vector3 boundingBoxPivot;
    public Color boundingBoxColor;

    public enum DrawMode { on, off, bounding_box_only, unit_cubes_only }

    private Vector3 currentActiveCube = new Vector3(0, 0, 0);
    private Vector3 previousActiveCube = new Vector3(0, 0, 0);

    //DELET
    private int counter = 0;

    void Start()
    {
        InvokeRepeating("Logger", 0, 1);
    }

    void OnDrawGizmos()
    {
        if (drawMode == DrawMode.off) return;

        if(drawMode == DrawMode.bounding_box_only)
        {
            DrawBoundingBox();
        }
        else if (drawMode == DrawMode.unit_cubes_only)
        {
            DrawUnitCubes();
        }
        else
        {
            DrawBoundingBox();
            DrawUnitCubes();
        }
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

                    bool isActiveCube = IsPlayerInTheBox(new Vector3(x, y, z));

                    if (isActiveCube)
                    {
                        currentActiveCube = new Vector3(x, y, z);

                        Gizmos.color = activeCubeColor;
                        Gizmos.DrawCube(cubeCenter, new Vector3(unitSize, unitSize, unitSize));
                    }
                    else
                    {
                        Gizmos.color = drawColor;
                        Gizmos.DrawWireCube(cubeCenter, new Vector3(unitSize, unitSize, unitSize));
                    }
                }
            }
        }

        if(currentActiveCube != previousActiveCube)
        {
            Logger("u");

            previousActiveCube = currentActiveCube;
        }
    }

    bool IsPlayerInTheBox(Vector3 cubeIndex)
    {
        Vector3 pos = player.position;

        if(((pos.x > boundingBoxPivot.x + cubeIndex.x) && (pos.x < boundingBoxPivot.x + cubeIndex.x + unitSize)) &&
        ((pos.y > boundingBoxPivot.y + cubeIndex.y) && (pos.y < boundingBoxPivot.y + cubeIndex.y + unitSize)) &&
        ((pos.z > boundingBoxPivot.z + cubeIndex.z) && (pos.z < boundingBoxPivot.z + cubeIndex.z + unitSize)))
        {
            return true;
        }

        return false;
    }

    void Logger(string name)
    {
        Debug.Log("Logged by " + name + " " + counter++);
    }

    void Logger()
    {
        Debug.Log("Logged by t " + counter++);
    }
}
