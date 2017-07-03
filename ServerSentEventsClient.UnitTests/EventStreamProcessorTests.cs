using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SSE.UnitTests {

	[TestFixture]
	internal sealed class EventStreamProcessorTests {

		private IEventStreamProcessor m_sut;

		private static Stream GetStream( string source ) {
			return new MemoryStream( Encoding.UTF8.GetBytes( source ) );
		}

		[SetUp]
		public void Setup() {
			m_sut = new EventStreamProcessor( new MessageParser() );
		}

		[Test]
		public void ProcessAsync_NoOnMessageHandler_ShoudntThrow() {
			Assert.DoesNotThrowAsync( async () => await m_sut.ProcessAsync( new MemoryStream(), CancellationToken.None ) );
		}

		[Test]
		public async Task ProcessAsync_OnMessageHandler_ExecutedOnce() {
			int executionCount = 0;
			ServerSentEventsMessage actrualMessage = null;

			m_sut.OnMessage = message => {
				executionCount++;
				actrualMessage = message;
			};

			string data = "data: Hello, World!\r\n\r\n";

			using ( var stream = GetStream( data ) ) {
				await m_sut.ProcessAsync( stream, CancellationToken.None );
			}

			Assert.That( executionCount, Is.EqualTo( 1 ) );
			actrualMessage.ShouldBeEquivalentTo( new ServerSentEventsMessage { Data = "Hello, World!" } );
		}

		[Test]
		public async Task ProcessAsync_OnMessageHandler_ExecutedTwice() {
			var actual = new List<ServerSentEventsMessage>();

			m_sut.OnMessage = message => {
				actual.Add( message );
			};

			string data = "data: Hello, World!\r\n\r\ndata: Good bye!\r\n\r\n";

			using ( var stream = GetStream( data ) ) {
				await m_sut.ProcessAsync( stream, CancellationToken.None );
			}

			var expected = new List<ServerSentEventsMessage> {
				new ServerSentEventsMessage { Data = "Hello, World!" },
				new ServerSentEventsMessage { Data = "Good bye!" }
			};
			actual.ShouldBeEquivalentTo( expected, config => config.WithStrictOrdering() );
		}

	}

}