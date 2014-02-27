using System;
using System.Net;

namespace Streamline.Security.Scanners.Core.Exceptions
{
	public class SecurityHttpException : Exception
	{
		public HttpWebResponse Response { get; set; }

		public SecurityHttpException(HttpWebResponse response)
		{
			Response = response;
		}
	}
}