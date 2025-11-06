# Estágio 1: Build (Compilação)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os ficheiros de projeto (.sln e .csproj)
COPY *.sln .

# --- CORREÇÃO AQUI: Removido "src/" de todos os caminhos ---
COPY Urly.API/*.csproj Urly.API/
COPY Urly.Application/*.csproj Urly.Application/
COPY Urly.Domain/*.csproj Urly.Domain/
COPY Urly.Infrastructure/*.csproj Urly.Infrastructure/
COPY Urly.Shared/*.csproj Urly.Shared/

# Copia os projetos de TESTE (agora estão na main!)
COPY Urly.Application.Tests/*.csproj Urly.Application.Tests/
COPY Urly.API.Tests/*.csproj Urly.API.Tests/

# Restaura as dependências
RUN dotnet restore "Urly.sln"

# Copia todo o resto do código-fonte
COPY . .

# Publica a aplicação API em modo Release
# --- CORREÇÃO AQUI: Removido "src/" do WORKDIR ---
WORKDIR "/src/Urly.API"
RUN dotnet publish "Urly.API.csproj" -c Release -o /app/publish

# ---

# Estágio 2: Final (Execução)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Urly.API.dll"]