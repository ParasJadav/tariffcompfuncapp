# TariffCompFuncApp (.NET) - Technical Documentation

## Overview

The **TariffCompFuncApp (.NET)** is an Azure Function App built using .NET to serve as the backend for the Tariff Comparison platform. It provides HTTP-triggered endpoints to fetch and calculate electricity tariff data based on user input (postal code and annual consumption).

Repository URL: [tariffcompfuncapp (.NET)](https://github.com/ParasJadav/tariffcompfuncapp/new/master)

---

## Technologies Used

- **Azure Functions** (.NET 6/7)
- **C#**
- **HTTP Triggers**
- **Newtonsoft.Json** (for JSON serialization)
- **Dependency Injection**

---

## Features

- **Tariff Calculation**: Computes cost based on annual consumption and base tariff data.
- **Clean Architecture**: Separation of concerns using services and interfaces.
- **Dependency Injection**: Built-in .NET dependency injection system.
- **Environment Configuration**: Uses local.settings.json and Azure App Settings.

---

## Setup Instructions

### Prerequisites

- .NET SDK (6.0 or later)
- Azure Functions Core Tools (v4)
- Visual Studio or VS Code

### Run Locally

```bash
# Clone the repo
$ git clone https://github.com/ParasJadav/tariffcompfuncapp.git
$ cd tariffcompfuncapp

# Start the function app
$ func start
```

## Deployment

You can publish the function app to Azure using:

```bash
$ cd tariffcompfuncapp
run DeployandTest.ps1
```

Or via Visual Studio's "Publish" option.

Ensure the required settings are present in the Azure portal Configuration tab.

---
