using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SSE.UnitTests {

	[TestFixture]
	internal sealed class ServerSentEventsClientTests {

		private IServerSentEventsClient m_sut;
		private Mock<IEventStreamClient> m_mockedEventStreamClient;
		private Mock<IEventStreamProcessor> m_mockedEventStreamProcessor;
		private MockRepository m_mockRepository;

		[SetUp]
		public void SetUp() {
			m_mockRepository = new MockRepository( MockBehavior.Strict );
			m_mockedEventStreamClient = m_mockRepository.Create<IEventStreamClient>();
			m_mockedEventStreamProcessor = m_mockRepository.Create<IEventStreamProcessor>();

			m_sut = new ServerSentEventsClient(
				m_mockedEventStreamClient.Object,
				m_mockedEventStreamProcessor.Object
			);
		}

		[Test]
		public void Start_WhenEventStreamClientFailedToStart_Throws() {
			m_mockedEventStreamClient.Setup( c => c.StartAsync() ).ThrowsAsync( new Exception() );

			m_sut.Start();

			m_mockRepository.VerifyAll();
		}

		[Test]
		public async Task Start_WhenEventStreamClientStartedSuccessfully_ReturnsSelf() {
			var stream = new MemoryStream();

			m_mockedEventStreamClient.Setup( c => c.StartAsync() ).ReturnsAsync( stream );
			m_mockedEventStreamProcessor.Setup( p => p.ProcessAsync( stream, It.IsAny<CancellationToken>() ) ).Returns( Task.CompletedTask );

			IServerSentEventsClient result = await m_sut.Start();

			Assert.That( result, Is.EqualTo( m_sut ) );
			m_mockRepository.VerifyAll();
		}

		[Test]
		public void Dispose_CallsEventStreamClientDispose() {
			m_mockedEventStreamClient.Setup( c => c.Dispose() );

			m_sut.Dispose();

			m_mockRepository.VerifyAll();
		}

		[Test]
		public void Stop_CallsEventStreamClientDispose() {
			m_mockedEventStreamClient.Setup( c => c.Dispose() );

			m_sut.Stop();

			m_mockRepository.VerifyAll();
		}

	}

}