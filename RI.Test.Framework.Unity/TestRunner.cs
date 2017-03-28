using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections.Linq;
using RI.Framework.Composition;
using RI.Framework.Composition.Catalogs;
using RI.Framework.Services;
using RI.Framework.Services.Dispatcher;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;

using UnityEngine;




namespace RI.Test.Framework
{
	public sealed class TestRunner : MonoBehaviour
	{
		#region Instance Methods

		private void Log (LogLevel severity, string format, params object[] args)
		{
			LogLocator.Log(severity, this.GetType().Name, format, args);
		}

		private int ProcessedTestMethods { get; set; }

		private List<object[]> TestMethods { get; set; }

		private void StartTests ()
		{
			this.Log(LogLevel.Debug, "Searching for available tests");

			ServiceLocator.GetInstance<CompositionContainer>().AddCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

			this.ProcessedTestMethods = 0;
			this.TestMethods = new List<object[]>();

			List<TestModule> testModules = ServiceLocator.GetInstances<TestModule>().ToList();
			foreach (TestModule testModule in testModules)
			{
				List<MethodInfo> testMethods = testModule.GetTestMethods();
				foreach (MethodInfo testMethod in testMethods)
				{
					this.TestMethods.Add(new object[] {testModule, testMethod});
				}
			}

			this.Log(LogLevel.Debug, "Found total {0} test methods", this.TestMethods.Count);
			this.Log(LogLevel.Debug, "Beginning run tests");

			this.ContinueTests();
		}

		private void ContinueTests ()
		{
			if (this.ProcessedTestMethods >= this.TestMethods.Count)
			{
				ServiceLocator.GetInstance<IDispatcherService>().Dispatch(DispatcherPriority.Idle, this.StopTests);
				return;
			}

			ServiceLocator.GetInstance<IDispatcherService>().Dispatch(DispatcherPriority.Idle, () =>
			{
				TestModule testModule = (TestModule)this.TestMethods[this.ProcessedTestMethods][0];
				MethodInfo testMethod = (MethodInfo)this.TestMethods[this.ProcessedTestMethods][1];

				this.Log(LogLevel.Debug, "------------------------------");
				this.Log(LogLevel.Debug, "Test: {0}.{1} @ {2}", testMethod.DeclaringType.Name, testMethod.Name, testModule.GetType().Name);

				this.ProcessedTestMethods++;

				try
				{
					testModule.InvokeTestMethod(testMethod, new Action(this.ContinueTests));

					this.Log(LogLevel.Debug, "Test succeeded");
				}
				catch (Exception exception)
				{
					this.Log(LogLevel.Error, "Test failed: {0}", exception.ToDetailedString());
				}
			});
		}

		private void StopTests ()
		{
			this.Log(LogLevel.Debug, "------------------------------");
			this.Log(LogLevel.Debug, "Finished running tests");
			this.Log(LogLevel.Debug, "Processed {0}/{1} test methods", this.ProcessedTestMethods, this.TestMethods.Count);

			ServiceLocator.GetInstance<Bootstrapper>().Shutdown();
		}

		private void Start ()
		{
			ServiceLocator.GetInstance<IDispatcherService>().Dispatch(DispatcherPriority.Idle, this.StartTests);
		}

		#endregion
	}
}
