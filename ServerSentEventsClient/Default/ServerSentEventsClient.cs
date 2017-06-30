using ServerSentEventsClient.Default;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSentEventsClient {

	public class ServerSentEventsClient : IServerSentEventsClient {

		private readonly IEventStreamClient m_eventStreamClient;
		private readonly IEventStreamProcessor m_eventStreamProcessor;
		private CancellationTokenSource m_cts;

		public ServerSentEventsClient( string baseUri, string eventStreamPath ) {
			m_eventStreamClient = new EventStreamClient( new Uri( new Uri( baseUri ), eventStreamPath ) );
			m_eventStreamProcessor = new EventStreamProcessor( new ServerSentEventsMessageParser() );
		}

		internal ServerSentEventsClient(
			IEventStreamClient eventStreamClient,
			IEventStreamProcessor eventStreamProcessor
		) {
			m_eventStreamClient = eventStreamClient;
			m_eventStreamProcessor = eventStreamProcessor;
		}

		IServerSentEventsClient IServerSentEventsClient.Start() {

			m_eventStreamClient.StartAsync()
				.ContinueWith(
					( task ) => {
						m_cts = new CancellationTokenSource();
						return m_eventStreamProcessor.ProcessAsync( task.Result, m_cts.Token );
					},
					TaskContinuationOptions.OnlyOnRanToCompletion
				);

			return this;
		}

		void IServerSentEventsClient.Stop() {
			m_eventStreamClient?.Dispose();
		}

		void IDisposable.Dispose() {
			if( m_cts != null && !m_cts.IsCancellationRequested ) {
				m_cts.Cancel();
			}

			m_eventStreamClient?.Dispose();
		}

	}

}