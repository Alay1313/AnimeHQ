using AutoMapper;
using Application;
using Domain;
using Microsoft.Extensions.Caching.Memory;
using Persistence;
using Moq;
using Xunit;
using FluentAssertions;

namespace AnimeHQ.Tests;

public class MangaServiceTests
{
    private readonly Mock<IMangaRepo> _repoMock;
    private readonly Mock<IMangaFavoriteRepo> _favoriteRepoMock;
    private readonly Mock<IMangaReviewRepo> _reviewRepoMock;
    private readonly Mock<IJikanService> _jikanMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IMemoryCache _cache;
    private readonly MangaService _sut;

    public MangaServiceTests()
    {
        _repoMock = new Mock<IMangaRepo>();
        _favoriteRepoMock = new Mock<IMangaFavoriteRepo>();
        _reviewRepoMock = new Mock<IMangaReviewRepo>();
        _jikanMock = new Mock<IJikanService>();
        _mapperMock = new Mock<IMapper>();
        _cache = new MemoryCache(new MemoryCacheOptions());

        _sut = new MangaService(
            _repoMock.Object,
            _favoriteRepoMock.Object,
            _reviewRepoMock.Object,
            _jikanMock.Object,
            _mapperMock.Object,
            _cache);
    }

    private static Manga MakeManga(int id, string title) => new Manga
    {
        MangaId = id,
        Title = title,
        ImageURL = "https://example.com/cover.jpg",
        Synopsis = "Test synopsis",
        Type = "Manga",
        Status = "Publishing"
    };

    [Fact]
    public async Task SearchAsync_WithJikanResults_DoesNotHitLocalDb()
    {
        // Arrange — Jikan succeeds, so local DB fallback should never be called
        var jikanResults = new List<JikanMangaData> { new JikanMangaData { MalId = 1, Title = "One Piece" } };
        _jikanMock.Setup(j => j.SearchMangaAsync("onepiece", 1))
            .ReturnsAsync(jikanResults);

        var mappedDtos = new List<MangaDto> { new MangaDto { MangaId = 1, Title = "One Piece" } };
        _mapperMock.Setup(m => m.Map<IEnumerable<MangaDto>>(jikanResults))
            .Returns(mappedDtos);

        // Act
        var result = await _sut.SearchAsync("onepiece", 1, 25);

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("One Piece");

        _repoMock.Verify(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_WhenJikanReturnsNull_FallsBackToLocalDb()
    {
        // Arrange — Jikan fails/null, should fall back to local DB cache
        _jikanMock.Setup(j => j.SearchMangaAsync("vinland", 1))
            .ReturnsAsync((IEnumerable<JikanMangaData>?)null);

        var localResults = new List<Manga> { MakeManga(2, "Vinland Saga") };
        _repoMock.Setup(r => r.SearchAsync("vinland", 1, 25, default))
            .ReturnsAsync(localResults);

        var mappedDtos = new List<MangaDto> { new MangaDto { MangaId = 2, Title = "Vinland Saga" } };
        _mapperMock.Setup(m => m.Map<IEnumerable<MangaDto>>(localResults))
            .Returns(mappedDtos);

        // Act
        var result = await _sut.SearchAsync("vinland", 1, 25);

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Vinland Saga");
    }

    [Fact]
    public async Task SearchAsync_CachesResults_SecondCallDoesNotHitJikanAgain()
    {
        // Arrange
        var jikanResults = new List<JikanMangaData> { new JikanMangaData { MalId = 3, Title = "Berserk" } };
        _jikanMock.Setup(j => j.SearchMangaAsync("berserk", 1))
            .ReturnsAsync(jikanResults);

        var mappedDtos = new List<MangaDto> { new MangaDto { MangaId = 3, Title = "Berserk" } };
        _mapperMock.Setup(m => m.Map<IEnumerable<MangaDto>>(jikanResults))
            .Returns(mappedDtos);

        // Act — call twice
        await _sut.SearchAsync("berserk", 1, 25);
        await _sut.SearchAsync("berserk", 1, 25);

        // Assert — Jikan should only be hit once, second call served from cache
        _jikanMock.Verify(j => j.SearchMangaAsync("berserk", 1), Times.Once);
    }

    [Fact]
    public async Task GetTopMangaAsync_WhenJikanReturnsNull_FallsBackToLocalCache()
    {
        // Arrange
        _jikanMock.Setup(j => j.GetTopMangaAsync(1, 25, default))
            .ReturnsAsync((IEnumerable<JikanMangaData>?)null);

        var cachedTop = new List<Manga> { MakeManga(4, "Attack on Titan") };
        _repoMock.Setup(r => r.GetTopCachedAsync(1, 25, default))
            .ReturnsAsync(cachedTop);

        var mappedDtos = new List<MangaDto> { new MangaDto { MangaId = 4, Title = "Attack on Titan" } };
        _mapperMock.Setup(m => m.Map<IEnumerable<MangaDto>>(cachedTop))
            .Returns(mappedDtos);

        // Act
        var result = await _sut.GetTopMangaAsync(1, 25);

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Attack on Titan");
    }

    [Fact]
    public async Task SyncFromJikanAsync_WhenMangaDoesNotExistLocally_CreatesNewEntry()
    {
        // Arrange
        var malId = 100;
        var jikanData = new JikanMangaData { MalId = malId, Title = "Chainsaw Man" };

        _jikanMock.Setup(j => j.GetMangaAsync(malId))
            .ReturnsAsync(new JikanMangaResponse { Data = jikanData });

        _repoMock.Setup(r => r.GetByIdAsync(malId, default))
            .ReturnsAsync((Manga?)null); // doesn't exist yet

        var mappedManga = MakeManga(malId, "Chainsaw Man");
        _mapperMock.Setup(m => m.Map<Manga>(jikanData))
            .Returns(mappedManga);

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Manga>(), default))
            .ReturnsAsync(mappedManga);

        _mapperMock.Setup(m => m.Map<MangaDto>(mappedManga))
            .Returns(new MangaDto { MangaId = malId, Title = "Chainsaw Man" });

        // Act
        var result = await _sut.SyncFromJikanAsync(malId);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Chainsaw Man");

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Manga>(), default), Times.Once);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<Manga>(), default), Times.Never);
    }

    [Fact]
    public async Task SyncFromJikanAsync_WhenMangaExistsLocally_UpdatesExistingEntry()
    {
        // Arrange
        var malId = 200;
        var jikanData = new JikanMangaData { MalId = malId, Title = "Jujutsu Kaisen" };

        _jikanMock.Setup(j => j.GetMangaAsync(malId))
            .ReturnsAsync(new JikanMangaResponse { Data = jikanData });

        var existingManga = MakeManga(malId, "Jujutsu Kaisen (old)");
        _repoMock.Setup(r => r.GetByIdAsync(malId, default))
            .ReturnsAsync(existingManga); // already exists

        var mappedManga = MakeManga(malId, "Jujutsu Kaisen");
        _mapperMock.Setup(m => m.Map<Manga>(jikanData))
            .Returns(mappedManga);

        _repoMock.Setup(r => r.UpdateAsync(malId, It.IsAny<Manga>(), default))
            .ReturnsAsync(mappedManga);

        _mapperMock.Setup(m => m.Map<MangaDto>(mappedManga))
            .Returns(new MangaDto { MangaId = malId, Title = "Jujutsu Kaisen" });

        // Act
        var result = await _sut.SyncFromJikanAsync(malId);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Jujutsu Kaisen");

        _repoMock.Verify(r => r.UpdateAsync(malId, It.IsAny<Manga>(), default), Times.Once);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Manga>(), default), Times.Never);
    }

    [Fact]
    public async Task SyncFromJikanAsync_WhenJikanReturnsNull_ThrowsException()
    {
        // Arrange
        var malId = 999;
        _jikanMock.Setup(j => j.GetMangaAsync(malId))
            .ReturnsAsync((JikanMangaResponse?)null);

        // Act
        var act = async () => await _sut.SyncFromJikanAsync(malId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Failed to fetch manga from Jikan API.");
    }
}