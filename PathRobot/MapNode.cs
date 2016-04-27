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
		public bool IsObstacle { get; private set; }

		public MapNode(int x, int y, bool isObstacle)
		{
			X = x;
			Y = y;
			IsObstacle = isObstacle;
		}

		public Vector2D GetPhysicalPosition()
		{
			return new Vector2D(X * CollisionMap.NodeSize, Y * CollisionMap.NodeSize);
		}
	}
}
