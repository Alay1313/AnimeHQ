using AutoMapper;
using Application;
using Domain;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using FluentAssertions;

namespace AnimeHQ.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepo> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IPasswordHasher<User>> _hasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly UserService _sut; // "system under test"

    public UserServiceTests()
    {
        _repoMock = new Mock<IUserRepo>();
        _mapperMock = new Mock<IMapper>();
        _hasherMock = new Mock<IPasswordHasher<User>>();
        _tokenServiceMock = new Mock<ITokenService>();

        _sut = new UserService(_repoMock.Object, _mapperMock.Object, _hasherMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithNewUsername_CreatesUserAndReturnsToken()
    {
        // Arrange
        var dto = new RegisterDto { Username = "newuser", Email = "new@test.com", Password = "Password123" };

        _repoMock.Setup(r => r.GetByUsernameAsync(dto.Username))
            .ReturnsAsync((User?)null); // no existing user

        _mapperMock.Setup(m => m.Map<User>(dto))
            .Returns(new User { UserName = dto.Username, Email = dto.Email });

        _hasherMock.Setup(h => h.HashPassword(It.IsAny<User>(), dto.Password))
            .Returns("hashed-password");

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u); // echo back what was passed in

        _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");

        // Act
        var result = await _sut.RegisterAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be(dto.Username);
        result.Email.Should().Be(dto.Email);
        result.Token.Should().Be("fake-jwt-token");

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ThrowsException()
    {
        // Arrange
        var dto = new RegisterDto { Username = "existinguser", Email = "test@test.com", Password = "Password123" };

        _repoMock.Setup(r => r.GetByUsernameAsync(dto.Username))
            .ReturnsAsync(new User { UserName = dto.Username, Email = "old@test.com" });

        // Act
        var act = async () => await _sut.RegisterAsync(dto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Username already exists.");

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithCorrectCredentials_ReturnsToken()
    {
        // Arrange
        var dto = new LoginDto { UsernameOrEmail = "testuser", Password = "Password123" };
        var existingUser = new User { Id = "user-1", UserName = "testuser", Email = "test@test.com", PasswordHash = "hashed" };

        _repoMock.Setup(r => r.GetByUsernameAsync(dto.UsernameOrEmail))
            .ReturnsAsync(existingUser);

        _hasherMock.Setup(h => h.VerifyHashedPassword(existingUser, existingUser.PasswordHash, dto.Password))
            .Returns(PasswordVerificationResult.Success);

        _tokenServiceMock.Setup(t => t.GenerateToken(existingUser))
            .Returns("fake-jwt-token");

        // Act
        var result = await _sut.LoginAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("fake-jwt-token");
        result.Username.Should().Be(existingUser.UserName);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ThrowsException()
    {
        // Arrange
        var dto = new LoginDto { UsernameOrEmail = "testuser", Password = "WrongPassword" };
        var existingUser = new User { Id = "user-1", UserName = "testuser", Email = "test@test.com", PasswordHash = "hashed" };

        _repoMock.Setup(r => r.GetByUsernameAsync(dto.UsernameOrEmail))
            .ReturnsAsync(existingUser);

        _hasherMock.Setup(h => h.VerifyHashedPassword(existingUser, existingUser.PasswordHash, dto.Password))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var act = async () => await _sut.LoginAsync(dto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task LoginAsync_WithNonexistentUser_ThrowsException()
    {
        // Arrange
        var dto = new LoginDto { UsernameOrEmail = "ghost", Password = "Password123" };

        _repoMock.Setup(r => r.GetByUsernameAsync(dto.UsernameOrEmail))
            .ReturnsAsync((User?)null);
        _repoMock.Setup(r => r.GetByEmailAsync(dto.UsernameOrEmail))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _sut.LoginAsync(dto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Invalid credentials.");
    }
}