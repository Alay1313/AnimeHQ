using AutoMapper;
using Application;
using Domain;
using Microsoft.Extensions.Caching.Memory;
using Persistence;
using Moq;
using Xunit;
using FluentAssertions;

namespace AnimeHQ.Tests;

public class AnimeServiceTests
{
    private readonly Mock<IAnimeRepo> _repoMock;
    private readonly Mock<IJikanService> _jikanMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IGenreRepo> _genreRepoMock;
    private readonly IMemoryCache _cache;
    private readonly AnimeService _sut;

    public AnimeServiceTests()
    {
        _repoMock = new Mock<IAnimeRepo>();
        _jikanMock = new Mock<IJikanService>();
        _mapperMock = new Mock<IMapper>();
        _genreRepoMock = new Mock<IGenreRepo>();
        _cache = new MemoryCache(new MemoryCacheOptions());

        _sut = new AnimeService(_repoMock.Object, _jikanMock.Object, _mapperMock.Object, _cache, _genreRepoMock.Object);
    }

    private static Anime MakeAnime(int id, string title) => new Anime
    {
        AnimeListId = id,
        Title = title,
        ImageURL = "https://example.com/image.jpg",
        Synopsis = "Test synopsis",
        Type = "TV",
        Status = "Airing"
    };

    [Fact]
    public async Task SearchAsync_WithLocalResults_DoesNotCallJikan()
    {
        // Arrange
        var localAnime = new List<Anime> { MakeAnime(1, "Naruto") };
        var mappedDtos = new List<AnimeDto> { new AnimeDto { AnimeListId = 1, Title = "Naruto" } };

        _repoMock.Setup(r => r.SearchAsync("naruto", 1, 20, null, default))
            .ReturnsAsync(localAnime);

        _mapperMock.Setup(m => m.Map<IEnumerable<AnimeDto>>(localAnime))
            .Returns(mappedDtos);

        // Act
        var result = await _sut.SearchAsync("naruto", 1, 20);

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Naruto");

        _jikanMock.Verify(j => j.SearchAnimeAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_WithNoLocalResults_FallsBackToJikan()
    {
        // Arrange
        _repoMock.Setup(r => r.SearchAsync("onepiece", 1, 20, null, default))
            .ReturnsAsync(new List<Anime>());

        var jikanResults = new List<JikanAnimeData> { new JikanAnimeData { MalId = 21, Title = "One Piece" } };
        _jikanMock.Setup(j => j.SearchAnimeAsync("onepiece", 1))
            .ReturnsAsync(jikanResults);

        var mappedDtos = new List<AnimeDto> { new AnimeDto { AnimeListId = 21, Title = "One Piece" } };
        _mapperMock.Setup(m => m.Map<IEnumerable<AnimeDto>>(jikanResults))
            .Returns(mappedDtos);

        // Act
        var result = await _sut.SearchAsync("onepiece", 1, 20);

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("One Piece");

        _jikanMock.Verify(j => j.SearchAnimeAsync("onepiece", 1), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_CachesResults_SecondCallDoesNotHitRepoAgain()
    {
        // Arrange
        var localAnime = new List<Anime> { MakeAnime(5, "Bleach") };
        var mappedDtos = new List<AnimeDto> { new AnimeDto { AnimeListId = 5, Title = "Bleach" } };

        _repoMock.Setup(r => r.SearchAsync("bleach", 1, 20, null, default))
            .ReturnsAsync(localAnime);
        _mapperMock.Setup(m => m.Map<IEnumerable<AnimeDto>>(localAnime))
            .Returns(mappedDtos);

        // Act — call twice
        await _sut.SearchAsync("bleach", 1, 20);
        await _sut.SearchAsync("bleach", 1, 20);

        // Assert — repo should only be hit once, second call served from cache
        _repoMock.Verify(r => r.SearchAsync("bleach", 1, 20, null, default), Times.Once);
    }

    [Fact]
    public async Task SyncFromJikanAsync_MapsJikanGenreNamesToLocalGenreIds()
    {
        // Arrange
        var malId = 999;
        var jikanData = new JikanAnimeData
        {
            MalId = malId,
            Title = "Test Anime",
            Genres = new List<JikanGenre>
            {
                new JikanGenre { Name = "Action" },
                new JikanGenre { Name = "Fantasy" }
            }
        };

        _jikanMock.Setup(j => j.GetAnimeAsync(malId))
            .ReturnsAsync(new JikanAnimeResponse { Data = jikanData });

        _mapperMock.Setup(m => m.Map<UpdateAnimeDto>(jikanData))
            .Returns(new UpdateAnimeDto { AnimeListId = malId, Title = "Test Anime", Status = "Airing" });

        _genreRepoMock.Setup(g => g.GetIdsByNamesAsync(
                It.Is<List<string>>(names => names.Contains("Action") && names.Contains("Fantasy")),
                default))
            .ReturnsAsync(new List<int> { 1, 5 });

        _repoMock.Setup(r => r.GetByIdAsync(malId, default))
            .ReturnsAsync((Anime?)null);

        _mapperMock.Setup(m => m.Map<CreateAnimeDto>(It.IsAny<UpdateAnimeDto>()))
            .Returns(new CreateAnimeDto { AnimeListId = malId, Title = "Test Anime" });

        _mapperMock.Setup(m => m.Map<Anime>(It.IsAny<CreateAnimeDto>()))
            .Returns(MakeAnime(malId, "Test Anime"));

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Anime>(), default))
            .ReturnsAsync(MakeAnime(malId, "Test Anime"));

        _mapperMock.Setup(m => m.Map<AnimeDto>(It.IsAny<Anime>()))
            .Returns(new AnimeDto { AnimeListId = malId, Title = "Test Anime" });

        // Act
        var result = await _sut.SyncFromJikanAsync(malId);

        // Assert
        result.Should().NotBeNull();
        _genreRepoMock.Verify(g => g.GetIdsByNamesAsync(
            It.Is<List<string>>(names => names.Contains("Action") && names.Contains("Fantasy")),
            default), Times.Once);
    }

    [Fact]
    public async Task SyncFromJikanAsync_WhenJikanReturnsNull_ThrowsException()
    {
        // Arrange
        var malId = 123;
        _jikanMock.Setup(j => j.GetAnimeAsync(malId))
            .ReturnsAsync((JikanAnimeResponse?)null);

        // Act
        var act = async () => await _sut.SyncFromJikanAsync(malId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Failed to fetch data from Jikan API.");
    }
}