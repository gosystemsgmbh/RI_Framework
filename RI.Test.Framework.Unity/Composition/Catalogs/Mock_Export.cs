#if PLATFORM_UNITY
#else
using RI.Framework.Composition;
using RI.Framework.Composition.Model;



namespace RI.Test.Framework.Composition.Catalogs
{
	[Export("Mock_Export")]
	public sealed class Mock_Export : ComposableMonoBehaviour
	{
	}
}
#endif
