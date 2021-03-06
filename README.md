# QuickStart - Config

## Overview
This repo contains code that will help you getting started with creating an application config object that pulls from various sources.

This provides examples on how to connect and configure various services.  See **Configuration Details** below.


## How to Use This Repo

This repository is designed to be as modular as possible so that developers can opt to take as much or as little as they want.  In addition, it is also meant
to explain how and why certain code choices were made.

## Configuration Details

### QuickStart Config Value Locations
Part of the beauty of this solution is that you can store configuration values in a number of different places.  However, for the purposes of the quick start these are where the default
values are located.  Feel free to change them as you see fit.

#### Secrets.json
> - AppId
> - TenantId
> - AppSecret
> - AzureAppConfigConnectionString
> - AzureKeyVaultUri
> - AzureConfigUri

#### Appsettings.json
> - CacheType
> - VerboseLogging
> - LogEntityFrameworkCalls
> - EnableSensitiveDataLogging

#### Azure Config
> - CacheType
> - RedisEndpoint
> - RedisServiceName
> - LogEntityFrameworkCalls
> - EnableSensitiveDataLogging

#### Azure Key Vault
> - ClientId
> - Secret
> - TenantId
> - RedisClientId
> - RedisSecret
> - ConnectionString

### Assemblies Used
A complete breakdown of which assemblies were used for which functionality lives in the [assemblies](Docs/assemblies.md) document.

### Config Folder
The config folder holds the main and sub objects that make up our main configuration object.  The purpose of having this is to make it so all of our configurations,
no matter what the source, all get loaded into this one object.

The **AppConfig** object is hierarchical, it has beneath it several child objects that each represent various verticals of configuration.  For example,
There is a **CacheConfig** object that holds all of the paramters for our cache, as well as a **Database Config** object that has details about any databases we are 
using.

### ARM Templates
In order to make things as easy as possible on developers trying out this technology, I have created a series of ARM templates that will allow you to deploy these instances as easily as possible.
#### Key Vault
[![Deploy To Azure](https://raw.githubusercontent.com/tallan/QuickStartConfig/master/Images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Ftallan%2FQuickStartConfig%2Fmaster%2FARM%2520Templates%2FCreateAndDeployKeyVault.json)
[![Visualize](https://raw.githubusercontent.com/tallan/QuickStartConfig/master/Images/visualizebutton.svg?sanitize=true)](http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2Ftallan%2FQuickStartConfig%2Fmaster%2FARM%2520Templates%2FCreateAndDeployKeyVault.json)

FYI, in the Key Vault ARM template it will ask you for **ObjectId**.  This can be the ID of your AD account which can easily be obtained with the following Powershell Code:
>Connect-AzAccount
>
>Get-AzADUser -Mail [your email address]

## Contributing

This project welcomes [contributions and suggestions](Docs/contribute.md).

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/).



## Issues

If there are any issues or improvements you would like to see in the code please submit an issue through the GitHub issue tracking mechinism.