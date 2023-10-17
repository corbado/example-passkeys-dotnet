# Passkey integration example for ASP.NET Core

This is a sample implementation of a ASP.NET Core application that offers passkey authentication. For simple passkey-first authentication, the Corbado web component is used.

## File structure

```
├── corbado-demo
|   ├── api
|   |   ...
|   |   └── Pages
|   |       ├── Index.cshtml      # The page which shows info about your profile when logged in
|   |       └── Login.cshtml      # The login page
|   |   
|   └── Properties
|       └── launchSettings.json   # Contains environment variables
```

## Prerequisites

Please follow the steps in [Getting started](https://docs.corbado.com/overview/getting-started) to create and configure
a project in the [Corbado developer panel](https://app.corbado.com/signin#register).

Open the launchSettings.json file and enter your projectID in all configurations.

## Usage

Open the solution in Visual Studio and run the project (Hit the "play" button or go to "Debug -> Start Debugging")

