﻿using System;
using System.Collections.Generic;

using RI.Framework.StateMachines.States;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Contains all information about a transition.
	/// </summary>
	public sealed class StateTransientInfo
	{
		#region Instance Constructor/Destructor

		internal StateTransientInfo (StateMachine stateMachine)
		{
			if (stateMachine == null)
			{
				throw new ArgumentNullException(nameof(stateMachine));
			}

			this.StateMachine = stateMachine;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the next state.
		/// </summary>
		/// <value>
		///     The next state.
		/// </value>
		public IState NextState { get; internal set; }

		/// <summary>
		///     Gets the list of transition parameters.
		/// </summary>
		/// <value>
		///     The list of transition parameters.
		///     An empty list is returned if no transition parameters were specified.
		/// </value>
		/// <remarks>
		///     <note type="important">
		///         Note that the leaving state gets a chance to modify the parameter list before the entering state sees it.
		///     </note>
		/// </remarks>
		public List<object> Parameters { get; internal set; }

		/// <summary>
		///     Gets the previous state.
		/// </summary>
		/// <value>
		///     The previous state.
		/// </value>
		public IState PreviousState { get; internal set; }

		/// <summary>
		///     Gets the state machine associated with the transition.
		/// </summary>
		/// <value>
		///     The state machine associated with the transition.
		/// </value>
		public StateMachine StateMachine { get; }

		#endregion
	}
}
