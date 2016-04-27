using System.Collections.Generic;
using PG4500_2016_Exam2;

namespace HomeExam.States
{
	public class FiniteStateMachine
	{
		private const int MaxTransitionsPerFrame = 10;
		public string CurrentStateID { get { return curState.Id; } }
		private State curState;
		private Queue<State> stateQueue = new Queue<State>();
		private readonly StateManager states;
		private readonly Trotor14_MechaGodzilla robot;
		private string name = "State Machine";

		public FiniteStateMachine(string name, Trotor14_MechaGodzilla robot)
		{
			this.name = name;
			this.robot = robot;
			states = new StateManager(robot);

			// Start in the idle state
			SetCurrentState(states.GetState(StateManager.StateIdle));
		}

		/// <summary>
		/// Enqueues a state with the gives stateID. 
		/// Checks to make sure stateID isn't null or the same as the current state.
		/// </summary>
		/// <param name="stateId">The State to enqueue.</param>
		public void EnqueueState(string stateId)
		{
			if (stateId != null && states.HasState(stateId) && curState != states.GetState(stateId))
			{
				stateQueue.Enqueue(states.GetState(stateId));
			}
		}

		public void Update()
		{
			int processCount = 0;

			do
			{
				// Make sure we don't spin outta control
				processCount++;
				if (processCount > MaxTransitionsPerFrame)
				{
					break;
				}

				if (stateQueue.Count > 0)
				{
					SetCurrentState(stateQueue.Dequeue());
				}

				string queuedState = curState.OnUpdate();
				EnqueueState(queuedState);

			} while (stateQueue.Count > 0);
		}

		/// <summary>
		/// Sets the current state and calls the appropriate functions on the states.
		/// </summary>
		private void SetCurrentState(State newState)
		{
			string transitionText = name + ": ";

			if (curState != null)
			{
				curState.OnExit();
				transitionText += curState.Id + " -> ";
			}

			curState = newState;

			if (curState != null)
			{
				curState.OnEnter();
				transitionText += curState.Id;
			}

			robot.Out.WriteLine(transitionText);
		}
	}
}
