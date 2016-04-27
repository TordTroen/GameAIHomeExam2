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

		public List<MapNode> map;
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
			int y = 0;
			for (int i = 0; i < simpleMap.Length; i ++)
			{
				int mod = (i + 1) % Width;
				if (mod == 0)
				{
					y++;
				}
				map.Add(new MapNode(i, x, y, simpleMap[i] == 1));
				//robot.Out.WriteLine(string.Format("[{0}, {1}]: {2}", x, y, simpleMap[i] == 1));
				//robot.Out.WriteLine(string.Format("[{0}, {1}]: {2} % {3} = {4}", x, y, i+1, Width, mod));
				x++;
			}

			foreach (var node in map)
			{
				node.Neighbours = GetNeighbours(node);
			}
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

		public void PaintMap(IGraphics graphics)
		{
			foreach (var node in map)
			{
				//graphics.DrawBox(node.IsObstacle ? Color.Red : Color.Blue, node.GetPhysicalPosition(), 127, (float)NodeSize, (float)NodeSize);
				Vector2D pos = node.GetPhysicalPosition();
				Color col = node.IsObstacle ? Color.Red : Color.Blue;
				graphics.FillRectangle(new SolidBrush(col), (int)(pos.X - (NodeSize / 2)), (int)(pos.Y - (NodeSize / 2)), (float)NodeSize, (float)NodeSize);
			}
		}
	}
}
