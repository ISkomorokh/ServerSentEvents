using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SSE.UnitTests {

	[TestFixture]
	internal sealed class ServerSentEventsClientTests {

		private IServerSentEventsClient m_sut;
		private Mock<IEventStreamClient> m_mockedEventStreamClient;
		private Mock<IEventStreamProcessor> m_mockedEventStreamProcessor;
		private Mock<ILoggerFactory> m_mockedLoggerFactory;
		private MockRepository m_mockRepository;

		[SetUp]
		public void SetUp() {
			m_mockRepository = new MockRepository( MockBehavior.Strict );
			m_mockedEventStreamClient = m_mockRepository.Create<IEventStreamClient>();
			m_mockedEventStreamProcessor = m_mockRepository.Create<IEventStreamProcessor>();
			m_mockedLoggerFactory = new Mock<ILoggerFactory>();

			m_mockedLoggerFactory.Setup( f => f.CreateLogger( It.IsAny<string>() ) ).Returns( Mock.Of<ILogger>() );

			m_sut = new ServerSentEventsClient(
				m_mockedEventStreamClient.Object,
				m_mockedEventStreamProcessor.Object,
				m_mockedLoggerFactory.Object
			);
		}

		[Test]
		public void Start_WhenEventStreamClientFailedToStart_Throws() {
			m_mockedEventStreamClient.Setup( c => c.StartAsync() ).ThrowsAsync( new Exception() );
			m_mockedEventStreamProcessor.SetupSet( p => p.OnMessage = It.IsAny<Action<ServerSentEventsMessage>>() );

			m_sut.Start();

			m_mockRepository.VerifyAll();
		}

		[Test]
		public async Task Start_WhenEventStreamClientStartedSuccessfully_ReturnsSelf() {
			var stream = new MemoryStream();

			m_mockedEventStreamClient.Setup( c => c.StartAsync() ).ReturnsAsync( stream );
			m_mockedEventStreamProcessor.Setup( p => p.ProcessAsync( stream, It.IsAny<CancellationToken>() ) )
				.Returns( Task.CompletedTask );
			m_mockedEventStreamProcessor.SetupSet( p => p.OnMessage = It.IsAny<Action<ServerSentEventsMessage>>() );

			IServerSentEventsClient result = await m_sut.Start();

			Assert.That( result, Is.EqualTo( m_sut ) );
			m_mockRepository.VerifyAll();
		}

		[Test]
		public void Dispose_CallsEventStreamClientDispose() {
			m_mockedEventStreamClient.Setup( c => c.Dispose() );
			m_mockedEventStreamProcessor.SetupSet( p => p.OnMessage = null );

			m_sut.Dispose();

			m_mockRepository.VerifyAll();
		}

		[Test]
		public void Stop_CallsEventStreamClientDispose() {
			m_mockedEventStreamClient.Setup( c => c.Dispose() );
			m_mockedEventStreamProcessor.SetupSet( p => p.OnMessage = null );

			m_sut.Stop();

			m_mockRepository.VerifyAll();
		}

	}

}