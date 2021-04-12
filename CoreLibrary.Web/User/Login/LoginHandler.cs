using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CoreLibrary.Domain;
using CoreLibrary.Web.Exceptions;
using CoreLibrary.Web.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CoreLibrary.Web.User.Login
{
    public class LoginHandler : IRequestHandler<LoginQuery, WebUser>
	{
		private readonly UserManager<ApplicationUser> _userManager;

		private readonly SignInManager<ApplicationUser> _signInManager;

		private readonly IJwtGenerator _jwtGenerator;

		public LoginHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtGenerator jwtGenerator)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_jwtGenerator = jwtGenerator;
		}

		public async Task<WebUser> Handle(LoginQuery request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
			{
				throw new RestException(HttpStatusCode.Unauthorized);
			}

			var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

			if (result.Succeeded)
			{
				return new WebUser
				{
								DisplayName = user.DisplayName,
								Token = _jwtGenerator.CreateToken(user),
								UserName = user.UserName,
								Image = null
							};
			}

			throw new RestException(HttpStatusCode.Unauthorized);
		}
	}
}
