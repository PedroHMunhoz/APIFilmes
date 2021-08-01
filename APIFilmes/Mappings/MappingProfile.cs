using APIFilmes.DTOs;
using APIFilmes.Model;
using AutoMapper;

namespace APIFilmes.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Filme, FilmeDTO>().ReverseMap();
            CreateMap<Genero, GeneroDTO>().ReverseMap();
        }
    }
}
