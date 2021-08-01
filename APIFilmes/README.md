# Filmes REST API
Uma API simples para cadastro e consulta de g�neros, filmes e loca��es.

Com ela � poss�vel efetuar as seguintes a��es:

- **Cadastro de G�neros de filme**
	- Inserir novos g�neros (ex.: a��o, com�dia, romance)
	- Atualizar g�neros existentes
	- Inativar um g�nero existente
	- Excluir g�neros

- **Cadastro de Filmes**
	- Inserir novos filmes vinculados a um g�nero existente
	- Atualizar um filme existente
	- Inativar um filme existente
	- Excluir um filme espec�fico
	- Excluir filmes em lote

- **Gerenciamento de usu�rios**
	- Registrar novos usu�rios para a API
	- Efetuar login de usu�rios existentes
	- Autentica��o (gera��o de token utilizado para as opera��es)

## Ambiente para Desenvolvimento e Deploy da API
Para conseguir visualizar o projeto, editar, compilar e fazer o deploy, � necess�rio ter o seguinte ambiente em seu computador:

- Microsoft Visual Studio 2019
- Microsoft SQL Server 2019
- SDK do .NET5 (dispon�vel em https://dotnet.microsoft.com/download/visual-studio-sdks?utm_source=getdotnetsdk&utm_medium=referral)

A API foi desenvolvida usando o padr�o Code-First e faz uso de Migrations do EF Core para gerenciamento da base de dados. Para criar o banco de dados inicial da aplica��o, ap�s abrir a mesma no Visual Studio, edite o arquivo de configura��es apontando para sua base de dados (local ou remota) e execute no Package Manager Console o comando "Update-Database", isso ir� criar a estrutura inicial do banco de dados.

## Acessando os recursos da API
Todas as requisi��es para a API devem ter a URL como essa:
><https://minhaurl.com.br/api/v1/>

Substitua de acordo com sua necessidade, **mas n�o altere a parte final** <code>/api/v1/</code>, caso contr�rio a API n�o ir� responder aos recursos solicitados.

Ap�s registrar seu usu�rio e/ou autenticar na API, todas as requisi��es devem ser enviadas com um Bearer Token nos headers HTTP, usando o token que foi gerado no momento da sua autentica��o.

## Requests e Responses da API
Toda a comunica��o com a API se d� atrav�s de objetos JSON, tanto para a manipula��o dos recursos, quanto para os retornos informados. Abaixo, alguns exemplos com a estrutura dos JSON a serem enviados e recebidos.

### Registrar um novo usu�rio
- M�todo: POST
- Recurso: <code>/api/v1/autorizacao/registrar</code>

### Efetuar login
- M�todo: POST
- Recurso: <code>/api/v1/autorizacao/login</code>

### Buscar todos os g�neros cadastrados
- M�todo: GET
- Recurso: <code>/api/v1/generos</code>

### Buscar todos os g�neros com seus filmes vinculados
- M�todo: GET
- Recurso: <code>/api/v1/generos/filmes</code>

### Buscar um g�nero pelo ID
- M�todo: GET
- Recurso: <code>/api/v1/generos/:id</code>

### Cadastrar um novo G�nero
- M�todo: POST
- Recurso: <code>/api/v1/generos</code>

### Atualizar um G�nero
- M�todo: PUT
- Recurso: <code>/api/v1/generos/:id</code>
- **Obs: o ID enviado no corpo da requisi��o deve ser igual ao que for enviado na URL**

### Deletar um G�nero
- M�todo: DELETE
- Recurso: <code>/api/v1/generos/:id</code>

### Inativar um G�nero
- M�todo: PATCH
- Recurso: <code>/api/v1/generos/:id</code>

### Buscar todos os filmes cadastrados
- M�todo: GET
- Recurso: <code>/api/v1/filmes</code>

### Buscar um filme pelo ID
- M�todo: GET
- Recurso: <code>/api/v1/filmes/:id</code>

### Cadastrar um novo Filme
- M�todo: POST
- Recurso: <code>/api/v1/filmes</code>
- **Obs: enviar o campo generoID no corpo da requisi��o, usando um ID v�lido obtivo no endpoint de G�neros**

### Atualizar um Filme
- M�todo: PUT
- Recurso: <code>/api/v1/filmes/:id</code>
- **Obs: o ID enviado no corpo da requisi��o deve ser igual ao que for enviado na URL**

### Deletar um Filme
- M�todo: DELETE
- Recurso: <code>/api/v1/filmes/:id</code>

### Inativar um Filme
- M�todo: PATCH
- Recurso: <code>/api/v1/filmes/:id</code>

### Deletar m�ltiplos filmes
- M�todo: DELETE
- Recurso: <code>/api/v1/filmes/deletemultiplos</code>
- **Obs: enviar no corpo da requisi��o um array de c�digos de filmes**
	- **Exemplo: [1, 2, 5, 7, 9]**

### Cadastrar uma Loca��o
- M�todo: POST
- Recurso: <code>/api/v1/locacoes</code>
