using System;
using System.Threading.Tasks;
using Joao.Business.Models;

namespace Joao.Business.Intefaces
{
    public interface IEnderecoRepository : IRepository<Endereco>
    {
        Task<Endereco> ObterEnderecoPorFornecedor(Guid fornecedorId);
    }
}