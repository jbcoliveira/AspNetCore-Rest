using System.Collections.Generic;
using Joao.Business.Notificacoes;

namespace Joao.Business.Intefaces
{
    public interface INotificador
    {
        bool TemNotificacao();
        List<Notificacao> ObterNotificacoes();
        void Handle(Notificacao notificacao);
    }
}