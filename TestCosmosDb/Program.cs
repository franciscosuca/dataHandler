using System;

partial class Program
{
    static async Task Main()
    {
        Console.WriteLine("Hello, World!");

        try
        {
            CosmosDbService cosmosDbService = new();
            List<Experience> experiences = await cosmosDbService.GetItems("Experience");
            Console.WriteLine("successfully connected to CosmosDB");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}