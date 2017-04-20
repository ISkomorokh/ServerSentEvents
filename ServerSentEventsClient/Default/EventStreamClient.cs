using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServerSentEventsClient.Default {

	internal class EventStreamClient : IEventStreamClient, IDisposable {

		private readonly Uri m_eventStreamPath;

		private readonly HttpClient m_httpClient;

		public EventStreamClient( Uri eventStreamPath ) {
			m_eventStreamPath = eventStreamPath;
			m_httpClient = new HttpClient();
		}

		Task<Stream> IEventStreamClient.StartAsync() {
			return m_httpClient.GetStreamAsync( m_eventStreamPath );
		}

		public void Dispose() {
			m_httpClient?.Dispose();
		}

	}

}