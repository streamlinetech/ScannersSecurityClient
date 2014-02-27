using System;
using RestSharp;

namespace SecurityClient.Core.Exceptions
{
	public class SecurityHttpException : Exception
	{
		public IRestResponse Response { get; set; }

		public SecurityHttpException(IRestResponse response)
			: base(response.ErrorMessage, response.ErrorException)
		{
			Response = response;
		}
	}
}