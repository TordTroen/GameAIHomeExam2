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
		private SeekBehaviour seek;

		public override void OnStart()
		{
			seek = new SeekBehaviour(robot);
		}

		public override string OnUpdate()
		{
			string ret = base.OnUpdate();

			//seek.Steer(robot.TargetNode.PhysicalPosition);
			if (robot.TargetPosition != null)
			{
				//Vector2D targetPosition = robot.TargetNode.PhysicalPosition;
				//if (robot.NextTargetNode != null)
				//{
				//	targetPosition = (robot.TargetNode.PhysicalPosition + robot.NextTargetNode.PhysicalPosition) * 0.5;
				//}
				//seek.Steer(targetPosition);
				seek.Steer(robot.TargetPosition);

				//Vector2D targetPos = robot.TargetNode.PhysicalPosition;

				//double angle = Vector2D.RotationAngleFromVectors(robot.Position, targetPos, robot.Heading);

				//robot.SetAhead(4);
				//robot.SetTurnRight(angle * 1000);
			}

			return ret;
		}
	}
}
