using HomeExam.Helpers;
using PG4500_2016_Exam2;
using Robocode.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeExam.SteeringBehavior
{
	public class ArrivalBehavior : SteeringBehaviour
	{
		private readonly double slowdownRadius = 100;

		public ArrivalBehavior(Trotor14MechaGodzilla robot, double slowdownRadius)
			: base(robot)
		{
			this.slowdownRadius = slowdownRadius;
		}

		public override void Steer(Vector2D targetPos)
		{
			Vector2D curPos = robot.Position;

			// Calculate the slowdownfactor based on the distance to the endtarget if we are inside the radius
			double dist = (targetPos - curPos).Length;
			double slowdownFactor = 1.0;
			if (dist < slowdownRadius)
			{
				slowdownFactor = dist / slowdownRadius;
			}
			Vector2D desiredVelocity = (Vector2D.Normalize(targetPos - curPos) * robot.CurrentSpeed) * slowdownFactor;

			ApplySteering(desiredVelocity, robot.VelocityVector);
		}
	}
}
