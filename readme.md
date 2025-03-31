# Cloud Setup

## Setup CosmosDB for RBAC authentication

Instead of relying on the connection strings for using or testing your application, it is better to use the tokenCredential generated by the instance DefaultAzureCredential. Follow the next steps to achieve the registration of your user on the CosmosDB.

1. Get the unique identifier of the role definition in the id property.

```az cosmosdb sql role definition list --resource-group "<name-of-existing-resource-group>" --account-name "<name-of-existing-nosql-account>"```

2. Get the unique identifier for your current account.

```az cosmosdb show --resource-group "<name-of-existing-resource-group>" --name "<name-of-existing-nosql-account>" --query "{id:id}"```

3. Create the role in AZ for

```az cosmosdb sql role assignment create -g "<resource_group>" -a "db_account" -d "val_from_step1" --principal-id "your_Entra_id" --scope "val_from_step2"```

4. Verfiy assignment
```az cosmosdb sql role assignment list -g "resource_group" -a "db_account"```

*Note*: for more information visit [MS Learn](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/security/how-to-grant-data-plane-role-based-access?tabs=built-in-definition%2Ccsharp&pivots=azure-interface-cli#assign-a-system-assigned-managed-identity-to-a-function-app).

# Setup Local Environment


## Running Blob Containers locally

### Azurite

This will help you to run an emulator of a blob storage locally in your PC.

- [Download Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio-code%2Cblob-storage#configure-azurite-extension-settings)
```npm install -g azurite```

- Start azurite

````azurite --silent --location c:\azurite --debug c:\azurite\debug.log````

### Azure Data Explorer

With this tool you will be able to manage the containers used for your local test.

- [Download Azura Data Explorere](https://azure.microsoft.com/en-us/products/storage/storage-explorer#Download-4)

---

Learn more about blob-storage functions at [Microsoft Learn](https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob-trigger?tabs=python-v2%2Cisolated-process%2Cnodejs-v4%2Cextensionv5&pivots=programming-language-csharp)

### Foot Note 

- Version of Newtonsoft to avoid: versions before [13.0.1](https://www.cvedetails.com/vulnerability-list/vendor_id-34310/product_id-167663/Newtonsoft-Json.net.html)