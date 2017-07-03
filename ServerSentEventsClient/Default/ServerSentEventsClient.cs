using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SSE {

	public class ServerSentEventsClient : IServerSentEventsClient {

		private readonly IEventStreamClient m_eventStreamClient;
		private readonly IEventStreamProcessor m_eventStreamProcessor;
		private CancellationTokenSource m_cts;

		public ServerSentEventsClient( string baseUri, string eventStreamPath ) {
			m_eventStreamClient = new EventStreamClient( new Uri( new Uri( baseUri ), eventStreamPath ) );
			m_eventStreamProcessor = new EventStreamProcessor( new MessageParser() );
		}

		internal ServerSentEventsClient(
			IEventStreamClient eventStreamClient,
			IEventStreamProcessor eventStreamProcessor
		) {
			m_eventStreamClient = eventStreamClient;
			m_eventStreamProcessor = eventStreamProcessor;
		}

		Action<HttpClient> IServerSentEventsClient.Configure { get; set; }

		Action<ServerSentEventsMessage> IServerSentEventsClient.OnMessage {
			get => m_eventStreamProcessor.OnMessage;
			set => m_eventStreamProcessor.OnMessage = value;
		}

		public Task<IServerSentEventsClient> Start() {
			IServerSentEventsClient @this = this;
			return @this.Start();
		}

		async Task<IServerSentEventsClient> IServerSentEventsClient.Start() {

			IServerSentEventsClient @this = this;

			@this.Configure?.Invoke( m_eventStreamClient.HttpClient );

			var stream = await m_eventStreamClient.StartAsync().ConfigureAwait( false );

			m_cts = new CancellationTokenSource();
#pragma warning disable 4014
			// Fire & forget
			m_eventStreamProcessor.ProcessAsync( stream, m_cts.Token );
#pragma warning restore 4014

			return this;
		}

		void IServerSentEventsClient.Stop() {
			IServerSentEventsClient @this = this;
			@this.Dispose();
		}

		void IDisposable.Dispose() {
			if( m_cts != null && !m_cts.IsCancellationRequested ) {
				m_cts.Cancel();
			}

			m_eventStreamClient?.Dispose();
		}

	}

}