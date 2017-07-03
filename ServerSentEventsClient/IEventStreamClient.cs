using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SSE {

	internal interface IEventStreamClient : IDisposable {

		HttpClient HttpClient { get; }

		Task<Stream> StartAsync();

	}

}