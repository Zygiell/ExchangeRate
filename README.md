# ExchangeRateAPI
Currencies Exchange Rate Viewer API.
Its a simple API that has two endpoints:
1. Download exchange rates from a date range. (requires authorization)
2. Generate ApiKey for authorization.

ExchangeRateAPI downloads data from Eurpean Central Bank Statistical Data and Metadata eXchange external API:
https://sdw-wsrest.ecb.europa.eu/help/

# Optimalization
Each time the application retrieves data from an external API, the data is saved to the database as "cache", 
so that when the same query is requested in the future, there is no need to query the external api.
Additionally, at first app startup, application seeds database with most common currency pairs for 10 years back.


# How to launch on local machine:
 PRE REQUISITES : .NET 6 FRAMEWORK + IDE (tested on Visual Studio)

 1. Clone repository (master branch).
 2. Set ExchangeRateAPI as a Startup Project.
 3. Provide valid Connection string in appsettings.json "ExchangeRateDbConnection" section.
 4. Open NuGet Package Manager Console
 5. In Package Manager Console set Default project to ExchangeRateAPI
 6. In console use "add-migration MigrationName" command, next use "update-database" command
 7. Launch project
 8. Every function from master branch should be working with SwaggerUI or any other app eg. Postman
 9. To be authorized, make sure you used ApiKey authorization:


 You can do so by using Generate API KEY method on SwaggerUI/any other app like Postman.
 Than with SwaggerUI paste jwtToken to Value field in Authorize function located on top right.
 With apps like Postman, add Authorize header with "Bearer jwtToken".