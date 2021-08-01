# Filmes REST API
Uma API simples para cadastro e consulta de gêneros, filmes e locações.

Com ela é possível efetuar as seguintes ações:

- **Cadastro de Gêneros de filme**
	- Inserir novos gêneros (ex.: ação, comédia, romance)
	- Atualizar gêneros existentes
	- Inativar um gênero existente
	- Excluir gêneros

- **Cadastro de Filmes**
	- Inserir novos filmes vinculados a um gênero existente
	- Atualizar um filme existente
	- Inativar um filme existente
	- Excluir um filme específico
	- Excluir filmes em lote

- **Gerenciamento de usuários**
	- Registrar novos usuários para a API
	- Efetuar login de usuários existentes
	- Autenticação (geração de token utilizado para as operações)

## Ambiente para Desenvolvimento e Deploy da API
Para conseguir visualizar o projeto, editar, compilar e fazer o deploy, é necessário ter o seguinte ambiente em seu computador:

- Microsoft Visual Studio 2019
- Microsoft SQL Server 2019
- SDK do .NET5 (disponível em https://dotnet.microsoft.com/download/visual-studio-sdks?utm_source=getdotnetsdk&utm_medium=referral)

A API foi desenvolvida usando o padrão Code-First e faz uso de Migrations do EF Core para gerenciamento da base de dados. Para criar o banco de dados inicial da aplicação, após abrir a mesma no Visual Studio, edite o arquivo de configurações apontando para sua base de dados (local ou remota) e execute no Package Manager Console o comando "Update-Database", isso irá criar a estrutura inicial do banco de dados.

## Acessando os recursos da API
Todas as requisições para a API devem ter a URL como essa:
><https://minhaurl.com.br/api/v1/>

Substitua de acordo com sua necessidade, **mas não altere a parte final** <code>/api/v1/</code>, caso contrário a API não irá responder aos recursos solicitados.

Após registrar seu usuário e/ou autenticar na API, todas as requisições devem ser enviadas com um Bearer Token nos headers HTTP, usando o token que foi gerado no momento da sua autenticação.

## Requests e Responses da API
Toda a comunicação com a API se dá através de objetos JSON, tanto para a manipulação dos recursos, quanto para os retornos informados. Abaixo, alguns exemplos com a estrutura dos JSON a serem enviados e recebidos.

### Registrar um novo usuário
- Método: POST
- Recurso: <code>/api/v1/autorizacao/registrar</code>

### Efetuar login
- Método: POST
- Recurso: <code>/api/v1/autorizacao/login</code>

### Buscar todos os gêneros cadastrados
- Método: GET
- Recurso: <code>/api/v1/generos</code>

### Buscar todos os gêneros com seus filmes vinculados
- Método: GET
- Recurso: <code>/api/v1/generos/filmes</code>

### Buscar um gênero pelo ID
- Método: GET
- Recurso: <code>/api/v1/generos/:id</code>

### Cadastrar um novo Gênero
- Método: POST
- Recurso: <code>/api/v1/generos</code>

### Atualizar um Gênero
- Método: PUT
- Recurso: <code>/api/v1/generos/:id</code>
- **Obs: o ID enviado no corpo da requisição deve ser igual ao que for enviado na URL**

### Deletar um Gênero
- Método: DELETE
- Recurso: <code>/api/v1/generos/:id</code>

### Inativar um Gênero
- Método: PATCH
- Recurso: <code>/api/v1/generos/:id</code>

### Buscar todos os filmes cadastrados
- Método: GET
- Recurso: <code>/api/v1/filmes</code>

### Buscar um filme pelo ID
- Método: GET
- Recurso: <code>/api/v1/filmes/:id</code>

### Cadastrar um novo Filme
- Método: POST
- Recurso: <code>/api/v1/filmes</code>
- **Obs: enviar o campo generoID no corpo da requisição, usando um ID válido obtivo no endpoint de Gêneros**

### Atualizar um Filme
- Método: PUT
- Recurso: <code>/api/v1/filmes/:id</code>
- **Obs: o ID enviado no corpo da requisição deve ser igual ao que for enviado na URL**

### Deletar um Filme
- Método: DELETE
- Recurso: <code>/api/v1/filmes/:id</code>

### Inativar um Filme
- Método: PATCH
- Recurso: <code>/api/v1/filmes/:id</code>

### Deletar múltiplos filmes
- Método: DELETE
- Recurso: <code>/api/v1/filmes/deletemultiplos</code>
- **Obs: enviar no corpo da requisição um array de códigos de filmes**
	- **Exemplo: [1, 2, 5, 7, 9]**

### Cadastrar uma Locação
- Método: POST
- Recurso: <code>/api/v1/locacoes</code>
