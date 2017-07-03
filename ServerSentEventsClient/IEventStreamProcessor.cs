using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SSE {

	internal interface IEventStreamProcessor {

		Action<ServerSentEventsMessage> OnMessage { get; set; }

		Task ProcessAsync( Stream eventStream, CancellationToken cancellationToken );

	}

}