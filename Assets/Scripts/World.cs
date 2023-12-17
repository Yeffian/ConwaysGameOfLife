using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile deadTile;
    [SerializeField] private Pattern pattern;
    [SerializeField] private float updateInterval = 0.05f;
    [SerializeField] private TextMeshProUGUI iterationText;
    [SerializeField] private TextMeshProUGUI populationText;

    private HashSet<Vector3Int> aliveCells;
    private HashSet<Vector3Int> cellsToCheck;

    public int Population { get; private set; }
    public int Iterations { get; private set; }
    public float Time { get; private set; }
    
    private bool IsAlive(Vector3Int cell) => currentState.GetTile(cell) == aliveTile;

    private void Awake()
    {
        aliveCells = new HashSet<Vector3Int>();
        cellsToCheck = new HashSet<Vector3Int>();
    }

    private void Start()
    {
        SetPattern(pattern);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2)) Reset();
    }

    private void OnEnable()
    {
        StartCoroutine(Simulate());
    }

    private void SetPattern(Pattern pattern)
    {
        Reset();

        Vector2Int center = pattern.GetCenter();

        foreach (var cellCoord in pattern.cells)
        {
            Vector3Int cell = (Vector3Int)(cellCoord - center);
            currentState.SetTile(cell, aliveTile);
            aliveCells.Add(cell);
        }

        Population = aliveCells.Count;
    }

    private void Reset()
    {
        aliveCells.Clear();
        cellsToCheck.Clear();
        
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
        
        Population = 0;
        Iterations = 0;
        Time = 0f;
    }

    private IEnumerator Simulate()
    {
        var interval = new WaitForSeconds(updateInterval);
        yield return interval;

        while (enabled)
        {
            iterationText.text = $"Iterations: {Iterations}";
            populationText.text = $"Population: {Population}";
            
            UpdateState();

            Population = aliveCells.Count;
            Iterations++;
            Time += updateInterval;

            yield return interval;
        }
    }

    private void UpdateState()
    {
        cellsToCheck.Clear();

        foreach (Vector3Int cell in aliveCells)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    cellsToCheck.Add(cell + new Vector3Int(x, y));
                }
            }
        }

        // cell behaviour
        foreach (Vector3Int cell in cellsToCheck)
        {
            int neighbors = CountNeighbors(cell);
            bool alive = IsAlive(cell);

            if (!alive && neighbors == 3) BirthCell(cell);
            else if (alive && (neighbors < 2 || neighbors > 3)) KillCell(cell);
            else nextState.SetTile(cell, currentState.GetTile(cell));
        }

        // swap states
        (currentState, nextState) = (nextState, currentState);
        nextState.ClearAllTiles();
    }

    private void KillCell(Vector3Int cell)
    {
        nextState.SetTile(cell, deadTile);
        aliveCells.Remove(cell);
    }

    private void BirthCell(Vector3Int cell)
    {
        nextState.SetTile(cell, aliveTile);
        aliveCells.Add(cell);
    }

    private int CountNeighbors(Vector3Int cell)
    {
        int count = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighbor = cell + new Vector3Int(x, y);

                if (x == 0 && y == 0) continue;
                if (IsAlive(neighbor)) count++;
            }
        }

        return count;
    }
}