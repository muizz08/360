using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SphereCircleDrawer : MonoBehaviour
{
    [Header("Target Sphere")]
    public Transform sphereCenterTransform;
    public float radius = 0f;
    public int segments = 20;

    [Header("Offsets")]
    public float latitudeOffsetY = 0f;   // naik/turun Y
    public float longitudeOffsetX = 0f;  // geser X derajat

    [Header("Line Renderers")]
    public LineRenderer latitudeLine;
    public LineRenderer longitudeLine;

    [Header("Spawn Settings")]
    public GameObject pointPrefab;   // prefab cube/button
    public bool spawnPoints = true;  // aktifkan kalau mau spawn

    [HideInInspector] public Vector3[] latitudePoints;
    [HideInInspector] public Vector3[] longitudePoints;

    [HideInInspector] public Vector2[] latitudeUVs;
    [HideInInspector] public Vector2[] longitudeUVs;

    public bool isLongitude = false;

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.loop = true;
        lr.positionCount = segments + 1;

        DrawCircle();

        if (latitudeLine != null)
        {
            latitudeLine.positionCount = segments + 1;
            DrawLatitude();
        }

        if (longitudeLine != null)
        {
            longitudeLine.positionCount = segments + 1;
            DrawLongitude();
        }

        if (spawnPoints && pointPrefab != null)
        {
            SpawnPrefabs(latitudePoints);
            SpawnPrefabs(longitudePoints);
        }

        // 🔹 Generate grid UV (misalnya 10x5)
        GenerateGridUV(10, 5);
    }

    private void DrawLatitude()
    {
        latitudePoints = new Vector3[segments + 1];
        latitudeUVs = new Vector2[segments + 1];

        Vector3 center = sphereCenterTransform.position;
        float sphereRadius = sphereCenterTransform.GetComponent<MeshRenderer>().bounds.extents.x;

        for (int i = 0; i <= segments; i++)
        {
            float theta = (float)i / segments * Mathf.PI * 2f;
            float x = Mathf.Cos(theta) * sphereRadius;
            float z = Mathf.Sin(theta) * sphereRadius;
            float y = latitudeOffsetY; // ketinggian (naik/turun)

            Vector3 pos = center + new Vector3(x, y, z);
            latitudeLine.SetPosition(i, pos);
            latitudePoints[i] = pos;

            // Konversi ke UV
            latitudeUVs[i] = WorldToUV(pos);
            Debug.Log($"Latitude point {i} UV: {latitudeUVs[i]}");
        }
    }

    private void DrawLongitude()
    {
        longitudePoints = new Vector3[segments + 1];
        longitudeUVs = new Vector2[segments + 1];

        Vector3 center = sphereCenterTransform.position;
        float sphereRadius = sphereCenterTransform.GetComponent<MeshRenderer>().bounds.extents.x;
        float offsetRad = longitudeOffsetX * Mathf.Deg2Rad;

        for (int i = 0; i <= segments; i++)
        {
            float theta = (float)i / segments * Mathf.PI * 2f;
            float y = Mathf.Sin(theta) * sphereRadius;
            float z = Mathf.Cos(theta + offsetRad) * sphereRadius;

            Vector3 pos = center + new Vector3(0, y, z);
            longitudeLine.SetPosition(i, pos);
            longitudePoints[i] = pos;

            // Konversi ke UV
            longitudeUVs[i] = WorldToUV(pos);
            Debug.Log($"Longitude point {i} UV: {longitudeUVs[i]}");
        }
    }

    void DrawCircle()
    {
        Vector3[] points = new Vector3[segments + 1];
        Vector3 center = sphereCenterTransform.position;

        float sphereRadius = 0.5f * sphereCenterTransform.localScale.x;

        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;

            if (isLongitude) // lingkaran vertikal
            {
                points[i] = center + new Vector3(
                    Mathf.Cos(angle) * sphereRadius,
                    Mathf.Sin(angle) * sphereRadius,
                    0f
                );
            }
            else // lingkaran horizontal (ekuator)
            {
                points[i] = center + new Vector3(
                    Mathf.Cos(angle) * sphereRadius,
                    0f,
                    Mathf.Sin(angle) * sphereRadius
                );
            }
        }

        lr.SetPositions(points);
    }

    /// <summary>
    /// Konversi world position ke koordinat UV (0–1)
    /// </summary>
    private Vector2 WorldToUV(Vector3 worldPos)
    {
        Vector3 localPos = (worldPos - sphereCenterTransform.position).normalized;

        // longitude (u)
        float u = 0.5f + (Mathf.Atan2(localPos.z, localPos.x) / (2 * Mathf.PI));

        // latitude (v)
        float v = 0.5f - (Mathf.Asin(localPos.y) / Mathf.PI);

        return new Vector2(u, v);
    }

    /// <summary>
    /// Buat grid UV kayak di gambar (0.0, 0.0) → (0.9, 1.0)
    /// </summary>
    void GenerateGridUV(int cols, int rows)
    {
        for (int y = 0; y <= rows; y++)
        {
            string row = "";
            for (int x = 0; x <= cols; x++)
            {
                float u = (float)x / cols;
                float v = (float)y / rows;

                row += $"({u:F1}, {v:F1})  ";
            }
            Debug.Log(row);
        }
    }

    private void SpawnPrefabs(Vector3[] points)
    {
        if (points == null) return;
        for (int i = 0; i < points.Length; i++)
        {
            Instantiate(pointPrefab, points[i], Quaternion.identity, transform);
        }
    }
}
