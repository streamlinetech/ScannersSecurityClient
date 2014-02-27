using System;
using System.Linq;
using Streamline.Security.Scanners.Core;
using Streamline.Security.Scanners.Core.Dtos;
using Streamline.Security.Scanners.Core.Exceptions;

namespace Terminal
{
	class Program
	{
		static void Main(string[] args)
		{
			var activeDirectoryId = new Guid("7F9E8D3E-1D58-4B49-8D38-084DCCD5B803");
			var ability = "wss superuser";

			try
			{
				// Valid
				var result = SecurityClient.IsUserInAbility("http://api-authorization.common.streamlinedb.dev/v1/", activeDirectoryId, ability);
				Console.WriteLine("Valid User and Ability should be true {0}", result);

				// Bogus Ability
				result = SecurityClient.IsUserInAbility("http://api-authorization.common.streamlinedb.dev/v1/", activeDirectoryId, "Bogus");
				Console.WriteLine("Bogus Ability, Should be false: {0}", result);
				
				// Empty Array 
				result = SecurityClient.IsUserInAbility("http://api-authorization.common.streamlinedb.dev/v1/", new ActiveDirectoryBasedAuthorizationRequest(activeDirectoryId, Enumerable.Empty<string>()));
				Console.WriteLine("Empty Array, Should be false: {0}", result);

				// Empty Guid
				result = SecurityClient.IsUserInAbility("http://api-authorization.common.streamlinedb.dev/v1/", new ActiveDirectoryBasedAuthorizationRequest(Guid.Empty, Enumerable.Empty<string>()));
				Console.WriteLine("Empty Guid, Should be false: {0}", result);

				// Valid, Url Without Slash
				result = SecurityClient.IsUserInAbility("http://api-authorization.common.streamlinedb.dev/v1", activeDirectoryId, ability);
				Console.WriteLine("Valid, Url Without Slash: {0}", result);

				result = SecurityClient.IsUserInAbility("http://zzapi-authorization.common.streamlinedb.dev/v1", activeDirectoryId, ability);
				Console.WriteLine("Bad Url, should be false: {0}", result);

			}
			catch (SecurityHttpException ex)
			{
				Console.WriteLine(ex);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}


			Console.ReadLine();
		}
	}
}
