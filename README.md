# Passkey integration example for .NET Core

This is a sample implementation of a .NET Core application that offers passkey authentication. For simple passkey-first authentication, the Corbado web component is used.

## File structure

```
├── corbado-demo
|   ├── api
|   |   ...
|   |   └── Pages
|   |       ├── Index.cshtml      # Configuration of the authentication providers
|   |       └── Login.cshtml      # Endpoint which requests an association token from Corbado
|   |   
|   └── Properties
|       └── launchSettings.json   # Contains environment variables
```

## Prerequisites

Please follow the steps in [Getting started](https://docs.corbado.com/overview/getting-started) to create and configure
a project in the [Corbado developer panel](https://app.corbado.com/signin#register).

Open the launchSettings.json file and enter your projectID in all configurations.

## Usage

Open the solution is visual studio and run the project

