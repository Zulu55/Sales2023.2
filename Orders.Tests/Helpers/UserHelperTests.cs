using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Orders.Backend.Helpers;
using Orders.Backend.UnitsOfWork;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Orders.Tests.Helpers
{
    [TestClass]
    public class UserHelperTests
    {
        private Mock<UserManager<User>> _userManagerMock = null!;
        private Mock<RoleManager<IdentityRole>> _roleManagerMock = null!;
        private Mock<IUsersUnitOfWork> _userUnitOfWorkMock = null!;
        private Mock<SignInManager<User>> _signInManagerMock = null!;
        private UserHelper _userHelper = null!;

        [TestInitialize]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);
            var optionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
            var loggerMock = new Mock<ILogger<SignInManager<User>>>();
            var authenticationSchemeProviderMock = new Mock<IAuthenticationSchemeProvider>();
            var userConfirmationMock = new Mock<IUserConfirmation<User>>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                httpContextAccessorMock.Object,
                claimsFactoryMock.Object,
                optionsAccessorMock.Object,
                loggerMock.Object,
                authenticationSchemeProviderMock.Object,
                userConfirmationMock.Object);

            _userUnitOfWorkMock = new Mock<IUsersUnitOfWork>();

            _userHelper = new UserHelper(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _userUnitOfWorkMock.Object,
                _signInManagerMock.Object);
        }

        [TestMethod]
        public async Task LoginAsync_ShouldCallPasswordSignInAsync()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "test@example.com", Password = "TestPassword123!" };
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, false, false))
                .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await _userHelper.LoginAsync(loginDTO);

            // Assert
            Assert.AreEqual(SignInResult.Success, result);
            _signInManagerMock.Verify(x => x.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, false, false), Times.Once());
        }

        [TestMethod]
        public async Task LogoutAsync_ShouldCallSignOutAsync()
        {
            // Act
            await _userHelper.LogoutAsync();

            // Assert
            _signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
        }

        [TestMethod]
        public async Task AddUserAsync_ShouldReturnIdentityResult()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var password = "TestPassword123!";
            _userManagerMock.Setup(x => x.CreateAsync(user, password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userHelper.AddUserAsync(user, password);

            // Assert
            Assert.AreEqual(IdentityResult.Success, result);
        }

        [TestMethod]
        public async Task AddUserToRoleAsync_ShouldAddUserToRole()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var roleName = "Admin";
            _userManagerMock.Setup(x => x.AddToRoleAsync(user, roleName))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _userHelper.AddUserToRoleAsync(user, roleName);

            // Assert
            _userManagerMock.Verify(x => x.AddToRoleAsync(user, roleName), Times.Once);
        }

        [TestMethod]
        public async Task CheckRoleAsync_ShouldCreateRoleIfNotExists()
        {
            // Arrange
            var roleName = "Admin";
            _roleManagerMock.Setup(x => x.RoleExistsAsync(roleName))
                .ReturnsAsync(false);

            // Act
            await _userHelper.CheckRoleAsync(roleName);

            // Assert
            _roleManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
        }

        [TestMethod]
        public async Task GetUserAsync_WithEmail_ShouldReturnUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Email = email };
            _userUnitOfWorkMock.Setup(x => x.GetAsync(email))
                .ReturnsAsync(new Response<User> { WasSuccess = true, Result = user });

            // Act
            var result = await _userHelper.GetUserAsync(email);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(email, result.Email);
        }

        [TestMethod]
        public async Task IsUserInRoleAsync_ShouldReturnTrueIfUserIsInRole()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var roleName = "Admin";
            _userManagerMock.Setup(x => x.IsInRoleAsync(user, roleName))
                .ReturnsAsync(true);

            // Act
            var result = await _userHelper.IsUserInRoleAsync(user, roleName);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ChangePasswordAsync_ShouldReturnSuccessResult()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var currentPassword = "CurrentPassword123!";
            var newPassword = "NewPassword123!";
            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, currentPassword, newPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userHelper.ChangePasswordAsync(user, currentPassword, newPassword);

            // Assert
            Assert.AreEqual(IdentityResult.Success, result);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ShouldReturnSuccessResult()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            _userManagerMock.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userHelper.UpdateUserAsync(user);

            // Assert
            Assert.AreEqual(IdentityResult.Success, result);
        }

        [TestMethod]
        public async Task GetUserAsync_WithUserId_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId.ToString() };
            _userUnitOfWorkMock.Setup(x => x.GetAsync(userId))
                .ReturnsAsync(new Response<User> { WasSuccess = true, Result = user });

            // Act
            var result = await _userHelper.GetUserAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId.ToString(), result.Id);
        }

        [TestMethod]
        public async Task GenerateEmailConfirmationTokenAsync_ShouldReturnToken()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(user))
                .ReturnsAsync("Confirmation_Token");

            // Act
            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

            // Assert
            Assert.AreEqual("Confirmation_Token", token);
        }

        [TestMethod]
        public async Task ConfirmEmailAsync_ShouldReturnSuccessResult()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var token = "Confirmation_Token";
            _userManagerMock.Setup(x => x.ConfirmEmailAsync(user, token))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userHelper.ConfirmEmailAsync(user, token);

            // Assert
            Assert.AreEqual(IdentityResult.Success, result);
        }

        [TestMethod]
        public async Task GeneratePasswordResetTokenAsync_ShouldReturnToken()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("Reset_Token");

            // Act
            var token = await _userHelper.GeneratePasswordResetTokenAsync(user);

            // Assert
            Assert.AreEqual("Reset_Token", token);
        }

        [TestMethod]
        public async Task ResetPasswordAsync_ShouldReturnSuccessResult()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var token = "Reset_Token";
            var newPassword = "NewPassword123!";
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, token, newPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userHelper.ResetPasswordAsync(user, token, newPassword);

            // Assert
            Assert.AreEqual(IdentityResult.Success, result);
        }
    }
}