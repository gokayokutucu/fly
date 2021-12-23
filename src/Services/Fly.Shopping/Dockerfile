#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Services/Fly.Shopping/Fly.Shopping.csproj", "src/Services/Fly.Shopping/"]
COPY ["src/Services/FLy.Persistence/FLy.Persistence.csproj", "src/Services/FLy.Persistence/"]
COPY ["src/Services/Fly.Application/Fly.Application.csproj", "src/Services/Fly.Application/"]
COPY ["src/Services/Fly.Domain/Fly.Domain.csproj", "src/Services/Fly.Domain/"]
COPY ["src/Services/Fly.Common/Fly.Common.csproj", "src/Services/Fly.Common/"]
COPY ["src/Services/Fly.Infrastructure/Fly.Infrastructure.csproj", "src/Services/Fly.Infrastructure/"]
RUN dotnet restore "src/Services/Fly.Shopping/Fly.Shopping.csproj"
COPY . .
WORKDIR "/src/src/Services/Fly.Shopping"
RUN dotnet build "Fly.Shopping.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fly.Shopping.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fly.Shopping.dll"]
