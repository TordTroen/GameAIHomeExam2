using HomeExam.Helpers;
using PG4500_2016_Exam2;
using Robocode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeExam
{
	public class CollisionMap
	{
		public const double NodeSize = 50; // The size of each node or cell
		public const int Width = 16; // Number of nodes across
		public const int Height = 12; // Number of nodes down
		private const double WallNodeWeight = double.MaxValue; // The weight of a wall node, hould be really high so we don't pass them. (shouldn't matter as those nodes aren't included in the search.

		// TODO Could store the nodes in a dictionary with x,y as key for faster lookup
		public List<MapNode> map = new List<MapNode>(); // The nodes we can move through
		public List<MapNode> obstacles = new List<MapNode>(); // The nodes we can't move through

		// The collision map we have been provided
		private readonly int[] rawMap =
		{
			0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0,
			1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0,
			1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0,
			0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0,
			0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0,
			0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0,
			0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0,
			0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		};

		// List of "direction" we look for neighbours in (can remove the last 4 to only have neighbours horizontally and vertically)
		private readonly MapNode[] PossibleNeighbours = 
		{
			new MapNode(1, 0),		// Right
			new MapNode(-1, 0),		// Left
			new MapNode(0, 1),		// Up
			new MapNode(0, -1),		// Down
			new MapNode(-1, -1),	// Down-left
			new MapNode(1, 1),		// Up-right
			new MapNode(-1, 1),		// Up-left
			new MapNode(1, -1)		// Down-right
		};
		private readonly Trotor14MechaGodzilla robot;

		public CollisionMap(Trotor14MechaGodzilla robot)
		{
			this.robot = robot;
			InitializeMap();
		}

		private void InitializeMap()
		{
			int x = 0;
			int y = Height-1;
			for (int i = 0; i < rawMap.Length; i ++)
			{
				// If we have reached the end of the row, go to the next row
				if (x >= Width)
				{
					y--;
					x = 0;
				}

				// Create a new node with the current x and y
				MapNode node = new MapNode(x, y);
				if (node.IsInsideMapBounds())
				{
					int rawNode = rawMap[i];
					// Add the node to the map or the obstacles list based on what the simplemap value is
					if (rawNode == 1)
					{
						obstacles.Add(node);
						//map.Add(node);
						node.Weight = WallNodeWeight;
					}
					else
					{
						map.Add(node);
						node.Weight = 0;
						//if (rawNode == 2)
						//{
						//	node.Weight = 50;
						//}
					}
				}
				
				x++;
			}

			// Find and store the neighbours of all the nodes in both the map and obstacles list
			foreach (var node in map)
			{
				node.Neighbours = GetNeighbours(node);
			}
			foreach (var node in obstacles)
			{
				node.Neighbours = GetNeighbours(node);
			}
		}

		public MapNode GetNode(Vector2D pos, bool includeObstacles)
		{
			// Calculate the x and y based on the position given
			int x = (int)(pos.X / NodeSize);
			int y = (int)(pos.Y / NodeSize);
			return GetNode(x, y, includeObstacles);
		}

		/// <summary>
		/// Returns a MapNode given an x and y. Returns null if there is no node found with the given x and y.
		/// </summary>
		public MapNode GetNode(int x, int y, bool includeObstacles)
		{
			MapNode n = null;
			MapNode comp = new MapNode(x, y);

			// Check if the node we want to find is inside the map bounds
			if (comp.IsInsideMapBounds())
			{
				// Try to find a matching node in the map
				foreach (var node in map)
				{
					if (node == comp)
					{
						n = node;
						break;
					}
				}

				// If we couldn't find a node in the map, look for it in the obstacles
				if (n == null && includeObstacles)
				{
					// Try to find a matching node in the list of obstacles
					foreach (var node in obstacles)
					{
						if (node == comp)
						{
							n = node;
							break;
						}
					}
				}
			}

			return n;
		}

		private List<MapNode> GetNeighbours(MapNode node)
		{
			var neighbours = new List<MapNode>();
			foreach (var direction in PossibleNeighbours)
			{
				MapNode neighbour = GetNode(node.X + direction.X, node.Y + direction.Y, false);
				if (neighbour != null)
				{
					neighbours.Add(neighbour);
				}
			}
			return neighbours;
		}

		public void PaintMap()
		{
			foreach (var node in map)
			{
				Vector2D pos = node.PhysicalPosition;
				robot.Drawing.DrawBox(Color.Yellow, pos, 127);
			}
		}
	}
}
