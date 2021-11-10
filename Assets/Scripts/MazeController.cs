using UnityEngine;
using UnityEngine.Tilemaps;
using MazeWorld;
using System;

public class MazeController : MonoBehaviour
{
    public Tilemap tileMap;
    public Tile mazeTile;
    private DFSgener Generator;
    private BFSsolver Solver;
    private MazeWorld.Grid Maze;

    public int Speed = 3;
    private int HaltedFrames = 0;
    public bool Looping = true;
    public bool Auto = true;

    public bool Salting = false; // Whether the Gener should randomly remove some Rocks
    public float SaltFactor = .1f; // The chance that a Rock will be removed

    public bool FinishedWithPhase = false;
    private enum Phases { Generating = 1, Solving = 2 }
    private Phases Phase;

    private int CellUpdateRate;

    void Start()
    {
        Generator = new DFSgener(this, Maze, null);
        Solver = new BFSsolver(this, Maze, null, null);
        this.Reset((20, 10));
        var c = new MazeWorld.Color(255, 0, 255);
        Debug.Log(c.Equals(MazeWorld.Color.Magenta));
        Debug.Log(ColorMap.ConvertColor(c));
    }

    // Update is called once per frame
    void Update()
    {
        // Checks to see if the Maze should move to the next phase
        phaseCheck();

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Maze.Step();
            Debug.Log("Step");
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Maze.Step();
            HaltedFrames--;
        }
        else if (Input.GetKey(KeyCode.Space))
            Maze.Step(getStepRepeatCount());
        else if (Input.GetKeyUp(KeyCode.Space))
            HaltedFrames = 0;
        else if (Input.GetKeyDown(KeyCode.R))
            Reset();
        else if (Input.GetKeyDown(KeyCode.T))
            generate();
        else if (Input.GetKeyDown(KeyCode.Y))
            solve();
        else if (Input.GetKeyDown(KeyCode.A))
            Auto = !Auto;
        else if (Auto) // If nothing else was pressed, then do the Auto check
            Maze.Step(getStepRepeatCount());

        // Changes speed
        if (Input.GetKeyDown(KeyCode.UpArrow))
            Speed++;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            Speed--;

        // Turns Salting on and off.
        // Salting will randomly mark some generated rocks to be removed.
        if (Input.GetKeyDown(KeyCode.S))
            Salting = !Salting;

        // Draw grid to tile map
        DrawGrid();
    }

    private void DrawGrid()
    {
        MazeWorld.Grid g = Maze;
        for (int i = 0; i < g.MaxX; i++)
        {
            for (int j = 0; j < g.MaxY; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                Entity e = g.Get(i, j);
                if (e != null)
                {
                    UnityEngine.Color c = ColorMap.ConvertColor(e.Color);
                    tileMap.SetColor(pos, c);
                }
                else
                {
                    // If there is no entity, then set the color to white
                    tileMap.SetColor(pos, UnityEngine.Color.white);
                }
            }
        }
    }


    private void Reset()
    {
        Generator.Finish();
        Solver.Finish();

        FinishedWithPhase = false;
        Phase = Phases.Generating;
        Looping = true;
        Auto = false;

        Maze.Reset();
        ResetTileMap();
        Generator.Start();
    }

    private void Reset((int, int) maxSize)
    {
        Generator.Finish();
        Solver.Finish();

        FinishedWithPhase = false;
        Phase = Phases.Generating;
        Looping = true;
        Auto = false;

        this.Maze = new MazeWorld.Grid(maxSize.Item1, maxSize.Item2);
        Generator.ForceGridChange(Maze);
        Solver.ForceGridChange(Maze);
        Solver.Target = new Location(Maze.MaxX - 1, Maze.MaxY - 1);
        ResetTileMap();
        Generator.Start();
    }

    private void ResetTileMap()
    {
        tileMap.ClearAllTiles();
        for (int x = 0; x < Maze.MaxX; x++)
        {
            for (int y = 0; y < Maze.MaxY; y++)
            {
                var pos = new Vector3Int(x, y, 0);
                tileMap.SetTile(pos, mazeTile);
                tileMap.SetTileFlags(pos, TileFlags.None);
                tileMap.SetColor(pos, UnityEngine.Color.gray);
            }
        }
    }

    private string GetHeaderText()
    {
        string text;

        text = "Speed: " + Speed +
        " UpdateRate: " + CellUpdateRate +
        " Looping: " + Looping.ToString() +
        " Auto: " + Auto.ToString();
        /*
        if (Ctrl.MouseIsOnGrid())
        {
            Entity e = Grid.Get(Ctrl.LocationOfMouse());
            text += e == null ? "" : " Object:" + e.ToString();
        }
        */

        return text;
    }

    private void SlowDown(double slowFactor)
    {
        CellUpdateRate = (int)Math.Pow(slowFactor, -3);
        if (Solver is BFSsolver)
            ((BFSsolver)Solver).CellUpdateRate = CellUpdateRate;
    }

    private void phaseCheck()
    {
        if (Looping && FinishedWithPhase)
        {
            switch (Phase)
            {
                case Phases.Generating:
                    FinishedWithPhase = false;
                    Phase = Phases.Solving;
                    Solver.Start();
                    break;
                case Phases.Solving:
                    FinishedWithPhase = false;
                    Phase = Phases.Generating;
                    Maze.Reset();
                    Generator.Start();
                    break;
            }
        }
    }


    private void generate()
    {
        if (Phase != Phases.Generating)
        {
            Solver.Finish();

            Phase = Phases.Generating;
            Looping = false;
            Auto = false;

            Maze.Reset();
            Generator.Start();
        }
    }

    private void solve()
    {
        if (Phase != Phases.Solving)
        {
            Generator.Finish();

            Phase = Phases.Solving;
            Looping = false;
            Auto = false;

            Solver.Start();
        }
    }

    private int getStepRepeatCount()
    {
        if (HaltedFrames > 0)
        {
            HaltedFrames--;
            return 0;
        }
        else
        {
            if (Speed < 0)
            {
                HaltedFrames = (int)Math.Pow(2, Math.Abs(Speed));
                return 1;
            }
            else
                return (int)Math.Pow(2, Speed);
        }
    }
}
