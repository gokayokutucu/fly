# FLY MONGODB WITH CACHING SAMPLE
This application is a composite service app that presents shopping product transactions and, that caches the data on redis instantly.

Start the applications with docker compose:

* docker-compose -up

NOTE: If you have any problem, please externally run the redis and mongo container and then start the web api app with `dotnet run` command manually.