using System;

namespace ServerSentEventsClient {

	public interface IServerSentEventsClient: IDisposable {

		IServerSentEventsClient Start();

		void Stop();

	}

}