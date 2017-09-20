using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SSE {

	public interface IServerSentEventsClient : IDisposable {

		Action<HttpClient> Configure { get; set; }

		Task<IServerSentEventsClient> Start();

		Action<ServerSentEventsMessage> OnMessage { get; set; }

		void AddEventListener( string @event, Action<ServerSentEventsMessage> handler );

		void RemoveEventListener( string @event, Action<ServerSentEventsMessage> handler );

		void Stop();

	}

}