FROM mcr.microsoft.com/dotnet/sdk:10.0 AS base
WORKDIR /app
EXPOSE 8080

COPY *.csproj ./
RUN dotnet restore

COPY . .

ENTRYPOINT ["dotnet", "watch", "run", "--project", "/app/TestDas.csproj", "--non-interactive", "--urls", "http://0.0.0.0:8080"]