using System;
using System.IO;
using System.Threading.Tasks;

namespace ServerSentEventsClient {

	internal interface IEventStreamClient : IDisposable {

		Task<Stream> StartAsync();

	}

}