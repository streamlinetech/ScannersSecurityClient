using System;
using System.Collections;
using System.Collections.Generic;

namespace SecurityClient.Core.Dtos
{
	public class ActiveDirectoryBasedAuthorizationRequest
	{
		public ActiveDirectoryBasedAuthorizationRequest()
		{

		}

		public ActiveDirectoryBasedAuthorizationRequest(Guid activeDirectoryId, IEnumerable<string> abilities)
		{
			ActiveDirectoryId = activeDirectoryId;
			Abilities = abilities;
		}

		public Guid ActiveDirectoryId { get; set; }

		public IEnumerable<string> Abilities { get; set; }
	}
}