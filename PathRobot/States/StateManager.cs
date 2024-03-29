﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeExam.States;
using PG4500_2016_Exam2;

namespace HomeExam.States
{
	/// <summary>
	/// Holds all states, so we only have one instance of each class.
	/// </summary>
	public class StateManager
	{
		public const string StateIdle = "Idle";
		public const string StateRadarLock = "ScanLock";
		public const string StateRadarSweep = "RadarSweep";
		public const string StateChaseTarget = "ChaseTarget";
		public const string StateTurnToTarget = "TurnToTarget";

		private readonly Dictionary<string, State> states;
		private readonly Trotor14MechaGodzilla robot;

		public StateManager(Trotor14MechaGodzilla robot)
		{
			this.robot = robot;

			// Add all the states to the dictionary and initialize them
			states = new Dictionary<string, State>
			{
				{ StateIdle, new StateIdle() },
				{ StateRadarSweep, new RadarStateScanSweep() },
				{ StateRadarLock, new RadarStateScanLock() },
				{ StateChaseTarget, new DriverStateChaseTarget() },
				{ StateTurnToTarget, new DriverStateTurnToTarget() }
			};
			// Initialize the states with the dictionary entry's key and a reference to the robot
			foreach (var item in states)
			{
				item.Value.Initialize(item.Key, robot);
			}
		}

		/// <summary>
		/// Returns a state with the specified stateID.
		/// </summary>
		public State GetState(string stateId)
		{
			State state = null;
			if (HasState(stateId))
			{
				state = states[stateId];
			}
			else
			{
				robot.Out.WriteLine(string.Format("Couldn't find the state '{0}'", stateId));	
			}
			return state;
		}

		/// <summary>
		/// Checks if the specified state exists.
		/// </summary>
		public bool HasState(string stateId)
		{
			return states.ContainsKey(stateId);
		}
	}
}
