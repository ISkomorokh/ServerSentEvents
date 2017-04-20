namespace ServerSentEventsClient.Default {

	// TODO: Make it immutable
	public class ServerSentEventsMessage : IServerSentEventsMessage {

		public string Id { get; set; }
		public string Event { get; set; }
		public string Data { get; set; }

	}

}