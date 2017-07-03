using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSE.UnitTests {

	[TestFixture]
	public class ServerSentEventsMessageParserTests {

		private IServerSentEventsMessageParser m_sut;

		[SetUp]
		public void SetUp() {
			m_sut = new MessageParser();
		}

		[TestCase( "data: Hello, World!\r\n\r\n" )]
		[TestCase( "data:Hello, World!\r\n\r\n" )]
		[TestCase( "data: Hello, World!\r\r" )]
		[TestCase( "data: Hello, World!\n\n" )]
		public void Parse_WhenDataIsFullAndCorrect_ReturnsOneMessage( string data ) {
			byte[] bytes = Encoding.UTF8.GetBytes( data );

			IEnumerable<ServerSentEventsMessage> messages = m_sut.Parse( new ArraySegment<byte>( bytes ) );

			ServerSentEventsMessage serverSentEventsMessage = messages.Single();
			Assert.That( serverSentEventsMessage.Data, Is.EqualTo( "Hello, World!" ) );
		}

		[TestCase( "data: Hello, World!\r\n\r\ndata: Good bye!" )]
		[TestCase( "data:Hello, World!\r\n\r\ndata:Good bye!" )]
		[TestCase( "data: Hello, World!\r\n\r\ndata: Good bye!\r\n" )]
		[TestCase( "data: Hello, World!\r\rdata: Good bye!" )]
		[TestCase( "data: Hello, World!\r\rdata: Good bye!\r" )]
		[TestCase( "data: Hello, World!\n\ndata: Good bye!" )]
		[TestCase( "data: Hello, World!\n\ndata: Good bye!\n" )]
		public void Parse_WhenDataIsIncompleteAndCorrect_ReturnsOneMessage( string data ) {
			byte[] bytes = Encoding.UTF8.GetBytes( data );

			IEnumerable<ServerSentEventsMessage> messages = m_sut.Parse( new ArraySegment<byte>( bytes ) );

			ServerSentEventsMessage serverSentEventsMessage = messages.Single();
			Assert.That( serverSentEventsMessage.Data, Is.EqualTo( "Hello, World!" ) );
		}

		[TestCase( "data: Hello, World!\r\n\r\ndata: Good bye!\r\n\r\n" )]
		[TestCase( "data:Hello, World!\r\n\r\ndata:Good bye!\r\n\r\n" )]
		[TestCase( "data:Hello, World!\r\rdata:Good bye!\r\r" )]
		[TestCase( "data:Hello, World!\n\ndata:Good bye!\n\n" )]
		public void Parse_WhenDataIsFullAndCorrect_ReturnsTwoMessages( string data ) {
			byte[] bytes = Encoding.UTF8.GetBytes( data );

			var messages = m_sut.Parse( new ArraySegment<byte>( bytes ) ).ToList();

			Assert.That( messages.Count, Is.EqualTo( 2 ) );
			Assert.That( messages[0].Data, Is.EqualTo( "Hello, World!" ) );
			Assert.That( messages[1].Data, Is.EqualTo( "Good bye!" ) );
		}

		[TestCase( "data: Hello, World!\r\n\r", "\ndata: Good bye!\r\n\r\n" )]
		[TestCase( "data: Hello, World!\r\n", "\r\ndata: Good bye!\r\n\r\n" )]
		[TestCase( "data: Hello, World!\r", "\n\r\ndata: Good bye!\r\n\r\n" )]
		[TestCase( "data: Hello, World!", "\r\n\r\ndata: Good bye!\r\n\r\n" )]
		[TestCase( "data: Hell", "o, World!\r\n\r\ndata: Good bye!\r\n\r\n" )]
		[TestCase( "data: Hello, World!\r\n\r\ndata: ", "Good bye!\r\n\r\n" )]
		[TestCase( "data: Hello, World!\r\n\r\ndata: Good bye!\r", "\n\r\n" )]
		[TestCase( "data: Hello, World!\r\n\r\ndata: Good bye!\r\n", "\r\n" )]
		[TestCase( "data: Hello, World!\r\n\r\ndata: Good bye!\r\n\r", "\n" )]
		[TestCase( "da", "ta:Hello, World!\r\rdata:Good bye!\r\r" )]
		[TestCase( "data:Hello, World!\r", "\rdata:Good bye!\r\r" )]
		[TestCase( "data:Hello, World!\r\rdata:Good bye!\r", "\r" )]
		[TestCase( "data:Hello, W", "orld!\n\ndata:Good bye!\n\n" )]
		[TestCase( "data:Hello, World!\n", "\ndata:Good bye!\n\n" )]
		[TestCase( "data:Hello, World!\n\ndata:Good bye!\n", "\n" )]
		public void Parse_WhenDataIsCorrectAndChunked_ReturnsTwoMessages( string chunk1, string chunk2 ) {
			byte[] bytes = Encoding.UTF8.GetBytes( chunk1 );

			var messages = m_sut.Parse( new ArraySegment<byte>( bytes ) ).ToList();

			bytes = Encoding.UTF8.GetBytes( chunk2 );

			messages = messages.Concat( m_sut.Parse( new ArraySegment<byte>( bytes ) ) ).ToList();

			Assert.That( messages.Count, Is.EqualTo( 2 ) );
			Assert.That( messages[0].Data, Is.EqualTo( "Hello, World!" ) );
			Assert.That( messages[1].Data, Is.EqualTo( "Good bye!" ) );
		}

	}

}