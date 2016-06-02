using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeExam
{
	public class MapNodeComparer : Comparer<MapNode>
	{
		public override int Compare(MapNode x, MapNode y)
		{
			//if (x == y)
			//{
			//	return 0;
			//}
			return Comparer<double>.Default.Compare(x.F, y.F);
		}
	}
}
