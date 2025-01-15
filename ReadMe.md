# Employee Scheduler

This project is a monolithic web application built with .NET and Blazor, adhering to Clean Architecture principles. It facilitates employee scheduling and utilizes a SQL Server database for data persistence.

## Prerequisites

Before running the application, ensure the following are installed and configured:

*   **SQL Server Express:** A local instance of SQL Server Express is required. You can download it from the official Microsoft website.
*   **.NET SDK:** The .NET SDK is necessary for building and running the application. Download the appropriate version from the official .NET website.
*   **Entity Framework Core Tools:** Install the Entity Framework Core global tool:

    ```bash
    dotnet tool install --global dotnet-ef
    ```

## Getting Started

1.  **Clone the Repository:** Clone the project repository to your local development environment:

2.  **Database Setup:** Navigate to the project **Infrastructure** and execute the following command:

    ```bash
    dotnet ef database update
    ```

    This command creates the database and applies any pending migrations. Ensure your connection string in the `appsettings.json` (or equivalent configuration file) is correctly configured to point to your SQL Server instance. Example:

3.  **Build the Solution:** Navigate to the project root directory and execude the following command:

    ```bash
    dotnet build
    ```

4.  **Run the Application:** Navigate to the project directory containing the `.csproj` file of your web application (likely within the Presentation layer) and execute the following command:

    ```bash
    dotnet run
    ```

    This will launch the Blazor application.

## Project Architecture

*   **Architecture:** Monolith
*   **Architectural Pattern:** Clean Architecture
*   **Layers:**
    *   **Presentation Layer**
    *   **Application Layer**
    *   **Domain Layer**
    *   **Infrastructure Layer**
*   **Database:** SQL Server
*   **ORM:** Entity Framework Core (Code-First Approach)