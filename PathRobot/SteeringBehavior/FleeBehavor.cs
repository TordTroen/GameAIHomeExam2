using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeExam.Helpers;
using Robocode.Util;
using PG4500_2016_Exam2;

namespace HomeExam
{
	public class FleeBehavior : SteeringBehaviour
	{
		public FleeBehavior(Trotor14_MechaGodzilla robot)
			: base(robot)
		{
			
		}

		public override void Steer(Vector2D targetPos)
		{
			Vector2D desiredVelocity = Vector2D.Normalize(robot.Position - targetPos) * Trotor14_MechaGodzilla.MaxSpeed;
			ApplySteering(desiredVelocity, robot.VelocityVector);
		}
	}
}
