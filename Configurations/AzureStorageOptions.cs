namespace PotyRetiradasApi.Configurations;

public sealed class AzureStorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public string ContainerName { get; set; } = "potyretiradas";
}