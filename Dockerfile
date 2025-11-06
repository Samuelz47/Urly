# Estágio 1: Build (Compilação)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os ficheiros de projeto (.sln e .csproj)
COPY *.sln .
COPY src/Urly.API/Urly.API.csproj src/Urly.API/
COPY src/Urly.Application/Urly.Application.csproj src/Urly.Application/
COPY src/Urly.Domain/Urly.Domain.csproj src/Urly.Domain/
COPY src/Urly.Infrastructure/Urly.Infrastructure.csproj src/Urly.Infrastructure/
COPY src/Urly.Shared/Urly.Shared.csproj src/Urly.Shared/

# Copia os projetos de TESTE (necessários para o 'restore')
COPY src/Urly.Application.Tests/Urly.Application.Tests.csproj src/Urly.Application.Tests/
COPY src/Urly.API.Tests/Urly.API.Tests.csproj src/Urly.API.Tests/

# Restaura as dependências
RUN dotnet restore "Urly.sln"

# Copia todo o resto do código-fonte
COPY . .

# Publica a aplicação API em modo Release
WORKDIR "/src/src/Urly.API"
RUN dotnet publish "Urly.API.csproj" -c Release -o /app/publish

# ---

# Estágio 2: Final (Execução)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expõe a porta que o ASP.NET Core escuta por defeito
EXPOSE 8080

# Comando para iniciar a API
ENTRYPOINT ["dotnet", "Urly.API.dll"]