using NUnit.Framework;

namespace ServerSentEventsClient.UnitTests {

	[TestFixture]
	internal sealed class ServerSentEventsClientTests {

		private IServerSentEventsClient m_sut = new ServerSentEventsClient( "https://stream.launchdarkly.com/", "flags" );

		[Test]
		public void Test() {

		}
	}

}