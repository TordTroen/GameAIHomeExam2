using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeExam.Helpers;
using PG4500_2016_Exam2;

namespace HomeExam.SteeringBehavior
{
	public class SeekBehaviour : SteeringBehaviour
	{
		public SeekBehaviour(Trotor14MechaGodzilla robot)
			: base(robot)
		{
		}

		public override void Steer(Vector2D targetPos)
		{
			Vector2D curPos = robot.Position;

			Vector2D desiredVelocity = (Vector2D.Normalize(targetPos - curPos) * Trotor14MechaGodzilla.MaxSpeed);

			ApplySteering(desiredVelocity, robot.VelocityVector);
		}
	}
}
