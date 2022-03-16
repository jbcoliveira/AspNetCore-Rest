using AutoMapper;
using Joao.API.DTO;
using Joao.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Joao.API.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Fornecedor, FornecedorDTO>().ReverseMap();
            CreateMap<ProdutoDTO , Produto>();

            CreateMap<ProdutoImagemDTO, Produto>().ReverseMap();

            CreateMap<Produto, ProdutoDTO>()
                .ForMember(dest => dest.NomeFornecedor, opt => opt.MapFrom(src => src.Fornecedor.Nome));

            CreateMap<Endereco, EnderecoDTO>().ReverseMap();
        }
    }
}
