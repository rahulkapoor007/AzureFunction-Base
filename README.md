# Azure Functions Base Structure with Clean Architecture

The Azure Function Base empowers developers to rapidly develop, deploy, and manage custom functions tailored to their specific needs. It provides a solid foundation for implementing clean architecture principles in Azure Functions.

## Features

- **Azure Function APIs**: Easily create CRUD operations for various entities.
- **File Upload Support**: Seamlessly handle file uploads in your APIs.
- **Generic Response Handling**: Standardize response structures for consistent client interaction.
- **Middleware Support**: Implement middleware for pre-processing requests or post-processing responses.
- **Request and Response Logging**: Log incoming requests and outgoing responses for improved debugging and monitoring.
- **Global Exception Handling**: Handle exceptions gracefully at a global level for better error management.

## Installation

To install and run the project locally, follow these steps:

1. Clone the repository:
    ```sh
    $ git clone https://github.com/rahulkapoor007/AzureFunction-Base.git
    $ cd AzureFunction.FunctionApp
    ```

2. Restore dependencies:
    ```sh
    $ dotnet restore
    ```

3. Run the application:
    ```sh
    $ dotnet run
    ```

## How to Test

You can test the following Azure Functions:

- **Create User**: `POST http://localhost:7052/api/users`
- **Get Users**: `GET http://localhost:7052/api/users?filter=Shane`
- **Get User by Id**: `GET http://localhost:7052/api/user?Id=2`

### Testing API

A Postman collection (`AzureFunction.Base.postman_collection.json`) is provided in the repository for convenient API testing. 

Generic response structure:
```json
{
    "Data": {
        "Success": true
    },
    "Succeeded": true,
    "MessageNumber": 5003,
    "Message": "Success!",
    "StatusCode": 200
}
```

## Contributing

Contributions are welcome and encouraged! If you encounter any issues or have ideas for improvements, please open an issue. Pull requests are also appreciated. For more detailed guidelines on contributing, please refer to our [Contribution Guide](CONTRIBUTING.md).
