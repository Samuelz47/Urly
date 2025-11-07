# Urly - API Encurtadora de URLs de Alta Performance

![.NET 9](https://img.shields.io/badge/.NET-9.0-purple?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-blue?logo=csharp)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue?logo=postgresql)
![Redis](https://img.shields.io/badge/Redis-Cache-red?logo=redis)
![Status](https://img.shields.io/badge/status-conclu%C3%ADdo-brightgreen)

**Urly** é um projeto de back-end que demonstra a construção de uma API de encurtador de URLs robusta, escalável e segura, construída com .NET 9 e princípios de Clean Architecture.

Este projeto vai além de um simples CRUD, implementando funcionalidades de nível profissional essenciais para uma aplicação de alta performance, incluindo cache distribuído com Redis e segurança com Rate Limiting.

---

## 🚀 API Ao Vivo (Deploy)

A API está deployada na nuvem usando Render (para o Docker) e Neon (para o PostgreSQL).

**Pode testar a API ao vivo aqui:**
### [https://urly-xbon.onrender.com](https://urly-xbon.onrender.com)

*(**Nota:** A instância gratuita do Render pode "adormecer". O primeiro carregamento pode demorar até 60 segundos para "acordar" o servidor!)*

---

## ✨ Funcionalidades Principais (O "Pulo do Gato")

* **Clean Architecture:** O projeto é estritamente separado em camadas (`Domain`, `Application`, `Infrastructure`, `API`).
* **Geração de Códigos Únicos:** Utiliza `RandomNumberGenerator` (seguro) e um alfabeto de 62 caracteres. Inclui lógica de verificação de colisão (`IsCodeUniqueAsync`) para garantir 100% de unicidade.
* **Cache de Alta Performance (Redis):** O *endpoint* de redirecionamento (`GET /api/{code}`) usa um padrão "Cache-Aside" com **Redis** (`IDistributedCache`). Se o link estiver no cache, o retorno é instantâneo, sem tocar na base de dados.
* **Segurança (Rate Limiting):** A API está protegida contra abuso com um Rate Limiter por IP (`fixed-by-ip`), retornando `HTTP 429` se o limite for excedido.
* **Analytics de Cliques:** A API regista cada redirecionamento (`UrlClick`) e expõe um *endpoint* de estatísticas (`GET /api/{code}/stats`).
* **Testes Unitários:** Cobertura de testes completa para a camada de `Application` (lógica de negócio) e `API` (controladores) usando xUnit e Moq.
* **Interface de Teste:** Inclui um front-end simples (`index.html` e `stats.html`) para demonstrar a funcionalidade.

## 🛠️ Tecnologias Utilizadas

* **.NET 9** (C# 12)
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **PostgreSQL** (Base de Dados principal, hosteada no Neon)
* **Redis** (Cache Distribuído, hosteado no Redis.com)
* **xUnit & Moq** (Testes Unitários)
* **AutoMapper** (Mapeamento de DTOs)
* **Docker** (Container para o deploy)
* **Render.com** (Hosting da API)

## 🚀 Como Executar Localmente

### Pré-requisitos
1.  [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
2.  Um servidor PostgreSQL (ex: [Neon](https://neon.tech/) gratuito ou um local)
3.  Um servidor Redis (ex: [Redis.com](https://redis.com/try-free/) gratuito ou um local)
4.  Git

### 1. Clonar o Repositório
```bash
git clone [https://github.com/Samuelz47/Urly.git](https://github.com/Samuelz47/Urly.git)
cd Urly