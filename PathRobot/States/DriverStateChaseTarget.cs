using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeExam.States
{
	public class DriverStateChaseTarget : State
	{
		private ArrivalBehavior arrival;

		public override void OnStart()
		{
			arrival = new ArrivalBehavior(robot, 150);
		}


		public override string OnUpdate()
		{
			string ret = base.OnUpdate();

			arrival.Steer(robot.TargetNode.GetPhysicalPosition());

			return ret;
		}
	}
}
