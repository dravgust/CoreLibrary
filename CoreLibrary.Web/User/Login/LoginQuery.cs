using MediatR;

namespace CoreLibrary.Web.User.Login
{
    public class LoginQuery : IRequest<WebUser>
	{
		public string Email { get; set; }

		public string Password { get; set; }
	}
}
