using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeExam.SteeringBehavior;

namespace HomeExam.States
{
	public class DriverStateChaseTarget : State
	{
		private SeekBehaviour seek;

		public override void OnStart()
		{
			seek = new SeekBehaviour(robot);
		}


		public override string OnUpdate()
		{
			string ret = base.OnUpdate();

			//seek.Steer(robot.TargetNode.PhysicalPosition);
			if (robot.TargetNode != null)
			{
				seek.Steer(robot.TargetNode.PhysicalPosition);
			}

			return ret;
		}
	}
}
