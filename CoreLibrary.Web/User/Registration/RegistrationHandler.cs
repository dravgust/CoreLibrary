using System;
using System.Threading;
using System.Threading.Tasks;
using CoreLibrary.Domain;
using CoreLibrary.Web.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CoreLibrary.Web.User.Registration
{
	public class RegistrationHandler : IRequestHandler<RegistrationCommand, WebUser>
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IJwtGenerator _jwtGenerator;
		//private readonly DataContext _context;

		public RegistrationHandler(/*DataContext context,*/ UserManager<ApplicationUser> userManager, IJwtGenerator jwtGenerator)
		{
			//_context = context;
			_userManager = userManager;
			_jwtGenerator = jwtGenerator;
		}

		public async Task<WebUser> Handle(RegistrationCommand request, CancellationToken cancellationToken)
		{
			//if (await _context.Users.Where(x => x.Email == request.Email).AnyAsync())
			//{
			//	throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exist" });
			//}

			//if (await _context.Users.Where(x => x.UserName == request.UserName).AnyAsync())
			//{
			//	throw new RestException(HttpStatusCode.BadRequest, new { UserName = "UserName already exist" });
			//}

			var user = new ApplicationUser
							{
								DisplayName = request.DisplayName,
								Email = request.Email,
								UserName = request.UserName
							};

			var result = await _userManager.CreateAsync(user, request.Password);

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

			throw new Exception("Client creation failed");
		}
	}
}