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
        var denuncia = _denunciaRepository.FindById(denunciaId);

        if (denuncia is null)
            throw new InvalidOperationException("Denúncia não encontrada.");

        var primeiraVistoria =
            denuncia.Status == StatusDenuncia.EmVistoria;

        var novaVistoria =
            denuncia.Status == StatusDenuncia.AguardandoNovaVistoria;

        if (!primeiraVistoria && !novaVistoria)
        {
            throw new InvalidOperationException(
                "A denúncia não está no status adequado para receber uma vistoria.");
        }

        if (dto.DataVistoria == default)
            throw new InvalidOperationException(
                "A data da vistoria é obrigatória.");

        if (string.IsNullOrWhiteSpace(dto.Relato))
            throw new InvalidOperationException(
                "O relato da vistoria é obrigatório.");

        var numeroVistoria = _vistoriaRepository
            .FindAll()
            .Where(v => v.DenunciaId == denunciaId)
            .Select(v => v.NumeroVistoria)
            .DefaultIfEmpty(0)
            .Max() + 1;

        var vistoria = new Vistoria
        {
            DenunciaId = denunciaId,
            NumeroVistoria = numeroVistoria,
            DataVistoria = dto.DataVistoria,
            Relato = dto.Relato.Trim(),
            IrregularidadeIdentificada = dto.IrregularidadeIdentificada,
            DataCriacao = DateTime.UtcNow
        };

        StatusDenuncia novoStatus;
        ResultadoDenuncia? resultado;

        if (primeiraVistoria)
        {
            novoStatus = vistoria.IrregularidadeIdentificada
                ? StatusDenuncia.AguardandoRegularizacao
                : StatusDenuncia.Encerrada;

            resultado = vistoria.IrregularidadeIdentificada
                ? null
                : ResultadoDenuncia.Improcedente;
        }
        else
        {
            novoStatus = vistoria.IrregularidadeIdentificada
                ? StatusDenuncia.EmProcessoDeInfracao
                : StatusDenuncia.Encerrada;

            resultado = vistoria.IrregularidadeIdentificada
                ? null
                : ResultadoDenuncia.Regularizada;
        }

        if (!DenunciaStatusRules.PodeTransicionar(
                denuncia.Status, novoStatus))
        {
            throw new InvalidOperationException(
                "Não é possível alterar o status da denúncia conforme o resultado da vistoria.");
        }

        _transactionManager.ExecuteInTransaction(() =>
        {
            _vistoriaRepository.Create(vistoria);

            denuncia.Status = novoStatus;
            denuncia.Resultado = resultado;

            _denunciaRepository.Update(denuncia);
        });

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
