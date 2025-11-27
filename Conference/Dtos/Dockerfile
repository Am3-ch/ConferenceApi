FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY Conference.csproj .
RUN dotnet restore "Conference.csproj"
COPY . .
RUN dotnet publish "Conference.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build 
/publish .

ENTRYPOINT [ "dotnet", "Conference.dll" ]