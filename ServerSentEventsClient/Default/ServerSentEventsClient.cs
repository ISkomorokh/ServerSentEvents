using System;
using System.IO;
using System.Threading.Tasks;
using ServerSentEventsClient.Default;

namespace ServerSentEventsClient {

	public class ServerSentEventsClient : IServerSentEventsClient {

		private readonly string m_baseUri;
		private readonly string m_eventStreamPath;
		private readonly IEventStreamClient m_eventStreamClient;
		private readonly IEventStreamProcessor m_eventStreamProcessor;

		public ServerSentEventsClient( string baseUri, string eventStreamPath ) {
			m_baseUri = baseUri;
			m_eventStreamPath = eventStreamPath;
			//m_eventStreamProcessor = eventStreamProcessor;
			m_eventStreamClient = new EventStreamClient( new Uri( new Uri( baseUri ), eventStreamPath ) );
		}

		internal ServerSentEventsClient(
			string baseUri,
			string eventStreamPath,
			IEventStreamClient eventStreamClient,
			IEventStreamProcessor eventStreamProcessor ) {
			m_baseUri = baseUri;
			m_eventStreamPath = eventStreamPath;
			m_eventStreamClient = eventStreamClient;
			m_eventStreamProcessor = eventStreamProcessor;
		}

		async Task IServerSentEventsClient.Start() {
			var eventStream = await m_eventStreamClient.StartAsync().ConfigureAwait( false );
			m_eventStreamProcessor.ProcessAsync( eventStream );
		}

	}

}