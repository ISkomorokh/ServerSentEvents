using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SSE {

	public class ServerSentEventsClient : IServerSentEventsClient {

		private readonly IEventStreamClient m_eventStreamClient;
		private readonly IEventStreamProcessor m_eventStreamProcessor;

		private readonly ConcurrentDictionary<string, ImmutableList<Action<ServerSentEventsMessage>>> m_eventListeners =
			new ConcurrentDictionary<string, ImmutableList<Action<ServerSentEventsMessage>>>();

		private readonly ILogger m_logger;
		private CancellationTokenSource m_cts;

		public ServerSentEventsClient( string baseUri, string eventStreamPath, ILoggerFactory loggerFactory ) {
			m_eventStreamClient = new EventStreamClient( new Uri( new Uri( baseUri ), eventStreamPath ) );
			m_eventStreamProcessor = new EventStreamProcessor( new MessageParser() );
			m_logger = loggerFactory.CreateLogger<ServerSentEventsClient>();
		}

		internal ServerSentEventsClient(
			IEventStreamClient eventStreamClient,
			IEventStreamProcessor eventStreamProcessor,
			ILoggerFactory loggerFactory
		) {
			m_eventStreamClient = eventStreamClient;
			m_eventStreamProcessor = eventStreamProcessor;
			m_logger = loggerFactory.CreateLogger<ServerSentEventsClient>();
		}

		Action<HttpClient> IServerSentEventsClient.Configure { get; set; }

		Action<ServerSentEventsMessage> IServerSentEventsClient.OnMessage {
			get => m_eventStreamProcessor.OnMessage;
			set => m_eventStreamProcessor.OnMessage = value;
		}

		async Task<IServerSentEventsClient> IServerSentEventsClient.Start() {

			IServerSentEventsClient @this = this;

			using ( m_logger.BeginScope( "Stratring SSE client" ) ) {

				m_logger.LogInformation( "Configuring HttpClient" );
				@this.Configure?.Invoke( m_eventStreamClient.HttpClient );

				m_logger.LogInformation( "Connecting..." );

				Stream stream;
				try {
					stream = await m_eventStreamClient.StartAsync().ConfigureAwait( false );
				} catch ( Exception e ) {
					m_logger.LogError( 0, e, "Failed to connect" );
					throw;
				}

				m_cts = new CancellationTokenSource();
#pragma warning disable 4014
				// Fire & forget
				m_eventStreamProcessor.ProcessAsync( stream, m_cts.Token );
#pragma warning restore 4014
				m_logger.LogInformation( "Processing SSE stream has started" );
			}

			return this;
		}

		void IServerSentEventsClient.AddEventListener( string @event, Action<ServerSentEventsMessage> handler ) {
			if( string.IsNullOrWhiteSpace( @event ) ) {
				throw new ArgumentNullException( nameof(@event) );
			}
			if( handler == null ) {
				throw new ArgumentNullException( nameof(handler) );
			}

			m_eventListeners.AddOrUpdate(
				@event,
				key => ImmutableList<Action<ServerSentEventsMessage>>.Empty.Add( handler ),
				( key, list ) => list.Add( handler )
			);
		}

		void IServerSentEventsClient.RemoveEventListener( string @event, Action<ServerSentEventsMessage> handler ) {
			if( string.IsNullOrWhiteSpace( @event ) ) {
				throw new ArgumentNullException( nameof(@event) );
			}
			if( handler == null ) {
				throw new ArgumentNullException( nameof(handler) );
			}
			if( m_eventListeners.TryGetValue( @event, out var handlers ) ) {
				if( !m_eventListeners.TryUpdate( @event, handlers.Remove( handler ), handlers ) ) {
					m_logger.LogWarning( $"Can't remove event handler \"{@event}\"" );
				}
			}
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