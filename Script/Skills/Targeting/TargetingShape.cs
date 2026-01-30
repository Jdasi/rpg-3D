using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public partial class TargetingShape : MonoBehaviour
{
    public bool AnyCollisions => _collisions.Count > 0;

    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] MeshRenderer _meshRenderer;

    private CharacterData _owner;
    private TargetingFlags _flags;
    private List<Token> _collisions;

    public void SetTargetingInfo(CharacterData owner, TargetingFlags flags)
    {
        _owner = owner;
        _flags = flags;
        _collisions = new List<Token>();
    }

    public void InitLine(float width, float length)
    {
        Debug.Assert(width > 0, $"[TargetingShape] InitLine - width was invalid: {width}");
        InitLine(width, width, length);
    }

    public void InitLine(float startWidth, float endWidth, float length)
    {
        if (Mathf.Approximately(startWidth, 0f))
        {
            InitTriangle(endWidth, length);
            return;
        }

        Debug.Assert(endWidth > 0, $"[TargetingShape] InitLine - endWidth was invalid: {endWidth}");
        Debug.Assert(length > 0, $"[TargetingShape] InitLine - length was invalid: {length}");
        ResetMesh(CreateLineMesh(startWidth, endWidth, length));
    }

    public void InitLineWithCap(float width, float length)
    {
        Debug.Assert(width > 0, $"[TargetingShape] InitLineWithCap - width was invalid: {width}");
        var lineMesh = CreateLineMesh(width, width, length);
        var capMesh = CreateArcMesh(width / 2, 180);

        // Prepare transformations
        var lineTransform = Matrix4x4.identity;
        var capTransform = Matrix4x4.TRS(
            new Vector3(0, 0, length), // Move cap to the end of the line
            Quaternion.identity,
            Vector3.one
        );

        // Prepare to combine
        var combineInstances = new CombineInstance[]
        {
            new CombineInstance
            {
                mesh = lineMesh,
                transform = lineTransform
            },
            new CombineInstance
            {
                mesh = capMesh,
                transform = capTransform
            },
        };

        // Combine meshes
        var combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances, true, true);
        ResetMesh(combinedMesh);

        Destroy(lineMesh);
        Destroy(capMesh);
    }

    public void InitTriangle(float width, float length)
    {
        Debug.Assert(width > 0, $"[TargetingShape] InitTriangle - width was invalid: {width}");
        Debug.Assert(length > 0, $"[TargetingShape] InitTriangle - length was invalid: {length}");
        ResetMesh(CreateTriangleMesh(width, length));
    }

    public void InitCircle(float radius)
    {
        Debug.Assert(radius > 0, $"[TargetingShape] InitCircle - radius was invalid: {radius}");
        ResetMesh(CreateCircleMesh(radius));
    }

    public void InitArc(float radius, float angle)
    {
        Debug.Assert(radius > 0f, $"[TargetingShape] InitArc - radius was invalid: {radius}");
        Debug.Assert(angle > 0f && angle <= 360f, $"[TargetingShape] InitArc - angle was invalid: {radius}");
        ResetMesh(CreateArcMesh(radius, angle));
    }

    public void IgnoreCollision(Collider other)
    {
        if (!TryGetComponent(out MeshCollider collider))
        {
            return;
        }

        Physics.IgnoreCollision(other, collider);
    }

    public List<Token> GetCollisions()
    {
        return _collisions;
    }

    private Mesh CreateLineMesh(float startWidth, float endWidth, float length)
    {
        float halfStart = startWidth * 0.5f;
        float halfEnd = endWidth * 0.5f;

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-halfStart, 0, 0),         // bottom left
            new Vector3(halfStart, 0, 0),          // bottom right
            new Vector3(-halfEnd, 0, length),      // top left
            new Vector3(halfEnd, 0, length),       // top right
        };

        int[] triangles = new int[6]
        {
            0, 2, 1, // first triangle
            1, 2, 3  // second triangle
        };

        Vector2[] uvs = _meshRenderer != null
            ? new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
            }
            : null;

        return CreateMesh(vertices, triangles, uvs);
    }

    private Mesh CreateTriangleMesh(float width, float length)
    {
        float halfEnd = width * 0.5f;

        Vector3[] vertices = new Vector3[3]
        {
            new Vector3(0, 0, 0),                   // single bottom vertex
            new Vector3(-halfEnd, 0, length),       // top left
            new Vector3(halfEnd, 0, length),        // top right
        };

        int[] triangles = new int[3] { 0, 1, 2 };

        Vector2[] uvs = _meshRenderer != null
            ? new Vector2[3]
            {
                new Vector2(0.5f, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
            }
            : null;

        return CreateMesh(vertices, triangles, uvs);
    }

    private Mesh CreateCircleMesh(float radius)
    {
        bool hasRenderer = _meshRenderer != null;

        int segments = GetSegmentCountForRadius(radius);
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];
        Vector2[] uvs = hasRenderer ? new Vector2[segments + 1] : null;

        // Center vertex at index 0
        vertices[0] = Vector3.zero;

        if (hasRenderer)
        {
            uvs[0] = new Vector2(0.5f, 0.5f);
        }

        float angleIncrement = 2 * Mathf.PI / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleIncrement;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            vertices[i + 1] = new Vector3(x, 0, z);

            if (hasRenderer)
            {
                uvs[i + 1] = new Vector2((x / (radius * 2)) + 0.5f, (z / (radius * 2)) + 0.5f);
            }

            int triIndex = i * 3;
            triangles[triIndex] = 0;
            triangles[triIndex + 1] = (i + 2 > segments) ? 1 : i + 2;
            triangles[triIndex + 2] = i + 1;
        }

        return CreateMesh(vertices, triangles, uvs);

        int GetSegmentCountForRadius(float radius, float segmentLength = 0.2f, int minSegments = 6, int maxSegments = 128)
        {
            int segments = Mathf.CeilToInt((2 * Mathf.PI * radius) / segmentLength);
            return Mathf.Clamp(segments, minSegments, maxSegments);
        }
    }

    private Mesh CreateArcMesh(float radius, float angle)
    {
        bool hasRenderer = _meshRenderer != null;

        int segments = GetSegmentCountForArc(radius, angle);
        Vector3[] vertices = new Vector3[segments + 2]; // center + edge points
        int[] triangles = new int[segments * 3];
        Vector2[] uvs = hasRenderer ? new Vector2[segments + 2] : null;

        // Center vertex at index 0
        vertices[0] = Vector3.zero;

        if (hasRenderer)
        {
            uvs[0] = new Vector2(0.5f, 0.5f);
        }

        float startAngle = -(angle / 2);
        float angleStep = angle / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angleDeg = startAngle + i * angleStep;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            float x = Mathf.Sin(angleRad) * radius;
            float z = Mathf.Cos(angleRad) * radius;

            int vertIndex = i + 1;
            vertices[vertIndex] = new Vector3(x, 0, z);

            Vector2 uv = new Vector2((x / (radius * 2f)) + 0.5f, (z / (radius * 2f)) + 0.5f);

            if (hasRenderer)
            {
                uvs[vertIndex] = new Vector2(Mathf.Clamp01(uv.x), Mathf.Clamp01(uv.y));
            }
        }

        for (int i = 0; i < segments; i++)
        {
            int triIndex = i * 3;
            triangles[triIndex] = 0;
            triangles[triIndex + 1] = i + 1;
            triangles[triIndex + 2] = i + 2;
        }

        return CreateMesh(vertices, triangles, uvs);

        int GetSegmentCountForArc(float radius, float arcDegrees, float segmentLength = 0.2f, int minSegments = 6, int maxSegments = 128)
        {
            float arcLength = 2 * Mathf.PI * radius * (arcDegrees / 360f);
            int segments = Mathf.CeilToInt(arcLength / segmentLength);
            return Mathf.Clamp(segments, minSegments, maxSegments);
        }
    }

    private Mesh CreateMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs)
    {
        var mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
        };

        if (uvs != null)
        {
            mesh.uv = uvs;
        }

        return mesh;
    }

    private void CleanupMesh()
    {
        if (_meshFilter.mesh == null)
        {
            return;
        }

        if (TryGetComponent(out MeshCollider meshCollider))
        {
            Destroy(meshCollider);
        }

        Destroy(_meshFilter.mesh);
    }

    private void ResetMesh(Mesh mesh)
    {
        CleanupMesh();

        _meshFilter.mesh = mesh;
        _meshFilter.mesh.RecalculateNormals();

        var meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = _meshFilter.mesh;

        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Token token))
        {
            return;
        }

        if (!TargetingSystem.VerifyTarget(_owner, token.Data, _flags))
        {
            return;
        }

        _collisions.Add(token);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Token token))
        {
            return;
        }

        _collisions.Remove(token);
    }

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField] float _startWidth = 1f;
    [SerializeField] float _endWidth = 1f;
    [SerializeField] float _length = 1f;

    [ContextMenu("Generate Line")]
    public void ContextMenuGenerateLine()
    {
        InitLine(_startWidth, _length);
    }

    [ContextMenu("Generate Cone")]
    public void ContextMenuGenerateCone()
    {
        InitLine(_startWidth, _endWidth, _length);
    }

    [ContextMenu("Generate Tri")]
    public void ContextMenuGenerateTri()
    {
        InitLine(0, _endWidth, _length);
    }

    [ContextMenu("Generate Circle")]
    public void ContextMenuGenerateCircle()
    {
        InitCircle(_startWidth);
    }

    [ContextMenu("Generate Arc")]
    public void ContextMenuGenerateArc()
    {
        InitArc(_startWidth, _endWidth);
    }
#endif
}
