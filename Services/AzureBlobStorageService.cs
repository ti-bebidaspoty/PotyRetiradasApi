using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using PotyRetiradasApi.Configurations;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Services;

public sealed class AzureBlobStorageService : IArquivoStorageService
{
    private const long TamanhoMaximoBytes = 5 * 1024 * 1024;

    private readonly AzureStorageOptions _options;

    public AzureBlobStorageService(IOptions<AzureStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> UploadImagemBase64Async(
        string imagemBase64,
        string nomeBaseArquivo,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(imagemBase64))
        {
            throw new ArgumentException("Imagem em base64 vazia.");
        }

        var match = Regex.Match(
            imagemBase64,
            @"^data:(?<mime>[\w\/\-\.+]+);base64,(?<data>.+)$",
            RegexOptions.Singleline);

        if (!match.Success)
        {
            throw new ArgumentException(
                "Formato Base64 inválido. Esperado 'data:<mime>;base64,...'.");
        }

        var mimeType = match.Groups["mime"].Value;
        var base64Data = match.Groups["data"].Value;

        var extensao = ObterExtensaoImagem(mimeType);

        if (string.IsNullOrEmpty(extensao))
        {
            throw new ArgumentException(
                "Formato de imagem inválido. Use jpeg, png, webp, gif ou bmp.");
        }

        byte[] bytes;

        try
        {
            bytes = Convert.FromBase64String(base64Data);
        }
        catch (FormatException)
        {
            throw new ArgumentException("Conteúdo Base64 inválido.");
        }

        if (bytes.Length > TamanhoMaximoBytes)
        {
            throw new ArgumentException(
                "A imagem deve possuir no máximo 5 MB.");
        }

        var containerClient = new BlobContainerClient(
            _options.ConnectionString,
            _options.ContainerName);

        await containerClient.CreateIfNotExistsAsync(
            PublicAccessType.None,
            cancellationToken: cancellationToken);

        var nomeSeguro = CriarNomeSeguro(nomeBaseArquivo);

        var nomeBlob =
            $"produtos/{nomeSeguro}-{Guid.NewGuid():N}{extensao}";

        var blobClient = containerClient.GetBlobClient(nomeBlob);

        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = mimeType,
                ContentDisposition = $"inline; filename=\"{Path.GetFileName(nomeBlob)}\""
            },
            Metadata = new Dictionary<string, string>
            {
                ["originalName"] = nomeSeguro,
                ["uploadedAt"] = DateTime.UtcNow.ToString("o")
            }
        };

        using var stream = new MemoryStream(bytes, writable: false);

        await blobClient.UploadAsync(
            stream,
            uploadOptions,
            cancellationToken);

        return blobClient.Uri.ToString();
    }

    private static string ObterExtensaoImagem(string mimeType)
    {
        return mimeType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/webp" => ".webp",
            "image/bmp" => ".bmp",
            _ => string.Empty
        };
    }

    private static string CriarNomeSeguro(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return "produto";
        }

        nome = Path.GetFileNameWithoutExtension(nome).Trim();

        nome = nome.Normalize(NormalizationForm.FormD);

        var sb = new StringBuilder();

        foreach (var ch in nome)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(ch);

            if (categoria != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(ch);
            }
        }

        nome = sb.ToString().Normalize(NormalizationForm.FormC);

        foreach (var c in Path.GetInvalidFileNameChars())
        {
            nome = nome.Replace(c, '_');
        }

        nome = Regex.Replace(nome, @"\s+", "_");
        nome = Regex.Replace(nome, @"[^a-zA-Z0-9_\-]", "_");

        if (nome.Length > 80)
        {
            nome = nome[..80];
        }

        return string.IsNullOrWhiteSpace(nome)
            ? "produto"
            : nome.ToLowerInvariant();
    }
}