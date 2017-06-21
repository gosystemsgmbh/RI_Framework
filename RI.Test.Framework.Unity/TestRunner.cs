using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition;
using RI.Framework.Composition.Catalogs;
using RI.Framework.Services;
using RI.Framework.Services.Dispatcher;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;

using UnityEngine;




namespace RI.Test.Framework
{
	public sealed class TestRunner : MonoBehaviour, ILogSource
	{
		#region Instance Fields

		[TextArea(10, 10)]
		public string ExcludedMethods = string.Empty;

		[TextArea(10, 10)]
		public string IncludedMethods = string.Empty;

		#endregion




		#region Instance Properties/Indexer

		private Exception Exception { get; set; }

		private HashSet<string> ExcludedMethodList { get; set; }

		private HashSet<string> IncludedMethodList { get; set; }

		private int ProcessedTestMethods { get; set; }

		private string Progress { get; set; }

		private List<object[]> TestMethods { get; set; }

		#endregion




		#region Instance Methods

		private void ContinueTests ()
		{
			if (this.ProcessedTestMethods >= this.TestMethods.Count)
			{
				ServiceLocator.GetInstance<DispatcherService>().Dispatch(DispatcherPriority.Idle, this.StopTests);
				return;
			}

			ServiceLocator.GetInstance<DispatcherService>().Dispatch(DispatcherPriority.Idle, () =>
			{
				TestModule testModule = (TestModule)this.TestMethods[this.ProcessedTestMethods][0];
				MethodInfo testMethod = (MethodInfo)this.TestMethods[this.ProcessedTestMethods][1];

				this.Log(LogLevel.Debug, "------------------------------");
				this.Log(LogLevel.Debug, "Test: {0}.{1} @ {2}", testMethod.DeclaringType.Name, testMethod.Name, testModule.GetType().Name);

				string testName = this.GetTestName(testMethod);

				this.ProcessedTestMethods++;

				this.Progress = this.ProcessedTestMethods.ToString() + " / " + this.TestMethods.Count.ToString() + Environment.NewLine + testName;

				if (this.ExcludedMethodList.Contains(testName))
				{
					this.Log(LogLevel.Warning, "Test excluded");

					this.ContinueTests();
				}
				else
				{
					ServiceLocator.GetInstance<DispatcherService>().Dispatch(DispatcherPriority.Idle, (_testModule, _testMethod) =>
					{
						try
						{
							_testModule.InvokeTestMethod(_testMethod, new Action(this.ContinueTests));

							this.Log(LogLevel.Debug, "Test succeeded");
						}
						catch (Exception exception)
						{
							this.Log(LogLevel.Error, "Test failed: {0}", exception.ToDetailedString());

							if (this.Exception == null)
							{
								this.Exception = exception;
							}
						}
					}, testModule, testMethod);
				}
			});
		}

		private string GetTestName (MethodInfo testMethod)
		{
			return testMethod.DeclaringType.Name + "." + testMethod.Name;
		}

		private void OnGUI ()
		{
			string text = this.Exception?.ToString() ?? this.Progress ?? string.Empty;
			GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text);
		}

		private void Start ()
		{
			ServiceLocator.GetInstance<DispatcherService>().Dispatch(DispatcherPriority.Idle, this.StartTests);
		}

		private void StartTests ()
		{
			this.Progress = null;
			this.Exception = null;

			this.ExcludedMethodList = new HashSet<string>(this.ExcludedMethods.SplitLines(), StringComparerEx.InvariantCultureIgnoreCase);
			this.IncludedMethodList = new HashSet<string>(this.IncludedMethods.SplitLines().Where(x => !x.IsNullOrEmptyOrWhitespace()), StringComparerEx.InvariantCultureIgnoreCase);

			this.Log(LogLevel.Debug, "Excluded tests: {0}", this.ExcludedMethodList.Join("; "));
			this.Log(LogLevel.Debug, "Included tests: {0}", this.IncludedMethodList.Join("; "));

			this.Log(LogLevel.Debug, "Searching for available tests");

			ServiceLocator.GetInstance<CompositionContainer>().AddCatalog(new AssemblyCatalog(false, Assembly.GetExecutingAssembly()));

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

			this.IncludedMethodList.RemoveWhere(x => x.IsNullOrEmptyOrWhitespace());
			if (this.IncludedMethodList.Count > 0)
			{
				this.TestMethods.RemoveWhere(x => !this.IncludedMethodList.Contains(this.GetTestName((MethodInfo)x[1])));
			}

			this.Log(LogLevel.Debug, "Found total {0} test methods", this.TestMethods.Count);
			this.Log(LogLevel.Debug, "Beginning run tests");

			this.ContinueTests();
		}

		private void StopTests ()
		{
			this.Log(LogLevel.Debug, "------------------------------");
			this.Log(LogLevel.Debug, "Finished running tests");
			this.Log(LogLevel.Debug, "Processed {0}/{1} test methods", this.ProcessedTestMethods, this.TestMethods.Count);

			ServiceLocator.GetInstance<Bootstrapper>().Shutdown();
		}

		#endregion
	}
}
