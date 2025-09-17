# Desafio BackEnd - Mottu

## 📌 Descrição
Exemplo de projeto utilizando `Clean Architecture`, `CQRS` e `DDD` para gerenciamento de `locação de motos`.  

### 🔧 Tecnologias utilizadas
- `.NET 8`
- `PostgreSQL 15-alpine`
- `Azure Storage Account / Azurite`
- `RabbitMQ 3-management`
- `Entity Framework Tools`

---

## 🚀 Progresso
- ✅ Regras de negócios (`100%`)  
- ✅ Integração com banco de dados (`100%`)  
- ✅ Testes (`100%`)  
- ⏳ Integração de eventos (`50%`) – pendente finalização com `RabbitMQ`  

---

## 🖥️ Como rodar localmente

### Pré-requisitos
- Ter o `Docker` instalado na máquina

### Passos
1. Clone o repositório  
2. Na raiz do projeto, execute: `docker-compose -f docker-compose.yml up --build -d`  
3. Após subir os containers, acesse a documentação via Swagger: `http://localhost:5000/swagger`

---

## 🧪 Testes

### Testes de Unidade
- Verificação das regras de negócios, sem necessidade de Docker  
- Local do projeto: `test\Bl.Mottu.Maintenance.Core.Tests\Bl.Mottu.Maintenance.Core.Tests.csproj`

### Testes de Integração
- Verificação do comportamento com o banco de dados  
- Local do projeto: `test\Bl.Mottu.Maintenance.Infrastructure.Tests\Bl.Mottu.Maintenance.Infrastructure.Tests.csproj`
