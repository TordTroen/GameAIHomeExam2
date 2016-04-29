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
	public class SteeringBehaviour
	{
		protected readonly Trotor14_MechaGodzilla robot;

		public SteeringBehaviour(Trotor14_MechaGodzilla robot)
		{
			this.robot = robot;
		}

		public virtual void Steer(Vector2D target) { }

		/// <summary>
		/// Applies the steeringforces to the robot based on the desiredVelocity and velocity spevified.
		/// </summary>
		protected void ApplySteering(Vector2D desiredVelocity, Vector2D velocity)
		{
			Vector2D curPos = robot.Position;

			Vector2D steering = desiredVelocity - velocity;

			steering.Truncate(Trotor14_MechaGodzilla.MaxSpeed);
			steering = steering / Trotor14_MechaGodzilla.Mass;

			velocity = velocity + steering;
			velocity.Truncate(Trotor14_MechaGodzilla.MaxSpeed);

			Vector2D pos = curPos + velocity;

			double angle = Vector2D.RotationAngleFromVectors(curPos, pos, robot.Heading);

			robot.SetAhead(desiredVelocity.Length);
			robot.SetTurnRight(angle);
		}
	}
}
