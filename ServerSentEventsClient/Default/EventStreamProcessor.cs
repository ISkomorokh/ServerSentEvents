using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSentEventsClient.Default {

	internal class EventStreamProcessor : IEventStreamProcessor {

		private readonly IServerSentEventsMessageParser m_messageParser;

		public EventStreamProcessor( IServerSentEventsMessageParser messageParser ) {
			m_messageParser = messageParser;
		}

		public Action<IServerSentEventsMessage> OnMessage { get; set; }

		async Task IEventStreamProcessor.ProcessAsync( Stream eventStream, CancellationToken cancellationToken ) {
			int bufferSize = 1024 * 64;
			byte[] buffer = new byte[bufferSize];
			bool endOfStream = false;

			while ( !endOfStream ) {
				int count = await eventStream.ReadAsync( buffer, 0, bufferSize, cancellationToken )
					.ConfigureAwait( false );
				foreach ( var message in m_messageParser.Parse( new ArraySegment<byte>( buffer, 0, count ) ) ) {
					PostMessage( message );
				}
				endOfStream = count == 0;
			}
		}

		private void PostMessage( IServerSentEventsMessage message ) {
			OnMessage?.Invoke( message );
		}

	}

}