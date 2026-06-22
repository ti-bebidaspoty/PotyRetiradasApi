namespace PotyRetiradasApi.Services.Interfaces;

public interface IArquivoStorageService
{
    Task<string> UploadImagemBase64Async(
        string imagemBase64,
        string nomeBaseArquivo,
        CancellationToken cancellationToken);
}