using System;
using System.Threading.Tasks;
using Joao.Business.Intefaces;
using Joao.Business.Models;
using Joao.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Joao.Data.Repository
{
    public class EnderecoRepository : Repository<Endereco>, IEnderecoRepository
    {
        public EnderecoRepository(MeuDbContext context) : base(context) { }

        public async Task<Endereco> ObterEnderecoPorFornecedor(Guid fornecedorId)
        {
            return await Db.Enderecos.AsNoTracking()
                .FirstOrDefaultAsync(f => f.FornecedorId == fornecedorId);
        }
    }
}