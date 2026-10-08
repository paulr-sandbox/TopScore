# Top Scorers Technical Assignment

## CLI Application

## How To Run

Navigate to the root project directory in your terminal and run the following command:

```console
dotnet run --project TopScorerCLI <Path-To-Csv>
```

Where `<Path-To-Csv>` should be replaced with the path to the CSV you want to evaluate. To use the provided test data, provide `docs/TestData.csv` as the argument.

If you are using Visual Studio Code, you can also open the command palette and select `Tasks: Run Task` and then select `Run CLI App`. This will then present you with a dialogue to enter the path to the CSV you want to evaluate.

## Overview

This application takes in a CSV file as an argument to complete the following:

1. Read the contents of the CSV.
2. Find the top score in the CSV as well as the names associated with it and print to the command line.
3. Populate a SQLite database with the contents of the CSV called cliDatabase.db. This is created in the root of the project and is shared with the API.

Evidence that this application meets the base requirements for this assignment can be seen in `TestScoresTests.cs`.

## Assumptions

- This application should just be invoked from the command line with the input file as an argument and doesn't need to take in user inputs.
- Since no IDs were defined, it is assumed that the combination of first name and last name should be unique in the database.
- The results in this application's output should only be based on the input file passed to it and not based on the contents of the database.
- The database column names do not need to match the CSV exactly as the whitespace is bad convention in column names.

## Limitations

- Currently the application expects all supplied CSVs to follow `FirstName, LastName, Score` as the format.
- The maintained database will persist between runs of the application and will require unique names. Should a name combination already appear in the database, their score will be updated with the latest value.

## Api

### How To Run

Navigate to the root project directory in your terminal and run the following command:

```console
dotnet watch --project TopScoreApi.Web --launch-profile https
```

This will launch the API on port 7252. If you have trouble with development certificates, please run `dotnet dev-certs https`. This will open a scalar page with the API documentation that can also be used to test the endpoints.

If you are using Visual Studio Code, you can also open the command palette and select `Tasks: Run Task` and then select `Run API`. This will run the API in the VS Code terminal and open up the scalar documentation.

### Overview

This simple API uses the database created by the CLI application to retrieve and update test scores. The following endpoints are available:

- `GET /api/Score/top-score`: Returns the highest score in the database as well as the names of the people who achieved that score. This isn't returned in the string format specified in the assignment document as it should be consumed by another application.
- `GET /api/Score/person`: Takes in a first name and surname and returns returns that person's name and their score if found.
- `POST /api/Score/upload-list`: Completes an upsert into the database using the provided list.
- `POST /api/Score/upload-csv`: Takes in a CSV file and completes an upsert into the database.

These are also documented in the scalar page.

This API has been architected to follow a simple Clean Architecture design that implements CQRS and the mediator pattern. While this may be excessive for these requirements, it provides a good foundation that can be built on as the requirements for this API evolve.

### Assumptions

- The CLI application will be run before the API in order to create and populate the database.
- The name search can be case sensitive.

### Limitations

- Combinations of first name and surname have to be unique.
- Names that already appear in the database will have their scores updated to the latest uploaded value.
- There is a lot of repeated code between the CLI application and API. This was done to isolate the responsibilities of the two applications and as a future enhancement could be brought in and tested in the API projects.
- This API is currently using standard logging and could benefit from using something like Serilog.

## Cloud Hosting

In order to host this API in the cloud I would first need to move my SQLite database to SQL to allow for better maintainability and scaling. I would then create a React front end for users to interact with the API.

These could then be hosted in Azure in their own app services. I would create a `bicep` template to maintain my infrastructure setup that would define the provisioning of the service plan and the creation of app services for the API and Client, as well as Azure SQL instance for the database. I would then need to create the appropriate pipelines to run my bicep template to create my resources and then deploy my code to their appropriate app services.

This would mostly be sufficient to host the apps, but it hasn't considered any security concerns. For these applications to be appropriately hosted, I would need to implement at least a basic authentication flow as well as define authorisation requirements in the API.

## Security

Since I've recommended hosting this application application in Azure, I would recommend leveraging the Azure tenant's Entra to handle authentication. We will assign users in the tenant to the appropriate Enterprise Application for the Client App Registration and provide them the role of `user` or `administrator`.

Once this has been setup, I can implement the authentication flow for the client app that uses the Microsoft Authentication Library (MSAL). This flow would follow the client app authenticating the user against Azure and providing them a valid Azure token. I would then add a simple JWT bearer authentication scheme to the API that accepts the Azure token. The appropriate role scopes would also need ot be configured in the API to then handle authorisation. Once this is setup, the client app would then include the provide token in the request headers against the API.

Now that users are able to authenticate against the API, I would then need to add the appropriate route guards to the controller and can then add the role checks. The `ScoreController` would need the `[Authorize]` attribute to ensure request have an appropriate token in their header and then `[Authorize(Roles = "role")]` to check if the user as the appropriate roles. In this API, I would allow all users to make `GET` requests, but only users with the `administrator` role would be able to make `POST` requests.

## Tests

Run the following command from the root directory to run the tests:

```console
dotnet test
```

## Additional Notes

While currently disabled, this API can make use of a code first approach for the database. Migrations can be run on application startup and can be created with the following command:

```console
dotnet ef migrations add InitialMigration --output-dir Persistence/Migrations --project TopScoreApi.Infrastructure --context ApplicationDbContext --startup-project TopScoreApi.Web
```

## Software Requirements

In order to run these applications you will need .net 10 installed.

Since SQLite runs in the application and commits the database to a file, I recommend using [SQLite Browser](https://sqlitebrowser.org/) to view the contents of the database.
