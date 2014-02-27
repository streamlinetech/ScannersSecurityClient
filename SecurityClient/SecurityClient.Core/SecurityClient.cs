using System;
using System.Collections.Generic;
using System.Linq;
using Streamline.Security.Scanners.Core.Dtos;
using Streamline.Security.Scanners.Core.Exceptions;

namespace Streamline.Security.Scanners.Core
{
	public static class SecurityClient
	{
		/// <summary>
		/// The request url
		/// </summary>
		public static Uri RequestUrl { get; set; }

		/// <summary>
		/// Determines if a User is in an Ability
		/// 
		/// Throws SecurityHttpException
		/// </summary>
		/// <param name="baseUrl">The base url of the api, will look similar to http://api-authorization.common.streamlinedb.dev/v1 </param>
		/// <param name="activeDirectoryId">Active Directory GUID</param>
		/// <param name="ability">An ability to validate against.  (This is the 'Name' of the ability)</param>
		/// <returns>True if the user is in the ability, false otherwise</returns>
		public static bool IsUserInAbility(string baseUrl, Guid activeDirectoryId, string ability)
		{
			return IsUserInAbility(baseUrl, activeDirectoryId, new List<string>()
		                                   {
			                                   ability
		                                   });
		}

		/// <summary>
		/// Determines if a User is in an Ability
		/// 
		/// Throws SecurityHttpException
		/// </summary>
		/// <param name="baseUrl">The base url of the api, will look similar to http://api-authorization.common.streamlinedb.dev/v1 </param>
		/// <param name="activeDirectoryId">Active Directory GUID</param>
		/// <param name="abilities">An ability to validate against.  (This is the 'Name' of the ability)</param>
		/// <returns>True if the user is in the ability, false otherwise</returns>
		public static bool IsUserInAbility(string baseUrl, Guid activeDirectoryId, IEnumerable<string> abilities)
		{
			var request = new ActiveDirectoryBasedAuthorizationRequest(activeDirectoryId, abilities);
			return IsUserInAbility(baseUrl, request);
		}

		/// <summary>
		/// Determines if a User is in an Ability
		/// 
		/// Throws SecurityHttpException
		/// </summary>
		/// <param name="baseUrl">The base url of the api, will look similar to http://api-authorization.common.streamlinedb.dev/v1 </param>
		/// <param name="authorizationRequest">An object that holds the active directory id and abilities</param>
		/// <returns>True if the user is in the ability, false otherwise</returns>
		public static bool IsUserInAbility(string baseUrl, ActiveDirectoryBasedAuthorizationRequest authorizationRequest)
		{
			var isUserInAbility = false;
			if (!ValidateRequest(authorizationRequest))
				return false;

			var relativeUrl = "providers/activedirectory";
			if (!baseUrl.Contains("/v1"))
				relativeUrl += "v1/" + relativeUrl;

			RequestUrl = new Uri(new Uri(baseUrl), relativeUrl);
			RequestUrl.MakeResourceRequest().Post(authorizationRequest, (exception, response) =>
																		{
																			if (response != null)
																				isUserInAbility = response.StatusCode.IsSuccess();
																		});


			return isUserInAbility;
		}

		static bool ValidateRequest(ActiveDirectoryBasedAuthorizationRequest authorizationRequest)
		{
			if (!authorizationRequest.Abilities.Any())
				return false;
			if (authorizationRequest.ActiveDirectoryId == Guid.Empty)
				return false;
			return true;
		}
	}
}
