
using AutoMapper;
using Domain;
namespace Application;

public class Mapping : Profile
{
    public Mapping()
    {

        CreateMap<JikanAnimeData, AnimeDto>()
            .ForMember(dest => dest.AnimeListId, opt => opt.MapFrom(src => src.MalId))
            .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.Images.Jpg.LargeImageUrl ?? src.Images.Jpg.ImageUrl))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score ?? 0))
            .ForMember(dest => dest.Synopsis, opt => opt.MapFrom(src => src.Synopsis ?? string.Empty))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type ?? "Unknown"))
            .ForMember(dest => dest.Episodes, opt => opt.MapFrom(src => src.Episodes ?? 0))
            .ForMember(dest => dest.AiredFrom, opt => opt.MapFrom(src => src.Aired.From))
            .ForMember(dest => dest.AiredTo, opt => opt.MapFrom(src => src.Aired.To))
            .ForMember(dest => dest.GenreIds, opt => opt.MapFrom(src => src.Genres.Select(g => g.MalId)));


        CreateMap<JikanAnimeData, UpdateAnimeDto>()
            .ForMember(dest => dest.AnimeListId, opt => opt.MapFrom(src => src.MalId))
            .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.Images.Jpg.LargeImageUrl ?? src.Images.Jpg.ImageUrl))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score ?? 0))
            .ForMember(dest => dest.Synopsis, opt => opt.MapFrom(src => src.Synopsis ?? string.Empty))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type ?? "Unknown"))
            .ForMember(dest => dest.Episodes, opt => opt.MapFrom(src => src.Episodes ?? 0))
            .ForMember(dest => dest.AiredFrom, opt => opt.MapFrom(src => src.Aired.From))
            .ForMember(dest => dest.AiredTo, opt => opt.MapFrom(src => src.Aired.To))
            .ForMember(dest => dest.GenreIds, opt => opt.MapFrom(src => src.Genres.Select(g => g.MalId)));



        CreateMap<Anime, AnimeDto>()
            .ForMember(dest => dest.GenreIds, opt => opt.MapFrom(src => src.AnimeGenres.Select(ag => ag.GenreId)))
            .ForMember(dest => dest.AnimeGenres, opt => opt.MapFrom(src => src.AnimeGenres));


         CreateMap<AnimeDto, UpdateAnimeDto>();




         CreateMap<UpdateAnimeDto, CreateAnimeDto>();

         CreateMap<AnimeDto, CreateAnimeDto>();

         CreateMap<CreateAnimeDto, Anime>()
        .ForMember(dest => dest.CachedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
        .ForMember(dest => dest.AnimeGenres, opt => opt.Ignore());



        CreateMap<UpdateAnimeDto, Anime>()
            .ForMember(dest => dest.AnimeListId, opt => opt.Ignore()) 
            .ForMember(dest => dest.CachedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.AnimeGenres, opt => opt.Ignore()); 



        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews == null ? 0 : src.Reviews.Count))
            .ForMember(dest => dest.FavoriteCount, opt => opt.MapFrom(src => src.Favorites == null ? 0 : src.Favorites.Count));




        CreateMap<UserDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.JoinedAt))
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.Favorites, opt => opt.Ignore());


        CreateMap<RegisterDto, User>()
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
        .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
        .ForMember(dest => dest.Reviews, opt => opt.Ignore())
        .ForMember(dest => dest.Favorites, opt => opt.Ignore());



        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User == null ? null : src.User.UserName))
            .ForMember(dest => dest.AnimeTitle, opt => opt.MapFrom(src => src.Anime == null ? null : src.Anime.Title));



        
        CreateMap<UpdateReviewDto, Review>()
            .ForMember(dest => dest.ReviewId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.AnimeId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());



        CreateMap<CreateReviewDto, Review>()
            .ForMember(dest => dest.ReviewId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore()) 
            .ForMember(dest => dest.Anime, opt => opt.Ignore());



            
        CreateMap<Favorite, FavoriteDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null))
            .ForMember(dest => dest.AnimeTitle, opt => opt.MapFrom(src => src.Anime != null ? src.Anime.Title : null));

        
        CreateMap<CreateFavoriteDto, Favorite>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Handled in Service
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Anime, opt => opt.Ignore());



        CreateMap<Genre, GenreDto>()
            .ForMember(dest => dest.AnimeCount, opt => opt.MapFrom(src => src.AnimeGenres == null ? 0 : src.AnimeGenres.Count));

        CreateMap<CreateGenreDto, Genre>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AnimeGenres, opt => opt.Ignore());



        CreateMap<Episode, EpisodeDto>();

        CreateMap<CreateEpisodeDto, Episode>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());



        CreateMap<UpdateEpisodeDto, Episode>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AnimeId, opt => opt.Ignore());




        CreateMap<JikanEpisodeData, Episode>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AnimeId, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title ?? string.Empty))
            .ForMember(dest => dest.AirDate, opt => opt.MapFrom(src => src.Aired))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration ?? string.Empty))
            .ForMember(dest => dest.Synopsis, opt => opt.MapFrom(src => string.Empty));



        CreateMap<JikanMangaData, Manga>()
        .ForMember(dest => dest.MangaId, opt => opt.MapFrom(src => src.MalId))
        .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src =>
        src.Images != null ? src.Images.Jpg.LargeImageUrl ?? src.Images.Jpg.ImageUrl : string.Empty))
        .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score ?? 0))
        .ForMember(dest => dest.Synopsis, opt => opt.MapFrom(src => src.Synopsis ?? string.Empty))
        .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type ?? "Unknown"))
        .ForMember(dest => dest.Chapters, opt => opt.MapFrom(src => src.Chapters ?? 0))
        .ForMember(dest => dest.Volumes, opt => opt.MapFrom(src => src.Volumes ?? 0))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "Unknown"))
        .ForMember(dest => dest.PublishedFrom, opt => opt.MapFrom(src => src.Published != null ? src.Published.From : default))
        .ForMember(dest => dest.PublishedTo, opt => opt.MapFrom(src => src.Published != null ? src.Published.To : default))
        .ForMember(dest => dest.Reviews, opt => opt.Ignore())
        .ForMember(dest => dest.Favorites, opt => opt.Ignore());

    CreateMap<Manga, MangaDto>();

    CreateMap<CreateMangaFavoriteDto, MangaFavorite>();

    CreateMap<MangaFavorite, MangaFavoriteDto>()
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null))
        .ForMember(dest => dest.MangaTitle, opt => opt.MapFrom(src => src.Manga != null ? src.Manga.Title : null));

    CreateMap<CreateMangaReviewDto, MangaReview>();

    CreateMap<MangaReview, MangaReviewDto>()
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null))
        .ForMember(dest => dest.MangaTitle, opt => opt.MapFrom(src => src.Manga != null ? src.Manga.Title : null));

    CreateMap<JikanMangaData, MangaDto>()
        .ForMember(dest => dest.MangaId, opt => opt.MapFrom(src => src.MalId))
        .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src =>
        src.Images != null ? src.Images.Jpg.LargeImageUrl ?? src.Images.Jpg.ImageUrl : string.Empty))
        .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score ?? 0))
        .ForMember(dest => dest.Synopsis, opt => opt.MapFrom(src => src.Synopsis ?? string.Empty))
        .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type ?? "Unknown"))
        .ForMember(dest => dest.Chapters, opt => opt.MapFrom(src => src.Chapters ?? 0))
        .ForMember(dest => dest.Volumes, opt => opt.MapFrom(src => src.Volumes ?? 0))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "Unknown"))
        .ForMember(dest => dest.PublishedFrom, opt => opt.MapFrom(src => src.Published != null ? src.Published.From : default))
        .ForMember(dest => dest.PublishedTo, opt => opt.MapFrom(src => src.Published != null ? src.Published.To : default));
    }

 
        

        

      

            


            
            


            
    }



