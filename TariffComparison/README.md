# TariffComparison

## Overview
The `TariffComparison` project is a .NET 8 application designed to compare and analyze tariff plans. It leverages Azure Functions to provide serverless functionality for reading and comparing tariff data.

## Features
- **Read Tariffs**: Fetches tariff data from an Excel file stored in Azure Blob Storage.
- **Compare Tariffs**: Provides a mechanism to compare different tariff plans.

## Project Structure
- **ReadTariffs**: Reads tariff data from an Excel file using EPPlus and caches the data for efficient access.
- **CompareTariffs**: A placeholder function for comparing tariff plans.
- **Program.cs**: Configures and runs the Azure Functions application.

## Prerequisites
- .NET 8 SDK
- Azure Storage Account
- EPPlus library for Excel file processing

## How to Run
1. Clone the repository.
2. Install the required .NET 8 SDK.
3. Configure the Azure Blob Storage SAS URL in the `ReadTariffs` function.
4. Run the application using the following command:
