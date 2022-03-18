using AutoMapper;
using Joao.Api.Extensions;
using Joao.API.DTO;
using Joao.Business.Intefaces;
using Joao.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Joao.API.Controllers
{

    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;
        private readonly IEnderecoRepository _enderecoRepository;

        public FornecedoresController(IFornecedorRepository fornecedorRepository, 
            IFornecedorService fornecedorService, 
            IMapper mapper,
            INotificador notificador,
            IEnderecoRepository enderecoRepository) : base (notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
            _enderecoRepository = enderecoRepository;
        }


        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<FornecedorDTO>> ObterTodos()
        {
            return _mapper.Map<IEnumerable<FornecedorDTO>>(await _fornecedorRepository.ObterTodos());
        }

        [Authorize]
        public async Task<FornecedorDTO> ObterFornecedorEndereco(Guid id)
        {
            return _mapper.Map<FornecedorDTO>(await _fornecedorRepository.ObterFornecedorEndereco(id));

        }

        [Authorize]
        [HttpGet("obter-endereco/{id:guid}")]
        public async Task<FornecedorDTO> ObterEnderecoPorId(Guid id)
        {
            return _mapper.Map<FornecedorDTO>(await _enderecoRepository.ObterPorId(id));
        }

        [Authorize]
        [HttpGet("{id:guid}")]

        public async Task<ActionResult<FornecedorDTO>> ObterPorId(Guid id)
        {
            var fornecedor = _mapper.Map<FornecedorDTO>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
            if (fornecedor == null) return NotFound();
            return Ok(fornecedor);
        }

        [ClaimsAuthorize("Fornecedor","Adicionar")]
        [HttpPost]
        public async Task<ActionResult<FornecedorDTO>> Adicionar(FornecedorDTO fornecedorDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            
            await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorDTO));

            return CustomResponse(fornecedorDTO);

        }
        
        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorDTO>> Atualizar(Guid id, FornecedorDTO fornecedorDTO)
        {
            if (id != fornecedorDTO.Id) {
                NotificarErro("O id informado não é o mesmo que foi passado na query");
                return CustomResponse(fornecedorDTO);
            } 

            if (!ModelState.IsValid){
                return CustomResponse(ModelState);
            }
            
            await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorDTO));

            return CustomResponse(fornecedorDTO);

        }

        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorDTO>> Excluir(Guid id)
        {
            var fornecedorDTO = await ObterFornecedorEndereco(id);
            if (fornecedorDTO == null) return NotFound();

            await _fornecedorService.Remover(id);

            return CustomResponse();

        }
    }
}
