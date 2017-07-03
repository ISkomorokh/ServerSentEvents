using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SSE {

	public interface IServerSentEventsClient : IDisposable {

		Action<HttpClient> Configure { get; set; }

		Action<ServerSentEventsMessage> OnMessage { get; set; }

		Task<IServerSentEventsClient> Start();

		void Stop();

	}

}