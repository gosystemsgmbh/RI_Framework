using System;
using System.Collections.Generic;
using System.Reflection;

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

		private void RunTests ()
		{
			this.Log(LogLevel.Debug, "Searching for available tests");

			Bootstrapper bootstrapper = ServiceLocator.GetInstance<Bootstrapper>();

			int totalTestMethods = 0;
			Dictionary<TestModule, List<MethodInfo>> testMethods = new Dictionary<TestModule, List<MethodInfo>>();
			List<TestModule> testModules = bootstrapper.Container.GetExports<TestModule>();

			foreach (TestModule testModule in testModules)
			{
				List<MethodInfo> testMethod = testModule.GetTestMethods();
				testMethods.Add(testModule, testMethod);
				totalTestMethods += testMethod.Count;
			}

			this.Log(LogLevel.Debug, "Found total {0} test modules and {1} test methods", testModules.Count, totalTestMethods);
			this.Log(LogLevel.Debug, "Beginning running tests");

			int processedTestModules = 0;
			int processedTestMethods = 0;
			bool failed = false;

			foreach (KeyValuePair<TestModule, List<MethodInfo>> test in testMethods)
			{
				TestModule testModule = test.Key;
				foreach (MethodInfo testMethod in test.Value)
				{
					this.Log(LogLevel.Debug, "------------------------------");
					this.Log(LogLevel.Debug, "Test: {0}.{1}", testModule.GetType().Name, testMethod.Name);

					processedTestMethods++;

					try
					{
						testMethod.Invoke(testModule, null);

						this.Log(LogLevel.Debug, "Test succeeded");
					}
					catch (Exception exception)
					{
						this.Log(LogLevel.Error, "Test failed: {0}", exception.ToDetailedString());

						failed = true;
						break;
					}
				}

				processedTestModules++;

				if (failed)
				{
					break;
				}
			}

			this.Log(LogLevel.Debug, "------------------------------");
			this.Log(LogLevel.Debug, "Finished running tests");
			this.Log(LogLevel.Debug, "Processed {0}/{1} test modules and {2}/{3} test methods", processedTestModules, testModules.Count, processedTestMethods, totalTestMethods);
		}

		private void Start ()
		{
			ServiceLocator.GetInstance<IDispatcherService>().Dispatch(DispatcherPriority.Frame, this.RunTests);
		}

		#endregion
	}
}
