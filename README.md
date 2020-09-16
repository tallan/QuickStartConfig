# QuickStart - Config

## Overview
This repo contains code that will help you getting started with creating an application config object that pulls from various sources.

This provides examples on how to connect and configure various services.  See **Configuration Details** below.


## How to Use This Repo

This repository is designed to be as modular as possible so that developers can opt to take as much or as little as they want.  In addition, it is also meant
to explain how and why certain code choices were made.

## Configuration Details

### Assemblies Used
A complete breakdown of which assemblies were used for which functionality lives in the [assemblies](Docs/assemblies.md) document.

### Config Folder
The config folder holds the main and sub objects that make up our main configuration object.  The purpose of having this is to make it so all of our configurations,
no matter what the source, all get loaded into this one object.

The **AppConfig** object is hierarchical, it has beneath it several child objects that each represent various verticals of configuration.  For example,
There is a **CacheConfig** object that holds all of the paramters for our cache, as well as a **Database Config** object that has details about any databases we are 
using.


## Contributing

This project welcomes [contributions and suggestions](Docs/contribute.md).

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/).

## Issues

If there are any issues or improvements you would like to see in the code please submit an issue through the GitHub issue tracking mechinism.