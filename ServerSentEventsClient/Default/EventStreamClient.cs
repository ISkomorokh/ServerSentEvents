using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SSE {

	internal class EventStreamClient : IEventStreamClient {

		private readonly Uri m_eventStreamPath;

		private readonly HttpClient m_httpClient;

		public EventStreamClient( Uri eventStreamPath ) {
			m_eventStreamPath = eventStreamPath;
			m_httpClient = new HttpClient {
				Timeout = Timeout.InfiniteTimeSpan
			};
		}

		HttpClient IEventStreamClient.HttpClient => m_httpClient;

		Task<Stream> IEventStreamClient.StartAsync() {
			return m_httpClient.GetStreamAsync( m_eventStreamPath );
		}

		void IDisposable.Dispose() {
			if( m_httpClient == null ) {
				return;
			}

			m_httpClient.CancelPendingRequests();
			m_httpClient.Dispose();
		}

	}

}