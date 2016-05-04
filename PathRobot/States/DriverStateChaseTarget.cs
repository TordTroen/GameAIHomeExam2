using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeExam.SteeringBehavior;
using HomeExam.Helpers;

namespace HomeExam.States
{
	public class DriverStateChaseTarget : State
	{
		private ArrivalBehavior seek;

		public override void OnStart()
		{
			seek = new ArrivalBehavior(robot, 0);
		}


		public override string OnUpdate()
		{
			string ret = base.OnUpdate();

			//seek.Steer(robot.TargetNode.PhysicalPosition);
			if (robot.TargetNode != null)
			{
				seek.Steer(robot.TargetNode.PhysicalPosition);

				//Vector2D targetPos = robot.TargetNode.PhysicalPosition;

				//double angle = Vector2D.RotationAngleFromVectors(robot.Position, targetPos, robot.Heading);

				//robot.SetAhead(4);
				//robot.SetTurnRight(angle * 1000);
			}

			return ret;
		}
	}
}
