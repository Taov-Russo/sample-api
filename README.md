# Introduction
This is a software development hiring coding task that implements a minimal backend API using .NET C# and MS SQL for handling tasks and pipelines. This API allows users to create, read, update and delete Tasks and Pipelines. Additionally, users can calculate the average time for a Pipeline and run the Pipeline, which will trigger an external program to execute each Task in sequence and store the Pipeline run time. This readme provides instructions on how to set up and run the project.

## Prerequisites
- Microsoft SQL Server 2019 or later
- Microsoft SQL Server Management Studio
- Postman (or an application Swagger)

## Setup
Execute the scripts located in the scripts folder of the project in SQL Server Management Studio to create a new database called SampleApiDB, tables and a login.

## Authentication
To use the API, you'll need to authenticate your requests using an access token. For this API, you can create a user account by sending a POST request to the /api/v3/users endpoint with the user's login and password in the request body. Once the user is created, they can then authenticate by sending a POST request to the /api/v3/auth endpoint with their login and password. This will return an access token that can be used to make subsequent requests to the API.

#Conclusion
The Sample.Api project provides a minimal backend API implementation for handling tasks and pipelines using .NET C# and MongoDB. The provided API endpoints allow users to create tasks and pipelines, read task average time and pipeline total average time, run pipelines, and store pipeline run time.