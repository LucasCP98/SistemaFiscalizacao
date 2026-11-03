
using SistemaFiscalizacao.Application.DTOs.Vistoria;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Domain.Rules;
using SistemaFiscalizacao.Infrastructure.Repositories;
using SistemaFiscalizacao.Infrastructure.Transactions;

namespace SistemaFiscalizacao.Application.Services.Implementation;

public class VistoriaServiceImplementation : IVistoriaService
{
    private readonly IRepository<Denuncia> _denunciaRepository;
    private readonly IRepository<Vistoria> _vistoriaRepository;
    private readonly ITransactionManager _transactionManager;

    public VistoriaServiceImplementation(
        IRepository<Denuncia> denunciaRepository,
        IRepository<Vistoria> vistoriaRepository,
        ITransactionManager transactionManager)
    {
        _denunciaRepository = denunciaRepository;
        _vistoriaRepository = vistoriaRepository;
        _transactionManager = transactionManager;
    }

    public VistoriaDTO Create(long denunciaId, CreateVistoriaDTO dto)
    {
        // Verifica se a denúncia existe
        var denuncia = _denunciaRepository.FindById(denunciaId);

        if (denuncia is null)
            throw new InvalidOperationException("Denúncia não encontrada.");

        // Verifica se a denúncia está na etapa correta
        if (denuncia.Status != StatusDenuncia.EmVistoria)
            throw new InvalidOperationException(
                "A denúncia não está no status adequado para receber uma vistoria.");

        // Valida os dados recebidos
        if (dto.DataVistoria == default)
            throw new InvalidOperationException("A data da vistoria é obrigatória.");

        if (string.IsNullOrWhiteSpace(dto.Relato))
            throw new InvalidOperationException("O relato da vistoria é obrigatório.");

        // Cria a entidade
        var vistoria = new Vistoria
        {
            DenunciaId = denunciaId,
            DataVistoria = dto.DataVistoria,
            Relato = dto.Relato,
            IrregularidadeIdentificada = dto.IrregularidadeIdentificada,
            DataCriacao = DateTime.UtcNow
        };

        // Define o novo status conforme o resultado da vistoria
        var novoStatus = vistoria.IrregularidadeIdentificada
            ? StatusDenuncia.AguardandoRegularizacao
            : StatusDenuncia.Encerrada;

        var resultado = vistoria.IrregularidadeIdentificada
            ? (ResultadoDenuncia?)null
            : ResultadoDenuncia.Improcedente;

        // Valida se a transição de status é permitida
        if (!DenunciaStatusRules.PodeTransicionar(denuncia.Status, novoStatus))
        {
            throw new InvalidOperationException(
                "Não é possível alterar o status da denúncia conforme o resultado da vistoria.");
        }

        // Persiste a vistoria e atualiza a denúncia na mesma transação
        _transactionManager.ExecuteInTransaction(() =>
        {
            _vistoriaRepository.Create(vistoria);

            denuncia.Status = novoStatus;
            denuncia.Resultado = resultado;

            _denunciaRepository.Update(denuncia);
        });

        // Retorna o DTO
        return new VistoriaDTO
        {
            Id = vistoria.Id,
            DenunciaId = vistoria.DenunciaId,
            DataVistoria = vistoria.DataVistoria,
            Relato = vistoria.Relato,
            IrregularidadeIdentificada = vistoria.IrregularidadeIdentificada,
            DataCriacao = vistoria.DataCriacao
        };
    }
}
