using System.IO;
using System.Threading.Tasks;

namespace ServerSentEventsClient {

	internal interface IEventStreamClient {

		Task<Stream> StartAsync();

	}

}