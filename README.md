# Share Your Experience


## Description



## Build Status


#### develop


[![Build status](https://dev.azure.com/CQCDigital/SYE-Project/_apis/build/status/SYE-NetCore-Build-Pipeline?branchName=develop)](https://dev.azure.com/CQCDigital/SYE-Project/_build/latest?definitionId=2)

#### master


[![Build status](https://dev.azure.com/CQCDigital/SYE-Project/_apis/build/status/SYE-NetCore-Build-Pipeline?branchName=master)](https://dev.azure.com/CQCDigital/SYE-Project/_build/latest?definitionId=2)

## Table of contents



## Development and Test Environment

### Framework
  * .Net Core 2.2



### Tools
  * Visual Studio 2017 or Visual Studio Code

  * Azure Cosmos DB Emulator download from [here](https://aka.ms/cosmosdb-emulator).

    Once you install and start the emulator please create databse, collection and populate it with data.
    You can find officual documentation (including limitations) for Cosmos DB emulator [here](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator).
 
    Alternatively you can use Docker image of Azure Cosmos DB Emulator**.

** This is not tested by SYE development team yet.

### Caching


### Session



## Building


## Developing and Testing locally (Debugging locally)

 * If you are in the middle of piece of work that requires access to existing Cosmos Db in cloud, please change "ASPNETCORE_ENVIRONMENT" in launchSettings.json from "Local" to "Development".

## Testing



## Deploying



## Key Contacts