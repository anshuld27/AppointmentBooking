# Appointment Booking Application

## Project Description
The Appointment Booking Application is a .NET 8 application that allows users to book appointments with sales managers. The application provides functionalities to query available time slots based on various criteria such as date, products, language, and customer rating.

## Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or any other compatible IDE
- PostgresSQL or any other compatible database

## Setup Instructions

### 1. Clone the Repository

git clone https://github.com/your-repository/appointment-booking.git 
cd appointment-booking

### 2. Configure the Database
Update the `appsettings.json` file with your database connection string:

{ "ConnectionStrings": { "DefaultConnection": "YourDatabaseConnectionString" } }

### 3. Build the Project
Build the project to restore the dependencies and compile the code:
dotnet build

## Running the Application

### 1. Run the Application
To run the application, use the following command:

dotnet run --project AppointmentBooking/AppointmentBooking.csproj

The application will start and be accessible at `http://localhost:3000` and `https://localhost:3001`.

### 2. Running the Unit Tests
To run the tests, use the following command:
dotnet test

This will execute all the unit tests and display the results in the terminal or command prompt.

## Project Structure
- **AppointmentBooking**: The main application project.
- **AppointmentBooking.Tests**: The test project containing unit tests for the application.
- **AppointmentBooking/Controllers**: Contains the API controllers.
- **AppointmentBooking/Services**: Contains the service classes for business logic.
- **AppointmentBooking/DTOs**: Contains the Data Transfer Objects (DTOs) used in the application.
- **AppointmentBooking/Models**: Contains the entity models representing the database tables.
- **AppointmentBooking/Data**: Contains the database context and migration files.
- **AppointmentBooking/Middleware**: Contains the middleware components for the application.

## Additional Information
- **Logging**: The application uses `ILogger` for logging errors and information.
- **Entity Framework Core**: The application uses Entity Framework Core for database operations.
- **In-Memory Database**: The test project uses an in-memory database for isolated testing.

## Contributing
If you would like to contribute to this project, please fork the repository and submit a pull request with your changes.

