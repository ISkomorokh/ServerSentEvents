﻿namespace SSE {

	// TODO: Make it immutable
	public class ServerSentEventsMessage {

		public string Id { get; set; }
		public string Event { get; set; }
		public string Data { get; set; }

	}

}