
using AutoMapper;
using Domain;
namespace Application;

public class Mapping : Profile
{
    public Mapping()
    {

        CreateMap<JikanAnimeData, AnimeDto>()
            .ForMember(dest => dest.AnimeListId, opt => opt.MapFrom(src => src.MalId))
            .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.Images.Jpg.ImageUrl ?? string.Empty))
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



        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User == null ? null : src.User.UserName))
            .ForMember(dest => dest.AnimeTitle, opt => opt.MapFrom(src => src.Anime == null ? null : src.Anime.Title));



        CreateMap<CreateReviewDto, Review>()
            .ForMember(dest => dest.ReviewId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore()) 
            .ForMember(dest => dest.Anime, opt => opt.Ignore());



        CreateMap<Favorite, FavoriteDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User == null ? null : src.User.UserName))
            .ForMember(dest => dest.AnimeTitle, opt => opt.MapFrom(src => src.Anime == null ? null : src.Anime.Title));




        CreateMap<CreateFavoriteDto, Favorite>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow)) 
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Anime, opt => opt.Ignore());



        CreateMap<Genre, GenreDto>()
            .ForMember(dest => dest.AnimeCount, opt => opt.MapFrom(src => src.AnimeGenres == null ? 0 : src.AnimeGenres.Count));

        CreateMap<GenreDto, Genre>()
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
    }

 
        

        

      

            


            
            


            
    }



