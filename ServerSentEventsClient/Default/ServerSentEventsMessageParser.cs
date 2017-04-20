using System;
using System.Collections.Generic;
using System.Text;

namespace ServerSentEventsClient.Default {

	// https://html.spec.whatwg.org/multipage/comms.html#sse-processing-model
	internal class ServerSentEventsMessageParser : IServerSentEventsMessageParser {

		private static byte ByteLF = 10;
		private static byte ByteCR = 13;
		private IServerSentEventsMessage m_parsedMessage;

		private readonly StringBuilder m_lastChunk = new StringBuilder(); // TODO: Specify size

		public IEnumerable<IServerSentEventsMessage> Parse( ArraySegment<byte> buffer ) {

			int lineStart = buffer.Offset;
			int currentPosition = lineStart;
			int lineLength = 0;

			for ( ; currentPosition < buffer.Count; currentPosition++ ) {

				if( !IsCarrierReturnSymbol( buffer.Array[currentPosition] ) ) {
					continue;
				}

				if( currentPosition < buffer.Count - 1 &&
					buffer.Array[currentPosition] == ByteCR &&
					buffer.Array[currentPosition + 1] == ByteLF
				) {
					currentPosition++;
				}

				lineLength = currentPosition + 1 - lineStart;
				if( lineLength <= 2 && m_parsedMessage != null ) {
					yield return m_parsedMessage;
					m_parsedMessage = null;
				} else {
					ParseLine( Encoding.UTF8.GetString( buffer.Array, lineStart, lineLength ) );
				}

				lineStart = currentPosition + 1;
			}

			if( lineLength < buffer.Count && currentPosition != lineStart ) {
				m_lastChunk.Append( Encoding.UTF8.GetString( buffer.Array, lineStart, currentPosition - lineStart ) );
			}
		}

		private static bool IsCarrierReturnSymbol( byte symbol ) {
			return symbol == ByteLF || symbol == ByteCR;
		}

		private void ParseLine( string line ) {

			if( m_parsedMessage == null ) {
				m_parsedMessage = new ServerSentEventsMessage();
			}

			if( m_lastChunk.Length > 0 ) {
				line = m_lastChunk.Append( line ).ToString();
				m_lastChunk.Clear();
			}

			if( line.StartsWith( ":", StringComparison.OrdinalIgnoreCase ) ) {
				return;
			}

			if( line.StartsWith( "event:", StringComparison.OrdinalIgnoreCase ) ) {
				m_parsedMessage.Event = line.Substring( 6 ).Trim();
			} else if( line.StartsWith( "id:", StringComparison.OrdinalIgnoreCase ) ) {
				m_parsedMessage.Id = line.Substring( 3 ).Trim();
			} else if( line.StartsWith( "data:", StringComparison.OrdinalIgnoreCase ) ) {
				m_parsedMessage.Data += line.Substring( 5 ).Trim(); // ? + '\n'; // TODO: Use StringBuilder and remove last \n
			}
		}

	}

}