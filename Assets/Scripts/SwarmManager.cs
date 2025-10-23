using System.Collections.Generic;
using UnityEngine;

public class SwarmManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject boidPrefab;
    public int numberOfBoids = 200;
    public float spawnRadius = 10f;

    [Header("Simulation Bounds")]
    public Vector3 bounds = new Vector3(30, 30, 30);
    public float boundsSteerForce = 3f;

    [Header("Boid Behavior Weights")]
    [Range(0f, 5f)]
    public float cohesionWeight = 1f;
    [Range(0f, 5f)]
    public float separationWeight = 2f;
    [Range(0f, 5f)]
    public float alignmentWeight = 1f;

    [Header("Boid General Settings")]
    public float perceptionRadius = 5f;
    public float maxSpeed = 4f;

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleLayer;
    public float obstacleLookAhead = 5f;
    public float boidRadius = 0.5f;
    [Range(0f, 20f)]
    public float obstacleWeight = 10f;

    [Header("Target Following")]
    public Transform target;
    [Range(0f, 10f)]
    public float targetWeight = 2f;

    [HideInInspector]
    public List<Boid> allBoids = new List<Boid>();

    // Grid optimization
    private float gridCellSize;
    private Dictionary<Vector3Int, List<Boid>> grid = new Dictionary<Vector3Int, List<Boid>>();

    void Awake()
    {
        gridCellSize = perceptionRadius;

        for (int i = 0; i < numberOfBoids; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            GameObject boidGO = Instantiate(boidPrefab, randomPos, Quaternion.identity, this.transform);

            Boid boid = boidGO.GetComponent<Boid>();
            boid.manager = this;
            allBoids.Add(boid);
        }
    }

    void LateUpdate()
    {
        UpdateGrid();
    }

    void UpdateGrid()
    {
        grid.Clear();

        foreach (var boid in allBoids)
        {
            Vector3Int cellCoords = GetCellCoords(boid.transform.position);

            if (!grid.ContainsKey(cellCoords))
            {
                grid[cellCoords] = new List<Boid>();
            }
            grid[cellCoords].Add(boid);
        }
    }

    private Vector3Int GetCellCoords(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / gridCellSize),
            Mathf.FloorToInt(position.y / gridCellSize),
            Mathf.FloorToInt(position.z / gridCellSize)
        );
    }

    public List<Boid> GetNeighborsUsingGrid(Boid currentBoid)
    {
        List<Boid> neighbors = new List<Boid>();
        Vector3Int boidCell = GetCellCoords(currentBoid.transform.position);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    Vector3Int cellToSearch = new Vector3Int(boidCell.x + x, boidCell.y + y, boidCell.z + z);

                    if (grid.TryGetValue(cellToSearch, out List<Boid> boidsInCell))
                    {
                        foreach (var otherBoid in boidsInCell)
                        {
                            if (otherBoid == currentBoid) continue;

                            if (Vector3.Distance(currentBoid.transform.position, otherBoid.transform.position) < perceptionRadius)
                            {
                                neighbors.Add(otherBoid);
                            }
                        }
                    }
                }
            }
        }
        return neighbors;
    }
}