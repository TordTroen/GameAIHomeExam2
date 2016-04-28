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
		public const double NodeSize = 50;
		public const int Width = 16;
		public const int Height = 12;

		public List<MapNode> map = new List<MapNode>();
		public List<MapNode> obstacles = new List<MapNode>();
		private int[] simpleMap = new int[] 
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
		private MapNode[] PossibleNeighbours = new MapNode[]
		{
			new MapNode(1, 0),
			new MapNode(-1, 0),
			new MapNode(0, 1),
			new MapNode(0, -1)
		};
		private readonly Trotor14_MechaGodzilla robot;

		public CollisionMap(Trotor14_MechaGodzilla robot)
		{
			this.robot = robot;
			InitializeMap();
		}

		private void InitializeMap()
		{
			//map = new MapNode[simpleMap.Length];
			//robot.Out.WriteLine("Lenght: " + simpleMap.Length);
			int x = 0;
			int y = Height-1;
			for (int i = 0; i < simpleMap.Length; i ++)
			{
				//int mod = (i + 1) % Width;
				//if (mod == 0)
				if (x >= Width)
				{
					y--;
					x = 0;
				}
				MapNode node = new MapNode(x, y);
				if (node.IsInsideMapBounds())
				{
					if (simpleMap[i] == 0)
					{
						map.Add(node);
					}
					else
					{
						obstacles.Add(node);
					}
				}
				
				//robot.Out.WriteLine(string.Format("[{0}, {1}]: {2}", x, y, simpleMap[i] == 1));
				//robot.Out.WriteLine(string.Format("[{0}, {1}]: {2} % {3} = {4}", x, y, i+1, Width, mod));
				x++;
			}

			//foreach (var node in map)
			//{
			//	node.Neighbours = GetNeighbours(node);
			//}
			//PaintMap();
		}

		public MapNode GetNode(Vector2D pos)
		{
			//if (map == null) return null;

			int x = (int)(pos.X / NodeSize);
			int y = (int)(pos.Y / NodeSize);
			return GetNode(x, y);
		}

		public MapNode GetNode(int x, int y)
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
				if (n == null)
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
			else
			{
				robot.Out.WriteLine("Node: " + comp + " is outside map bounds");
			}

			if (n == null)
			{
				robot.Out.WriteLine("Couldn't find node: " + comp);
			}

			return n;
		}

		private List<MapNode> GetNeighbours(MapNode node)
		{
			var neighbours = new List<MapNode>();
			foreach (var direction in PossibleNeighbours)
			{
				Predicate<MapNode> nodeFinder = (MapNode n) => n.X == node.X + direction.X && n.Y == node.Y + direction.Y;
				MapNode neighbour = map.Find(nodeFinder);
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
				Vector2D pos = node.GetPhysicalPosition();
				robot.Drawing.DrawBox(Color.Yellow, pos, 127);
			}
		}
	}
}
