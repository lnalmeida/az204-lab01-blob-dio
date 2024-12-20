// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices.JavaScript;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class Program
{
    //nome da storage account
    private const string storageAccountName = "staaz204lab0blob";
    //endereço do endpoint do serviço de blob na storage account
    private const string blobServiceEndpoint = $"https://{storageAccountName}.blob.core.windows.net";
    //chave primária da storage account
    private const string storageAccountKey = "<Inser your account key>";

    //inivia o código criando a função assíncrona Main
    public static async Task Main(string[] args)
    {
        //cria uma nova instância da classe StorageSharedKeyCredential
        StorageSharedKeyCredential accountCredentials = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);
        //Cria uma nova instancia de BlobServiceClient
        BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobServiceEndpoint), accountCredentials);
        //invoca GetAccountInfoAsync de BlobServiceClient(retorna informações sobre a storage account
        AccountInfo info = await blobServiceClient.GetAccountInfoAsync();
        //envia uma mensagem de boas vindas
        await Console.Out.WriteLineAsync("Conectado ao Azure Storage Account");
        //imprime o nome da conta
        await Console.Out.WriteLineAsync($"Nome da Conta:\t{storageAccountName}");
        //imprime o tipo da conta
        await Console.Out.WriteLineAsync($"Tipo da Conta:\t{info?.AccountKind}");
        //imprime o SKU da conta
        await Console.Out.WriteLineAsync($"SKU da Conta: {info?.SkuName}");
        await Console.Out.WriteLineAsync("---------------------------------------------------");
        //Cria um container novo
        await createContainerAsync(blobServiceClient, "container-teste");
        await Console.Out.WriteLineAsync("---------------------------------------------------");
        //enumera os containers existentes
        await EnumerateContainerAsync(blobServiceClient);
        await Console.Out.WriteLineAsync("---------------------------------------------------");
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("container-teste");
        await GetBlobAsync(blobContainerClient, "cv-g-sanches.pdf");


        //     await EnumerateBlobAsync(blobServiceClient, "compressed-audio");
        //     await EnumerateBlobAsync(blobServiceClient, "raste-graphics");
    }

    private static async Task EnumerateContainerAsync(BlobServiceClient client)
    {
        //cria um foreach asyncrono que itera sobre o resultado da invocação de GetBlobContainerAsync da classe BlobServiceClient
        await foreach (BlobContainerItem containerItem in client.GetBlobContainersAsync())
        {
            await Console.Out.WriteLineAsync($"Container:\t{containerItem.Name}");
            await EnumerateBlobAsync(client, containerItem.Name);
        }
    }

    private static async Task EnumerateBlobAsync(BlobServiceClient client, string containerName)
    {
        BlobContainerClient containerClient = client.GetBlobContainerClient(containerName);
        await Console.Out.WriteLineAsync($"Buscando:\t{containerClient.Name}");
        
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            await Console.Out.WriteLineAsync($"Blobs encontrados:\t{blobItem.Name}");
        }
    }

    private static async Task<BlobContainerClient> createContainerAsync(BlobServiceClient blobServiceClient, string containerName)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        await Console.Out.WriteLineAsync($"Novo container:\t{containerClient.Name}");
        return containerClient;
    }

    private static async Task<BlobClient> GetBlobAsync(BlobContainerClient client, string blobName)
    {
        BlobClient blob = client.GetBlobClient(blobName);
        bool blobExists = await blob.ExistsAsync();
        if (!blobExists)
        {
            await Console.Out.WriteLineAsync($"Blob \t{blob.Name} não encontrado.");
            
        }
        else
        {
            await Console.Out.WriteLineAsync($"Blob \t{blob.Name} encontrado, URI:\t{blob.Uri}.");
        }
        return blob;
    }
}
