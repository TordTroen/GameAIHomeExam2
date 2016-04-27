using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeExam.States
{
	public class RadarStateScanSweep : State
	{
		public override void OnEnter()
		{
			robot.SetTurnRadarRight(1000);
		}
	}
}
