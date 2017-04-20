namespace ServerSentEventsClient {

	public interface IServerSentEventsMessage {

		string Id { get; set; }
		string Event { get; set; }
		string Data { get; set; }

	}

}