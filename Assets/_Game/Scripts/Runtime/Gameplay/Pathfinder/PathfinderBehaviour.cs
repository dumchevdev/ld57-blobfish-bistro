using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Gameplay.Pathfinder
{
    public class PathfinderBehaviour : MonoBehaviour
    {
        [SerializeField] private float _nodeRadius = 0.25f;
        [SerializeField] private Vector2 _gridSize = new Vector2(20, 10);

        [Header("Debug")] [SerializeField] public bool _drawGridInEditor = false;
        [SerializeField] private Color _walkableColor = Color.green;
        [SerializeField] private Color _obstacleColor = Color.red;
        [SerializeField] private Color _gridColor = Color.white;

        private Node[,] _grid;

        private class Node
        {
            public bool Walkable;
            public Vector2 WorldPosition;
            public int GridX, GridY;
            public int GCost, HCost;
            public Node Parent;

            public int FCost => GCost + HCost;
        }

        public void CreateGrid()
        {
            var gridWorldSize = new Vector2Int(
                Mathf.RoundToInt(_gridSize.x / (_nodeRadius * 2)),
                Mathf.RoundToInt(_gridSize.y / (_nodeRadius * 2)));

            _grid = new Node[gridWorldSize.x, gridWorldSize.y];

            var worldBottomLeft = (Vector2)transform.position - Vector2.right * _gridSize.x / 2 -
                                  Vector2.up * _gridSize.y / 2;

            for (int x = 0; x < gridWorldSize.x; x++)
            {
                for (int y = 0; y < gridWorldSize.y; y++)
                {
                    var worldPoint = worldBottomLeft +
                                     Vector2.right * (x * _nodeRadius * 2 + _nodeRadius) +
                                     Vector2.up * (y * _nodeRadius * 2 + _nodeRadius);

                    bool walkable = !Physics2D.OverlapCircle(worldPoint, _nodeRadius, LayerMask.GetMask("Obstacle"));

                    _grid[x, y] = new Node
                    {
                        Walkable = walkable,
                        WorldPosition = worldPoint,
                        GridX = x,
                        GridY = y
                    };
                }
            }
        }

        public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
        {
            var startNode = NodeFromWorldPoint(startPos);
            var targetNode = NodeFromWorldPoint(targetPos);

            if (startNode == null || targetNode == null || !targetNode.Walkable)
                return null;

            var openSet = new List<Node> { startNode };
            var closedSet = new HashSet<Node>();

            startNode.GCost = 0;
            startNode.HCost = GetDistance(startNode, targetNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost ||
                        (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (var neighbor in GetNeighbors(currentNode))
                {
                    if (!neighbor.Walkable || closedSet.Contains(neighbor))
                        continue;

                    int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return null;
        }

        private List<Vector2> RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Vector2>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.WorldPosition);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        private List<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.GridX + x;
                    int checkY = node.GridY + y;

                    if (checkX >= 0 && checkX < _grid.GetLength(0) &&
                        checkY >= 0 && checkY < _grid.GetLength(1))
                    {
                        neighbors.Add(_grid[checkX, checkY]);
                    }
                }
            }

            return neighbors;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        private Node NodeFromWorldPoint(Vector2 worldPosition)
        {
            float percentX = (worldPosition.x + _gridSize.x / 2) / _gridSize.x;
            float percentY = (worldPosition.y + _gridSize.y / 2) / _gridSize.y;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((_grid.GetLength(0) - 1) * percentX);
            int y = Mathf.RoundToInt((_grid.GetLength(1) - 1) * percentY);

            return _grid[x, y];
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_drawGridInEditor) return;

            if (_grid == null || _grid.Length == 0)
            {
                DrawGridPreview();
                return;
            }

            foreach (var node in _grid)
            {
                Gizmos.color = node.Walkable ? _walkableColor : _obstacleColor;
                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * (_nodeRadius * 2 * 0.9f));
            }

            Gizmos.color = _gridColor;
            var center = transform.position;
            Gizmos.DrawWireCube(center, new Vector3(_gridSize.x, _gridSize.y, 0));
        }

        private void DrawGridPreview()
        {
            Gizmos.color = _gridColor;
            var center = transform.position;
            Gizmos.DrawWireCube(center, new Vector3(_gridSize.x, _gridSize.y, 0));

            int gridSizeX = Mathf.RoundToInt(_gridSize.x / (_nodeRadius * 2));
            int gridSizeY = Mathf.RoundToInt(_gridSize.y / (_nodeRadius * 2));

            var worldBottomLeft = (Vector2)center - Vector2.right * _gridSize.x / 2 - Vector2.up * _gridSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    var worldPoint = worldBottomLeft +
                                     Vector2.right * (x * _nodeRadius * 2 + _nodeRadius) +
                                     Vector2.up * (y * _nodeRadius * 2 + _nodeRadius);

                    Gizmos.DrawWireSphere(worldPoint, _nodeRadius * 0.5f);
                }
            }
        }
#endif
    }
}