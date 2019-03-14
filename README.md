# Share Your Experience


## Description



## Build Status

#### main

[![Build status](https://dev.azure.com/CQCDigital/SYE-Project/_apis/build/status/SYE-NetCore-Build-Pipeline)](https://dev.azure.com/CQCDigital/SYE-Project/_build/latest?definitionId=2)

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

    Alternatively you can use Docker image of Azure Cosmos DB Emulator**.
  * MSOpenTech’s Redis on Windows you can download from MS Archive [here](https://github.com/MicrosoftArchive/redis/releases).
    
    The default connection host:port [localhost:6379](http://localhost:6379) are used for local development environment unless you supplied different port during installation of Redis.
    
    While this is relatively older version of Redis, it seems sufficient and easiest method to run Redis for development environment.
    Alternatively you can use Docker image of Redis**.

** This is not tested by SYE development team yet.

### Caching
We will be using Redis as cache store.


### Session
We will be using Redis as session store.

For time being we will share same instance of Redis between cache and session.

## Building



## Testing



## Deploying



## Key Contacts