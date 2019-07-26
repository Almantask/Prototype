using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Common;
using Project.Core.Characters;
using Project.Settings;
using UnityEngine;

namespace Project.Core.Grid {
    public class GridManager : MonoBehaviour {
        public int width = 12;
        public int depth = 12;

        private Dictionary<Vector3, Tile> Grid { get; set; }
        private Dictionary<Vector3, Edge> Edges { get; set; }

        private GameObject _parent;
        private GridSettings settings;

        private Dictionary<Tile, List<Tile>> _cache;
        private Dictionary<Tile, Dictionary<Tile, int>> _graph;

        public void InitializeGrid() {
            settings = GameManager.Settings.gridSettings;
            CreateParent();
            CreateGrid();
            CreateEdges();
            SetTileNeighbours();
            SetTileEdges();
        }

        private void CreateGrid() {
            Grid = new Dictionary<Vector3, Tile>();
            for(int x = 0; x < width; x++) {
                for(int z = 0; z < depth; z++) {
                    Vector3 position = new Vector3(x, 0, z);
                    Tile tile = Instantiate(settings.prefab, position, Quaternion.identity, _parent.transform);
                    tile.Initialize(position);
                    tile.Renderer.material = settings.tileMaterial;
                    tile.name = "Tile[" + (x + 1) + "," + (z + 1) + "]";
                    Grid.Add(position, tile);
                }
            }
        }

        private void CreateParent() {
            _parent = new GameObject("Grid") {
                layer = 10
            };
            var boxCollider = _parent.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(width * 1.05f, 0, depth * 1.05f);
            var position = _parent.transform.position;
            boxCollider.transform.position = new Vector3(position.x + (width / 2f) - 0.5f, 0,
                position.z + (depth / 2f) - 0.5f);
        }

        public Tile GetTile(Vector3 position) {
            var x = Mathf.RoundToInt(position.x);
            var z = Mathf.RoundToInt(position.z);
            Grid.TryGetValue(new Vector3(x, 0, z), out Tile tile);
            return tile;
        }

        public Tile GetTile(int x, int z) {
            var _x = Mathf.Clamp(x, 0, width - 1);
            var _z = Mathf.Clamp(z, 0, depth - 1);
            return GetTile(new Vector3(_x, 0, _z));
        }

        private void CreateEdges() {
            Edges = new Dictionary<Vector3, Edge>();
            foreach(var tile in Grid.Values) {
                foreach(var direction in Tile.Directions) {
                    if(direction == Tile.Directions[0]) continue;
                    if(direction == Tile.Directions[2]) continue;
                    if(direction == Tile.Directions[5]) continue;
                    if(direction == Tile.Directions[7]) continue;
                    var position = tile.Position + direction / 2;
                    if(Edges.ContainsKey(position)) continue;
                    var edge = new Edge(position);
                    var roll = Random.Range(0, 100);
                    if(roll < 10) edge.Walled = true;
                    if(edge.Walled) {
                        var obj = Instantiate(settings.wall);
                        obj.transform.position = edge.Position;
                        edge.wall = obj;
                    }
                    Edges.Add(position, edge);
                }
            }
        }

        public Edge GetEdge(Vector3 position) {
            var x = Mathf.Round(position.x * 2) / 2;
            var z = Mathf.Round(position.z * 2) / 2;
            Edges.TryGetValue(new Vector3(x, 0, z), out Edge edge);
            return edge;
        }

        private void SetTileEdges() {
            foreach(var tile in Grid.Values) {
                foreach(var direction in Tile.Directions) {
                    var edge = GetEdge(tile.Position + (direction / 2));
                    if(edge == null) continue;
                    tile.Edges.Add(edge);
                }
                foreach(var edge in tile.Edges) {
                    if(edge == tile.Edges[0] || edge == tile.Edges[3]) {
                        if(edge.wall == null) continue;
                        edge.wall.transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                }
            }
        }

        private void SetTileNeighbours() {
            foreach(var tile in Grid.Values) {
                for(int x = -1; x <= 1; x++) {
                    for(int z = -1; z <= 1; z++) {
                        if(x == 0 && z == 0) continue;
                        var direction = new Vector3(x, 0, z);
                        var neighbour = GetTile(tile.Position + direction);
                        if(!neighbour) continue;
                        tile.Neighbours.Add(neighbour);
                    }
                }
            }
        }

        public void SetOutlineMaterials(List<Tile> range, bool movementRange) {
            foreach(var tile in range) {
                if(tile.Character) continue;
                tile.Renderer.material = settings.GetOutlineMaterial(GetMaterialIndex(range, tile));
                if(movementRange) {
                    tile.Renderer.material.SetColor("_BaseColor", settings.movementRange);
                } else {
                    tile.Renderer.material.SetColor("_BaseColor", settings.abilityRange);
                }
                if(GetMaterialIndex(range, tile) == 15) {
                    tile.Renderer.material.SetColor("_BaseColor", new Color(0, 0, 0, 0));
                }
            }
        }

        private int ConvertBoolToInt(bool value) {
            return value == true ? 1 : 0;
        }

        private int GetMaterialIndex(List<Tile> range, Tile tile) {
            var northNeighbour = GetTile(tile.Position + Tile.Directions[1]);
            var westNeighbour = GetTile(tile.Position + Tile.Directions[3]);
            var eastNeighbour = GetTile(tile.Position + Tile.Directions[4]);
            var southNeighbour = GetTile(tile.Position + Tile.Directions[6]);
            int north = 0;
            int west = 0;
            int east = 0;
            int south = 0;
            if(northNeighbour) {
                north = ConvertBoolToInt(range.Contains(northNeighbour));
            }
            if(northNeighbour) {
                west = ConvertBoolToInt(range.Contains(westNeighbour));
            }
            if(northNeighbour) {
                east = ConvertBoolToInt(range.Contains(eastNeighbour));
            }
            if(northNeighbour) {
                south = ConvertBoolToInt(range.Contains(southNeighbour));
            }
            int index = north + 2 * west + 4 * east + 8 * south;
            return index;
        }

        private bool IsTileConnected(Tile a, Tile b) {
            return a.IsConnected(b);
        }

        private bool IsTileTraversable(Tile tile) {
            return tile.IsTraversable();
        }

        public List<Tile> FindPath(Tile origin, Tile destination) {
            var frontier = new PriorityQueue<Tile>();
            var cameFrom = new Dictionary<Tile, Tile>();
            var costSoFar = new Dictionary<Tile, int>();
            frontier.Enqueue(origin, 0);
            cameFrom.Add(origin, default);
            costSoFar.Add(origin, 0);
            while(frontier.Count != 0) {
                var current = frontier.Dequeue();
                if(current == destination) break;
                var neighbours = GetNeighbours(_graph, current);
                foreach(var next in neighbours) {
                    var cost = costSoFar[current] + _graph[current][next];
                    if(!costSoFar.ContainsKey(next) || cost < costSoFar[next]) {
                        costSoFar[next] = cost;
                        var priority = cost + GetEuclideanDistance(next, destination);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }
            var path = new List<Tile>();
            if(!cameFrom.ContainsKey(destination)) return path;
            path.Add(destination);
            var temp = destination;
            while(temp != origin) {
                var waypoint = cameFrom[temp];
                path.Add(waypoint);
                temp = waypoint;
            }
            if(!path.Contains(origin)) {
                path.Add(origin);
            }
            path.Reverse();
            return path;
        }

        public List<Vector3> SmoothPath(List<Tile> path) {
            if(path == null || path.Count < 3) {
                return new List<Vector3>();
            }
            var points = new List<Vector3>();
            for(int index = 0; index < path.Count; index++) {
                points.Add(path[index].Position);
            }
            var segmentCount = 10;
            var smoothedPath = new List<Vector3>();
            Vector3 p0, p1, p2;
            for(int j = 0; j < points.Count - 2; j++) {
                // determine control points of segment
                p0 = 0.5f * (points[j] + points[j + 1]);
                p1 = points[j + 1];
                p2 = 0.5f * (points[j + 1] + points[j + 2]);

                // set points of quadratic Bezier curve
                Vector3 position;
                float t;
                float pointStep = 1.0f / segmentCount;
                if(j == points.Count - 3) {
                    pointStep = 1.0f / (segmentCount - 1.0f);
                    // last point of last segment should reach p2
                }
                for(int i = 0; i < segmentCount; i++) {
                    t = i * pointStep;
                    position = (1.0f - t) * (1.0f - t) * p0
                    + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
                    smoothedPath.Add(position);
                }
            }
            return smoothedPath;
        }

        public List<Tile> FindAbilityRange(Tile origin, int distance) {
            var range = new List<Tile>();
            for(var x = -distance; x <= distance; x++) {
                for(var z = -distance; z <= distance; z++) {
                    var position = new Vector3(x, 0, z);
                    var tile = GetTile(origin.Position + position);
                    if(tile == null || tile == origin) continue;
                    var euclidean = GetEuclideanDistance(tile, origin);
                    if(euclidean <= distance + 0.5f && !range.Contains(tile)) {
                        range.Add(tile);
                    }
                }
            }
            return range;
        }

        public List<Tile> FindMovementRange(Tile origin, int distance) {
            _graph = GetGraph();
            var movementPoints = distance * 5;
            var range = new List<Tile>();
            foreach(var tile in FindAbilityRange(origin, distance)) {
                if(tile == origin) continue;
                var path = FindPath(origin, tile);
                if(GetPathCost(path) > movementPoints) continue;
                foreach(var node in path) {
                    if(range.Contains(node) || node.Equals(origin)) continue;
                    range.Add(node);
                }
            }
            return range;
        }

        private Dictionary<Tile, Dictionary<Tile, int>> GetGraph() {
            _graph = new Dictionary<Tile, Dictionary<Tile, int>>();
            foreach(var tile in Grid.Values) {
                _graph[tile] = new Dictionary<Tile, int>();
                foreach(var neighbour in tile.Neighbours.FindAll(IsTileTraversable)) {
                    if(!neighbour.IsConnected(tile)) continue;
                    _graph[tile][neighbour] = tile.GetMovementCost(neighbour);
                }
            }
            return _graph;
        }

        private int GetPathCost(List<Tile> p) {
            var cost = 0;
            for(int i = 0; i < p.Count - 1; i++) {
                cost += _graph[p[i]][p[i + 1]];
            }
            return cost;
        }


        private List<Tile> GetNeighbours(Dictionary<Tile, Dictionary<Tile, int>> graph, Tile tile) {
            if(!graph.ContainsKey(tile)) return new List<Tile>();
            return graph[tile].Keys.ToList();
        }

        private float GetEuclideanDistance(Tile a, Tile b) {
            return Mathf.Sqrt(Mathf.Pow(a.Position.x - b.Position.x, 2) + Mathf.Pow(a.Position.z - b.Position.z, 2));
        }
    }
}