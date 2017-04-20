using System.IO;
using System.Threading.Tasks;

namespace ServerSentEventsClient {

	internal interface IEventStreamProcessor {

		Task ProcessAsync( Stream eventStream );

	}

}