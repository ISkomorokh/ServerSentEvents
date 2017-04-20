using System;
using System.Collections.Generic;

namespace ServerSentEventsClient {

	internal interface IServerSentEventsMessageParser {

		IEnumerable<IServerSentEventsMessage> Parse( ArraySegment<byte> buffer );

	}

}