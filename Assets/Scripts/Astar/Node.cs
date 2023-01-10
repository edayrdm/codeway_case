using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// This calss is a node use by the AStar algorithm
/// </summary>
[Serializable]
public class Node
{
    /// <summary>
    /// The nodes grid position
    /// </summary>
    public Point GridPosition { get; private set; }

    /// <summary>
    /// The nodes position in the world, this is more a reference to the tile that the node is connected to
    /// </summary>
    public Vector2 WorldPosition { get; set; }

    /// <summary>
    /// A reference to the tile that this node belongs to
    /// </summary>
    public TileScript TileRef { get;  private set; }

    /// <summary>
    /// A reference to the nodes parent
    /// </summary>
    public Node Parent { get; private set; }

    public int G { get; set; }

    public int H { get; set; }

    public int F { get; set; }

    /// <summary>
    /// The node's constuctor
    /// </summary>
    /// <param name="tileRef">A reference to the tilescript</param>
    public Node(TileScript tileRef)
    {
        this.TileRef = tileRef;
        this.GridPosition = tileRef.GridPosition;
        this.WorldPosition = tileRef.WorldPosition;
    }

    /// <summary>
    /// Calculate all values for the node
    /// </summary>
    /// <param name="parent">The node's parent</param>
    public void CalcValues(Node parent,Node goal ,int gCost)
    {
        this.Parent = parent;
        this.G = parent.G + gCost;
        this.H = ((Math.Abs(GridPosition.X - goal.GridPosition.X)) + Math.Abs((goal.GridPosition.Y - GridPosition.Y))) * 10;
        this.F = G + H;
    }

}
