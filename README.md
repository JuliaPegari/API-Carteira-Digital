````markdown
# 💰 Carteira Digital API

API para gerenciar carteiras digitais e transações financeiras.

---

## 🛠 Tecnologias Utilizadas

- **C#**
- **.NET 6**
- **PostgreSQL**
- **JWT** para autenticação

---

## 🚀 Como Executar

### 1. Configuração do Docker

Certifique-se de que o Docker está instalado e em execução em sua máquina.

Execute o comando para levantar o banco de dados:

```bash
docker-compose up
```
````

### 2. Configuração do Projeto

Com o banco de dados em funcionamento, execute os seguintes comandos para rodar o projeto:

**Restaurar pacotes NuGet:**

```bash
dotnet restore
```

**Rodar o projeto:**

```bash
dotnet run
```

A API estará disponível em:
🔗 `https://localhost:5056`

---

## 📡 Endpoints da API

### 1. 📥 Registrar Usuário

- **Endpoint:** `POST /auth/registrar`
- **Descrição:** Cria um novo usuário no sistema.

**Body:**

```json
{
  "nome": "Alice",
  "email": "alice@example.com",
  "senha": "senha123"
}
```

**Resposta:**

```json
{
  "mensagem": "Usuário registrado com sucesso."
}
```

---

### 2. 🔐 Login

- **Endpoint:** `POST /auth/login`
- **Descrição:** Realiza o login e retorna o token JWT.

**Body:**

```json
{
  "email": "alice@example.com",
  "senha": "senha123"
}
```

**Resposta:**

```json
{
  "token": "jwt_token_aqui"
}
```

---

### 3. 💼 Consultar Saldo da Carteira

- **Endpoint:** `GET /carteiras/saldo`
- **Descrição:** Consulta o saldo da carteira do usuário autenticado.
- **Cabeçalhos:**

  ```
  Authorization: Bearer <jwt_token_aqui>
  ```

**Resposta:**

```json
{
  "saldo": 1000
}
```

---

### 4. ➕ Adicionar Saldo à Carteira

- **Endpoint:** `POST /carteiras/adicionar-saldo`
- **Descrição:** Adiciona saldo à carteira do usuário autenticado.

**Body:**

```json
{
  "valor": 100
}
```

**Resposta:**

```json
{
  "mensagem": "Saldo: XX"
}
```

---

### 5. 🔁 Criar Transferência

- **Endpoint:** `POST transacao/transferir`
- **Descrição:** Cria uma transferência entre a carteira do usuário autenticado e outra carteira.

**Body:**

```json
{
  "carteiraDestinoId": 2,
  "valor": 50
}
```

**Resposta:**

```json
{
  "mensagem": "Transferência realizada com sucesso."
}
```

---

### 6. 📜 Listar Transferências

- **Endpoint:** `GET /transacao/listar`
- **Descrição:** Lista todas as transferências realizadas pelo usuário autenticado. É possível filtrar por período de data.
- **Cabeçalhos:**

  ```
  Authorization: Bearer <jwt_token_aqui>
  ```

- **Parâmetros de consulta (opcionais):**

  - `dataInicio`: Data de início do filtro (formato `yyyy-MM-dd`)
  - `dataFim`: Data de fim do filtro (formato `yyyy-MM-dd`)

**Resposta:**

```json
[
    {
        "tipo": "Enviada",
        "de": "teste",
        "para": "joao",
        "valor": 10,
        "data": "07/05/2025 21:21"
    },...
]
```

---

## 🧪 Testes

Para rodar os testes automatizados:

```bash
dotnet test
```

---

## 📂 Scripts e Seed

A aplicação possui um script que popula o banco de dados com dados fictícios na primeira execução, via `DbSeeder`.

---
