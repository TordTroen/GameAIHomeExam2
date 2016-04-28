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
		private Vector2D[] PossibleNeighbours = new Vector2D[]
		{
			new Vector2D(1, 0),
			new Vector2D(-1, 0),
			new Vector2D(0, 1),
			new Vector2D(0, -1)
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
				if (simpleMap[i] == 0)
				{
					map.Add(new MapNode(x, y));
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

			MapNode n = null;
			int x = (int)(pos.X / NodeSize);
			int y = (int)(pos.Y / NodeSize);
			n = GetNode(x, y);
			return n;
		}

		public MapNode GetNode(int x, int y)
		{
			MapNode n = null;
			MapNode comp = new MapNode(x, y);
			foreach (var node in map)
			{
				if (node == comp)
				{
					n = node;
					break;
				}
			}

			if (n == null)
			{
				// TODO look in the map of obstacles
				robot.Out.WriteLine("Couldn't find node: " + comp);
			}

			return n;
		}

		private List<MapNode> GetNeighbours(MapNode node)
		{
			var neighbours = new List<MapNode>();
			foreach (var neigh in PossibleNeighbours)
			{
				Predicate<MapNode> nodeFinder = (MapNode n) => n.X == node.X + neigh.X && n.Y == node.Y + neigh.Y;
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
