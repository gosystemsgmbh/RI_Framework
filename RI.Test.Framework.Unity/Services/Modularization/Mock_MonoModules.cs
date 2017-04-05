using RI.Framework.Composition.Model;
using RI.Framework.Services.Modularization;

namespace RI.Test.Framework.Cases.Services.Modularization
{
	[Export]
	public sealed class Mock_ModuleA : MonoModule
	{
	}

	[Export]
	public sealed class Mock_ModuleB : MonoModule
	{
	}
}