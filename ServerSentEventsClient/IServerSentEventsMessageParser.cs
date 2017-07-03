using System;
using System.Collections.Generic;

namespace SSE {

	internal interface IServerSentEventsMessageParser {

		IEnumerable<ServerSentEventsMessage> Parse( ArraySegment<byte> buffer );

	}

}