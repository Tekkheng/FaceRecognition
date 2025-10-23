FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY FaceLiveness.Api/FaceLiveness.Api.csproj FaceLiveness.Api/
COPY FaceLiveness.Application/FaceLiveness.Application.csproj FaceLiveness.Application/
COPY FaceLiveness.Infrastructure/FaceLiveness.Infrastructure.csproj FaceLiveness.Infrastructure/
COPY FaceLiveness.Domain/FaceLiveness.Domain.csproj FaceLiveness.Domain/
RUN dotnet restore FaceLiveness.Api/FaceLiveness.Api.csproj

COPY . .
WORKDIR /src/FaceLiveness.Api
RUN dotnet publish FaceLiveness.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FaceLiveness.Api.dll"]
