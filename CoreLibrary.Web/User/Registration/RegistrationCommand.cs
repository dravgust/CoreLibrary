using MediatR;

namespace CoreLibrary.Web.User.Registration
{
	public class RegistrationCommand : IRequest<WebUser>
	{
		public string DisplayName { get; set; }

		public string UserName { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }
	}
}