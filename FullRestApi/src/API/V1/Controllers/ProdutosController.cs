using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Joao.API.Controllers;
using Joao.API.DTO;
using Joao.Business.Intefaces;
using Joao.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Joao.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;

        public IProdutoService ProdutoService => _produtoService;

        public ProdutosController(INotificador notificador, 
                                  IProdutoRepository produtoRepository, 
                                  IProdutoService produtoService, 
                                  IMapper mapper,
                                  IUser user) : base(notificador, user)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<ProdutoDTO>> ObterTodos()
        {
            return _mapper.Map<IEnumerable<ProdutoDTO>>(await _produtoRepository.ObterProdutosFornecedores());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoDTO>> ObterPorId(Guid id)
        {
            var ProdutoDTO = await ObterProduto(id);

            if (ProdutoDTO == null) return NotFound();

            return ProdutoDTO;
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> Adicionar(ProdutoDTO ProdutoDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imagemNome = Guid.NewGuid() + "_" + ProdutoDTO.Imagem;
            if (!UploadArquivo(ProdutoDTO.ImagemUpload, imagemNome))
            {
                return CustomResponse(ProdutoDTO);
            }

            ProdutoDTO.Imagem = imagemNome;
            await ProdutoService.Adicionar(_mapper.Map<Produto>(ProdutoDTO));

            return CustomResponse(ProdutoDTO);
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id, ProdutoDTO ProdutoDTO)
        {
            if (id != ProdutoDTO.Id)
            {
                NotificarErro("Os ids informados não são iguais!");
                return CustomResponse();
            }

            var produtoAtualizacao = await ObterProduto(id);
            ProdutoDTO.Imagem = produtoAtualizacao.Imagem;
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (ProdutoDTO.ImagemUpload != null)
            {
                var imagemNome = Guid.NewGuid() + "_" + ProdutoDTO.Imagem;
                if (!UploadArquivo(ProdutoDTO.ImagemUpload, imagemNome))
                {
                    return CustomResponse(ModelState);
                }

                produtoAtualizacao.Imagem = imagemNome;
            }

            produtoAtualizacao.Nome = ProdutoDTO.Nome;
            produtoAtualizacao.Descricao = ProdutoDTO.Descricao;
            produtoAtualizacao.Valor = ProdutoDTO.Valor;
            produtoAtualizacao.Ativo = ProdutoDTO.Ativo;

            await ProdutoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

            return CustomResponse(ProdutoDTO);
        }

        [Authorize]
        [HttpPost("Adicionar")]
        public async Task<ActionResult<ProdutoDTO>> AdicionarAlternativo(ProdutoImagemDTO ProdutoDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imgPrefixo = Guid.NewGuid() + "_";
            if (!await UploadArquivoAlternativo(ProdutoDTO.ImagemUpload, imgPrefixo))
            {
                return CustomResponse(ModelState);
            }

            ProdutoDTO.Imagem = imgPrefixo + ProdutoDTO.ImagemUpload.FileName;
            await ProdutoService.Adicionar(_mapper.Map<Produto>(ProdutoDTO));

            return CustomResponse(ProdutoDTO);
        }

        [Authorize]
        [RequestSizeLimit(40000000)]
        //[DisableRequestSizeLimit]
        [HttpPost("imagem")]
        public ActionResult AdicionarImagem(IFormFile file)
        {
            return Ok(file);
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoDTO>> Excluir(Guid id)
        {
            var produto = await ObterProduto(id);

            if (produto == null) return NotFound();

            await ProdutoService.Remover(id);

            return CustomResponse(produto);
        }

        private bool UploadArquivo(string arquivo, string imgNome)
        {
            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            var imageDataByteArray = Convert.FromBase64String(arquivo);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\demo-webapi\\src\\assets", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

            return true;
        }
        private async Task<bool> UploadArquivoAlternativo(IFormFile arquivo, string imgPrefixo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\demo-webapi\\src\\assets", imgPrefixo + arquivo.FileName);

            if (System.IO.File.Exists(path))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return true;
        }


        private async Task<ProdutoDTO> ObterProduto(Guid id)
        {
            return _mapper.Map<ProdutoDTO>(await _produtoRepository.ObterProdutoFornecedor(id));
        }
    }
}