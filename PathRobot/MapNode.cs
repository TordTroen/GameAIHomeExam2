using HomeExam.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeExam
{
	public class MapNode
	{
		public int X { get; private set; }
		public int Y { get; private set; }
		public List<MapNode> Neighbours { get; set; }
		public Vector2D PhysicalPosition { get; private set; }

		public double G { get; set; }
		public double H { get; set; }
		public double F { get; set; }
		public MapNode Parent { get; set; }
		public bool IsClosed { get; set; } = false;

		public MapNode(int x, int y)
		{
			Neighbours = new List<MapNode>();
			X = x;
			Y = y;
			PhysicalPosition = new Vector2D(X * CollisionMap.NodeSize, Y * CollisionMap.NodeSize) + new Vector2D(CollisionMap.NodeSize / 2, CollisionMap.NodeSize / 2);
		}

		public bool IsInsideMapBounds()
		{
			return X >= 0 && X < CollisionMap.Width && Y >= 0 && Y < CollisionMap.Height;
		}

		public override bool Equals(object obj)
		{
			return this == (MapNode)obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(MapNode a, MapNode b)
		{
			if (ReferenceEquals(a, b))
			{
				return true;
			}
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(MapNode a, MapNode b)
		{
			return !(a == b);
		}

		public override string ToString()
		{
			return string.Format("[{0}, {1}]", X, Y);
		}
	}
}
