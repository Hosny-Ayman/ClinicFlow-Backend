\# ClinicFlow



A clinic management system backend built with ASP.NET Core Web API.



\## Overview



ClinicFlow is a backend system designed to manage clinic operations,

including doctors, patients, appointments, medical records,

prescriptions, schedules, vacations, and clinic working hours.



The application follows Clean Architecture principles to separate

business logic from infrastructure and API concerns.



\## Features



\- User Authentication

\- JWT Authentication

\- Access and Refresh Tokens

\- Role-Based Authorization

\- Permission-Based Authorization

\- Clinic Management

\- Doctor Management

\- Patient Management

\- Appointment Management

\- Doctor Schedule Management

\- Doctor Vacation Management

\- Clinic Working Hours Management

\- Medical Records Management

\- Prescription Management

\- Specialty Management

\- User Management

\- System Settings

\- Payment Methods Management

\- File Upload and Storage

\- Background Jobs

\- Request Rate Limiting

\- Global Exception Handling

\- API Validation

\- Structured Logging



\## Tech Stack



\### Backend



\- C#

\- .NET 8

\- ASP.NET Core Web API

\- Entity Framework Core

\- SQL Server

\- LINQ



\### Architecture \& Design



\- Clean Architecture

\- Repository Pattern

\- Unit of Work Pattern

\- Specification Pattern

\- Dependency Injection

\- DTOs

\- Service Layer



\### Authentication \& Authorization



\- JWT Bearer Authentication

\- Access Tokens

\- Refresh Tokens

\- Role-Based Authorization

\- Permission-Based Authorization

\- Policy-Based Authorization



\### Validation \& Mapping



\- FluentValidation

\- AutoMapper



\### Background Processing



\- Hangfire

\- SQL Server Storage



\### File Storage



\- Cloudinary



\### Logging



\- Serilog

\- Console Logging

\- File Logging

\- Better Stack



\### API \& Security



\- Swagger / OpenAPI

\- CORS

\- ASP.NET Core Rate Limiting

\- Global Exception Handling



\### Testing



\- xUnit

\- Moq

\- Entity Framework Core InMemory

\- Coverlet



\### Containerization



\- Docker



\## Architecture



The project follows Clean Architecture and is separated into

different layers:



```text

ClinicFlow

│

├── ClinicFlow.Api

│

├── ClinicFlow.Application

│

├── ClinicFlow.Domain

│

├── ClinicFlow.Infrastructure

│

└── ClinicFlow.UnitTests



\### ClinicFlow.Api



Responsible for handling HTTP requests, API endpoints,

middleware, authentication and authorization configuration.



\### ClinicFlow.Application



Contains the application's business logic, services,

DTOs, validators, and application-level abstractions.



\### ClinicFlow.Domain



Contains the core business entities, enums,

and domain-level abstractions.



\### ClinicFlow.Infrastructure



Contains implementations related to data access,

Entity Framework Core, repositories, authentication,

file storage, background jobs, and external services.



\### ClinicFlow.UnitTests



Contains unit tests for the application's business logic

and services.



\## Database



The application uses SQL Server with Entity Framework Core

for data access and database management.



Entity Framework Core migrations are used to manage database

schema changes.



\### Entity Relationship Diagram



The database ERD is available here:



\[View ERD](docs/Clinic-Flow-Erd-mermaid.mmd)



The editable database diagram is also available here:



\[Open Draw.io Diagram](docs/ClinicFlowDigram.drawio)



\## Authentication \& Authorization



The application uses JWT Bearer Authentication to secure protected

API endpoints.



\### Authentication



\- JWT Access Tokens

\- Refresh Tokens

\- Cookie-based token handling



\### Authorization



The authorization system supports:



\- Role-Based Authorization

\- Permission-Based Authorization

\- Policy-Based Authorization



Custom authorization requirements and handlers are used to validate

user permissions for protected resources.



\## Validation \& Mapping



The application uses FluentValidation to validate incoming requests

before processing them.



AutoMapper is used to handle object-to-object mapping between

entities and DTOs.



\## Background Jobs



Hangfire is used to run background jobs in the application.



The application uses background processing for doctor vacation-related

operations, with SQL Server used as the Hangfire storage.



\## File Storage



Cloudinary is used for file storage.



File storage is abstracted behind an application service interface,

allowing the storage implementation to remain separated from the

application's business logic.



\## Logging



The application uses Serilog for structured logging.



Logs are configured to be written to:



\- Console

\- File

\- Better Stack



\## Error Handling



The application uses global exception handling middleware to

centralize unexpected exception handling.



Application operations also use an operation result approach

to represent successful and failed operations in a consistent way.





\## API Documentation



The API is documented using Swagger / OpenAPI.



Swagger provides an interactive interface for exploring and testing

the available API endpoints.



\## Testing



The project includes a dedicated unit testing project:



\- xUnit

\- Moq

\- Entity Framework Core InMemory

\- Coverlet



Unit tests cover different application services and business logic.



\## Docker



The application includes Docker support through a Dockerfile,

allowing the backend to be built and run inside a container.



\## Getting Started



\### Prerequisites



\- .NET 8 SDK

\- SQL Server



\### Clone the Repository



```bash

git clone https://github.com/Hosny-Ayman/ClinicFlow-Backend.git

cd ClinicFlow-Backend





\## Project Structure



```text

ClinicFlow-Backend

│

├── ClinicFlow.Api

│   └── API layer, controllers, middleware, and application configuration

│

├── ClinicFlow.Application

│   └── Business logic, services, DTOs, validators, and specifications

│

├── ClinicFlow.Domain

│   └── Core entities, enums, and domain abstractions

│

├── ClinicFlow.Infrastructure

│   └── Data access, repositories, authentication, file storage,

│       background jobs, and external service implementations

│

├── ClinicFlow.UnitTests

│   └── Unit tests

│

└── docs

&#x20;   ├── Clinic-Flow-Erd-mermaid.mmd

&#x20;   └── ClinicFlowDigram.drawio





\## Live Demo



\### Frontend



\[ClinicFlow Frontend](https://clinicflow-frontend-6upd.onrender.com/home)



\### Backend API



\[ClinicFlow Backend API](https://clinicflow-backend-654e.onrender.com)



\### API Documentation



\[Swagger](https://clinicflow-backend-654e.onrender.com/swagger/index.html)





\## Screenshots



\### Landing Page



!\[ClinicFlow Landing Page](docs/screenshots/landing-page.png)



\### Doctors Management



!\[Doctors Management](docs/screenshots/doctors.png)



\### Appointments Management



!\[Appointments Management](docs/screenshots/appointments.png)



\### Doctor Dashboard



!\[Doctor Dashboard](docs/screenshots/doctor-dashboard.png)



\### Appointment Booking



!\[Appointment Booking](docs/screenshots/booking.png)

