using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Robocode.Util;

namespace HomeExam.Helpers
{
	public class Vector2D
	{
		public double X { get; set; }
		public double Y { get; set; }

		public double Length { get { return Math.Sqrt((X * X) + (Y * Y)); } }

		public Vector2D()
		{
			Set(0.0, 0.0);
		}

		public Vector2D(double x, double y)
		{
			Set(x, y);
		}

		public Vector2D(Vector2D clone)
		{
			Set(clone);
		}

		public void Set(double x, double y)
		{
			X = x;
			Y = y;
		}

		public void Set(Vector2D v)
		{
			X = v.X;
			Y = v.Y;
		}

		public double Dot(Vector2D v)
		{
			double dot = 0;
			dot = X * v.X + Y * v.Y;
			//double angle = 0;
			//dot = Length * v.Length * Math.Cos(angle);
			return dot;
		}

		public double Distance(Vector2D v)
		{
			double dx = v.X - X;
			double dy = v.Y - Y;
			return Math.Sqrt((dx * dx) + (dy * dy));
		}

		public Vector2D ProjectForTime(double headingRadians, double velocity, double time)
		{
			return new Vector2D(
				X + (Math.Sin(headingRadians) * velocity * time),
				Y + (Math.Cos(headingRadians) * velocity * time));
		}

		public void Clamp(double xMin, double yMin, double xMax, double yMax)
		{
			if (X < xMin) X = xMin;
			else if (X > xMax) X = xMax;
			else if (Y < yMin) Y = yMin;
			else if (Y > yMax) Y = yMax;
		}

		public void Truncate(double max)
		{
			if (Length <= max) return;

			Normalize();
			X *= max;
			Y *= max;
		}

		public void Normalize()
		{
			double length = Length;
			if (length.IsZero()) return;

			X /= length;
			Y /= length;
		}

		/// <summary>
		/// Returns the angle between two vectors in relation to the coordsystem we can use to turn the robot.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="heading"></param>
		/// <returns></returns>
		public static double RotationAngleFromVectors(Vector2D a, Vector2D b, double heading)
		{
			double absDeg = AbsoluteDegrees(a, b);
			return Utils.NormalRelativeAngleDegrees(absDeg - heading);
		}

		// TODO Move from here, it doesn't really make sense to have it here
		public static double AbsoluteDegrees(Vector2D a, Vector2D b)
		{
			return Utils.ToDegrees(Math.Atan2(b.X - a.X, b.Y - a.Y));
		}

		public static Vector2D Normalize(Vector2D v)
		{
			Vector2D u = new Vector2D(v.X, v.Y);
			u.Normalize();
			return u;
		}

		public static Vector2D operator+(Vector2D v, Vector2D u)
		{
			return new Vector2D(v.X + u.X, v.Y + u.Y);
		}

		public static Vector2D operator-(Vector2D v, Vector2D u)
		{
			return new Vector2D(v.X - u.X, v.Y - u.Y);
		}

		public static Vector2D operator*(Vector2D v, double s)
		{
			return new Vector2D(v.X * s, v.Y * s);
		}

		public static Vector2D operator /(Vector2D v, double s)
		{
			return v * (1/s);
		}

		public static bool operator ==(Vector2D v, Vector2D u)
		{
			if (ReferenceEquals(v, u))
			{
				return true;
			}
			if (((object)v == null) || ((object)u == null))
			{
				return false;
			}
			return Utils.IsNear(v.X, u.X) && Utils.IsNear(v.Y, u.Y);
		}
		public static bool operator !=(Vector2D v, Vector2D u)
		{
			return !(v == u);
		}

		public override string ToString()
		{
			return string.Format("[{0}, {1}]", X, Y);
		}
	}
}
