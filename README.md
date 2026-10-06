# Top Scorers Technical Assignment for Ninety One

## CLI Application

## How to run

Navigate to the root project directory in your terminal and run the following command:

```console
dotnet run --project TopScorerCLI <Path-To-Csv>
```

Where <Path-To-Csv> should be replaces with the CSV you want to evaluate.

If you are using Visual Studio Code, you can also open the command palette and select `Tasks: Run Task` and then select `Run CLI App`. This will then present you with a dialogue to enter the path to the CSV you want to evaluate.

## Overview

This application takes in a CSV file as an argument a to complete the following:

1. Read the contents of the CSV.
2. Find the top score in the CSV as well as the names associated with it.
3. Populate a SQLite databse with the contents of the CSV.

## Limitations

- Currently the application expects all supplied CSVs to follow `FirstName, LastName, Score` as the format.
- The maintained databse will persist between runs of the application and will allow for duplicate entries.

### CLI Application

[CLI Instructions]

#### Instructions

How to run this guy

#### Assumptions

### API

[API Instructions]

## Technologies Used

[Technologies]

dotnet ef migrations add InitialMigration --output-dir Persistence/Migrations --project TopScoreApi.Infrastructure --context ApplicationDbContext --startup-project TopScoreApi.Web
