# Top Scorers Technical Assignment for Ninety One

## CLI Application

## How To Run

Navigate to the root project directory in your terminal and run the following command:

```console
dotnet run --project TopScorerCLI <Path-To-Csv>
```

Where `<Path-To-Csv>` should be replaced with the path to the CSV you want to evaluate.

If you are using Visual Studio Code, you can also open the command palette and select `Tasks: Run Task` and then select `Run CLI App`. This will then present you with a dialogue to enter the path to the CSV you want to evaluate.

## Overview

This application takes in a CSV file as an argument to complete the following:

1. Read the contents of the CSV.
2. Find the top score in the CSV as well as the names associated with it and print to the command line.
3. Populate a SQLite database with the contents of the CSV called cliDatabse.db. This is created in the root of the project and is shared with the API.

## Assumptions

- This application should just be invoked from the command line with the input file as an argument and doesn't need to take in user inputs.
- Since no IDs were defined, it is assumed that the combination of first name and last name should be unique in the databse.
- The results of this application should only be based on the input file passed to it and not based on the contents of the database.
- The database column names do not need to match the CSV exactly as the whitespace is bad convention in column names.

## Limitations

- Currently the application expects all supplied CSVs to follow `FirstName, LastName, Score` as the format.
- The maintained databse will persist between runs of the application and will require unique names. Should a name already appear in the database, their score will be updated with the latest value.

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

- `GET /api/Score/top-score`: Returns the highest score in the database as well as the names of the people who achieved that score. This isn't returned in the format specified in the assignemnt document as it should be consumed by another application.
- `GET /api/Score/person`: Takes in a first name and surname and returns returns that person's name and thier score if found.
- `POST /api/Score/upload-list`: Completes an upsert into the database using the provided list.
- `POST /api/Score/upload-csv`: Take in a CSV file and completes and upsert into the database.

These are also documented in the scalar page.

This API has been architected to follow a simple Clean Architechture design that implements CQRS and the mediator pattern. While this may be excessive for these requirements, it provides a good foundation that can be built on as the requirements for this API evolve.

### Assumptions

- The CLI application will be run before the API in order to create the database.
- The name search can be case sensitive.

### Limitations

- Combinations of first name and surname have to be unique.
- Names that already appear in the database will have thier scores updated to the latest uploaded value.
- There is a lot of repeated code between the CLI application and API. This was done to isolate the responsibilites of the two applications and as a future enhancement could be brought in and tested in the API projects.

## Cloud Hosting

Assuming hosting on Azure. Create bicep files for IaaS. Host in Azure App Service. Provies setup to Entra for security

TODO: Add Details

## Security

TODO: Add Details

TODO: Test with large file

## Tests

Run the following command from the root directory to run the tests:

```console
dotnet test
```

### Additional Notes

While currently disabled, this API can make use of a code first approach for the database. Migrations can be run on application startup and can be created with the following command:

```console
dotnet ef migrations add InitialMigration --output-dir Persistence/Migrations --project TopScoreApi.Infrastructure --context ApplicationDbContext --startup-project TopScoreApi.Web
```

## Software Recommendations

Since SQLite runs in the application and commits the database to a file, I recommend using [SQLite Browser](https://sqlitebrowser.org/) to view the contents of the database.
