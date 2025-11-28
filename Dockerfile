FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY Conference.csproj ./

RUN dotnet restore "Conference.csproj"

COPY . .
WORKDIR /src
RUN dotnet publish "Conference.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Conference.dll"]