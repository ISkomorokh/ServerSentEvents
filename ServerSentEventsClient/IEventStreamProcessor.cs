using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSentEventsClient {

	internal interface IEventStreamProcessor {

		Action<IServerSentEventsMessage> OnMessage { get; set; }

		Task ProcessAsync( Stream eventStream, CancellationToken cancellationToken );

	}

}