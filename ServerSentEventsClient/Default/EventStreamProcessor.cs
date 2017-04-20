using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServerSentEventsClient.Default {

	internal class EventStreamProcessor : IEventStreamProcessor {

		private readonly Action<IServerSentEventsMessage> m_onMesssage;
		private string m_tail = string.Empty;

		public EventStreamProcessor( Action<IServerSentEventsMessage> onMesssage ) {
			m_onMesssage = onMesssage;
		}

		async Task IEventStreamProcessor.ProcessAsync( Stream eventStream ) {
			int bufferSize = 1024 * 64;
			byte[] buffer = new byte[bufferSize];
			while ( eventStream.CanRead ) {
				int bytesRead = await eventStream.ReadAsync( buffer, 0, bufferSize ).ConfigureAwait( false );
				ParseServerMessages( Encoding.UTF8.GetString( buffer, 0, bytesRead ) );
			}
		}

		private void ParseServerMessages( string rawData ) {
			int pos = 0;
			string message = string.Empty;
			string dataToParse = m_tail + rawData;

			while ( ( pos = dataToParse.IndexOf( '\n', pos ) ) >= 0 ) {
				message = dataToParse.Substring( 0, pos );
				ProcessMessage( message );
				m_tail = dataToParse.Substring( pos + 1 );
			}
		}

		private void ProcessMessage( string message ) {

			IServerSentEventsMessage currentMessage = null;

			m_onMesssage( currentMessage );
		}

	}

}