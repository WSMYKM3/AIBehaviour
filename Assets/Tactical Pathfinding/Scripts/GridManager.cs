using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class GridManager : MonoBehaviour
{
    public int gridSizeX = 10;
    public int gridSizeZ = 10;//attention here is Z not Y since Y axis is up and down
    public float cellSize = 1.0f;

    public TextMeshPro textPrefab;//show value of every single node
    public Material dangerZoneMaterial;//color the dangerZone

    Node[,] grid;

    //int here represents eight-way segments, eight 45 degrees of aera
    //each position will be mapped to a 8-sector bitmask
    readonly Dictionary<Vector3, int> nodeSectorData = new();

    public struct Node
    {
        public Vector3 position;
        public bool isWalkable;

        public Node(Vector3 position, bool isWalkable = false)
        {
            this.position = position;
            this.isWalkable = isWalkable;
        }
    }
    void PrecomputeSectors()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");//remember to set enemy tag

        foreach (var node in grid)
        {
            if (!node.isWalkable) continue;

            int sectorMask = 0;
            foreach(var enemy in enemies){
                if (enemy.GetComponent<Enemy>() is not { } enemyComponent) continue;

                float detectionRadius = enemyComponent.detectionRadius;
                Vector3 direction = (node.position - enemy.transform.position).normalized;
                float distance = Vector3.Distance(node.position, enemy.transform.position);

                if(distance >detectionRadius) continue;
                if(IsObstructed(enemy.transform.position, node.position)) continue;
            }
        }
    }

    int GetSectorForDirection(Vector3 direction)
    {

    }

    bool IsObstructed(Vector3 from, Vector3 to)
    {
        return Physics.Raycast(from, (to - from).normalized,Vector3.Distance(from, to));//origin, direction, maxDistance
    }

    void InitializeGrid()
    {
        grid = new Node[gridSizeX, gridSizeZ];
        for(int x =0; x < gridSizeX; x++)
        for(int z =0; z < gridSizeZ; z++){
            var pos = new Vector3(x, 0, z) * cellSize;
            grid[x,z] = new Node(pos, IsPositionOnNavMesh(pos));
        }
    }

    bool IsPositionOnNavMesh(Vector3 position) => NavMesh.SamplePosition(position, out _, cellSize / 2, NavMesh.AllAreas); //out _ is a placeholder since I do not need the position back
}
