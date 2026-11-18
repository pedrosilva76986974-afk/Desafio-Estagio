# IMDb Challenge API

API REST em .NET 8, com SQL Server, Entity Framework Core Code First, ASP.NET Core Identity, JWT Bearer, Swagger e testes xUnit. Controllers cuidam do HTTP; Services concentram os casos de uso; Domain define entidades e a regra do voto; Data contém persistência e migrations; Security valida credenciais e perfis.

## Executar com Docker

Pré-requisitos: Docker Desktop com engine Linux e Docker Compose v2. O SQL Server usa a imagem de desenvolvimento para x86-64.

1. Copie `.env.example` para `.env`:
   ```powershell
   Copy-Item .env.example .env
   ```
2. Preencha as quatro variáveis em `.env`:

   | Variável | Valor necessário |
   | --- | --- |
   | `SQL_PASSWORD` | Senha forte para SQL Server, com letras maiúsculas/minúsculas, números e símbolos; evite `;` por fazer parte da connection string. |
   | `JWT_SIGNING_KEY` | Segredo aleatório com pelo menos 32 bytes. |
   | `ADMIN_EMAIL` | E-mail do primeiro administrador. |
   | `ADMIN_PASSWORD` | Pelo menos 10 caracteres, maiúscula, minúscula, número e símbolo. |

   Para gerar uma chave aleatória no PowerShell:
   ```powershell
   [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
   ```
   Não publique `.env`. Se um valor contiver `$`, coloque-o entre aspas simples no `.env` para evitar interpolação do Compose.

3. Execute na raiz:
   ```powershell
   docker compose up --build -d
   docker compose logs migrate api
   ```

O Compose aguarda o banco, aplica as migrations, cria o primeiro administrador e inicia a API. O bootstrap é idempotente: uma conta administradora ativa existente é preservada, inclusive sua senha. Ele recusa promover uma conta comum ou reativar uma conta excluída. As credenciais do bootstrap são fornecidas apenas ao serviço `migrate`.

Swagger: http://localhost:8080/swagger — saúde do processo: http://localhost:8080/health. O endpoint de saúde não verifica a disponibilidade do banco.

`docker compose down` encerra os serviços preservando o volume. O banco não publica porta no host. A API publica HTTP somente em localhost; em produção, use HTTPS no servidor ou proxy e credenciais de banco com permissões restritas. A imagem final executa com usuário sem privilégios. A senha `sa` no Compose é exclusiva para a execução local do desafio.

## Executar sem Docker

Instale o SDK .NET 8 e tenha uma instância SQL Server acessível. No PowerShell, configure as variáveis na mesma sessão:

```powershell
$env:ConnectionStrings__DefaultConnection = 'Server=localhost;Database=ImdbChallenge;Integrated Security=True;Encrypt=True;TrustServerCertificate=True'
$env:Jwt__SigningKey = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
$env:BootstrapAdmin__Email = 'admin@example.com'
# Preencha com a senha escolhida para o primeiro administrador:
$env:BootstrapAdmin__Password = 'SUBSTITUA_POR_UMA_SENHA_FORTE'
dotnet restore DESAFIO_ESTAGIO.sln
dotnet run --project DESAFIO_ESTAGIO -- --migrate --bootstrap-admin
dotnet dev-certs https --trust
dotnet run --project DESAFIO_ESTAGIO --launch-profile https
```

A conexão mostrada usa autenticação Windows. Para SQL Authentication, substitua por `User Id` e `Password`. Swagger local: https://localhost:7128/swagger. Para execução HTTP local, configure `$env:HttpsRedirection__Enabled = 'false'` e use `--launch-profile http` (porta 5049). Não há migração automática na inicialização normal da API.

## Autenticação e autorização

1. Entre com `POST /api/auth/login` usando `{ "email": "...", "password": "..." }`.
2. Copie `accessToken`. No Swagger, clique **Authorize** e cole somente o token.
3. Em outros clientes, envie `Authorization: Bearer <accessToken>`.

O token expira em 30 minutos. Assinatura HS256, emissor, audiência e validade são verificados. Cada requisição autenticada também verifica a conta ativa, perfil e security stamp no banco. Atualizar ou excluir a conta invalida tokens anteriores; faça login novamente após uma atualização. Usuários não escolhem seu perfil no cadastro. Senhas usam o hasher do Identity e não são retornadas em respostas.

O login bloqueia a conta por 15 minutos após cinco falhas. Login e cadastro público têm limite de dez requisições por minuto por IP. Atrás de proxy, configure encaminhamento de IP apenas para proxies confiáveis antes de adaptar esse limite.

## Rotas

| Método e rota | Acesso | Comportamento |
| --- | --- | --- |
| `POST /api/auth/login` | Público | Retorna JWT Bearer. |
| `POST /api/users` | Público | Cadastra usuário comum; retorna 201. |
| `GET /api/users/me` | Usuário | Consulta a própria conta. |
| `PUT /api/users/me` | Usuário | Atualiza nome/e-mail próprios. |
| `DELETE /api/users/me` | Usuário | Exclusão lógica própria; retorna 204. |
| `GET /api/users` | Administrador | Lista usuários comuns ativos em ordem de nome. |
| `GET /api/users/{id}` | Administrador | Consulta usuário comum ativo. |
| `PUT /api/users/{id}` | Administrador | Atualiza usuário comum ativo. |
| `DELETE /api/users/{id}` | Administrador | Exclui logicamente usuário comum. |
| `POST /api/administrators` | Administrador | Cria outro administrador; retorna 201. |
| `GET /api/administrators` | Administrador | Lista administradores ativos em ordem de nome. |
| `GET /api/administrators/{id}` | Administrador | Consulta administrador ativo. |
| `PUT /api/administrators/{id}` | Administrador | Atualiza administrador ativo. |
| `DELETE /api/administrators/{id}` | Administrador | Exclusão lógica; último administrador ativo é protegido. |
| `POST /api/movies` | Administrador | Cadastra filme; retorna 201 e Location. |
| `GET /api/movies` | Público | Lista, filtra, ordena e pagina filmes. |
| `GET /api/movies/{id}` | Público | Todos os dados, atores, quantidade e média dos votos. |
| `PUT /api/movies/{id}/vote` | Usuário | Cria ou substitui o próprio voto; retorna 204. |

Corpos de cadastro de conta: `{ "name": "Pedro Gabriel", "email": "pedro@example.com", "password": "..." }`. Atualização usa apenas `name` e `email`.

Exemplo de filme:

```json
{
  "title": "Interestelar",
  "synopsis": "Uma equipe viaja em busca de um futuro para a humanidade.",
  "director": "Christopher Nolan",
  "genre": "Ficção científica",
  "releaseYear": 2014,
  "actors": ["Matthew McConaughey", "Anne Hathaway"]
}
```

Exemplo de voto: `{ "score": 4 }`. O identificador do usuário vem do JWT, sem aceitar outro usuário no corpo. Administradores não votam. A chave composta `(MovieId, UserId)` impede votos duplicados; repetir o PUT mantém um voto por usuário e filme. Em inserções simultâneas, a restrição única pode retornar 409; o cliente pode repetir o PUT.

Exemplos de listagem:

```http
GET /api/movies?director=Nolan&name=Inter&genre=fic&actor=Anne&page=1&pageSize=10&sort=Votes
GET /api/movies?sort=Title
GET /api/users?page=1&pageSize=20
```

Filtros são combinados por AND e buscam trechos dos textos. Sensibilidade a maiúsculas e acentos segue a collation do SQL Server. `sort=Votes` (padrão) ordena pela **quantidade** de votos decrescente, desempata por título e ID. `sort=Title` ordena por título e ID. Quantidade e média são retornadas separadamente; filme sem votos tem média zero. Atores duplicados são removidos após normalização de espaços e comparação sem distinguir maiúsculas.

Sem `page`, retorna todos os resultados; com `page`, usa páginas a partir de 1 e `pageSize` entre 1 e 100 (padrão 20). O envelope contém `items`, `total`, `page` e `pageSize`. Uma página além do fim retorna lista vazia com o total preservado.

Exclusão lógica mantém a conta e seu histórico de votos, que continua compondo as médias. Contas excluídas não aparecem nas consultas e não podem autenticar. E-mails continuam reservados após exclusão para impedir reutilização de identidade.

Erros usam ProblemDetails: 400 para validação, 401 para autenticação inválida, 403 para perfil sem permissão, 404 para registro ausente, 409 para conflito e 429 para limite de requisições. Consultas usam LINQ parametrizado; constraints de banco reforçam unicidade e notas de 0 a 4.

## Migrations

A migration inicial e o snapshot estão em `DESAFIO_ESTAGIO/Data/Migrations`. A criação do banco usa `MigrateAsync`, nunca `EnsureCreated` na aplicação. `EnsureCreated` é usado apenas nos testes SQLite isolados.

```powershell
dotnet tool restore
dotnet ef migrations list --project DESAFIO_ESTAGIO
dotnet ef migrations has-pending-model-changes --project DESAFIO_ESTAGIO
dotnet ef migrations script --idempotent --project DESAFIO_ESTAGIO -o migration.sql
dotnet ef database update --project DESAFIO_ESTAGIO
```

A factory de design time permite gerar scripts sem configurar JWT ou o bootstrap. `database update` exige uma connection string válida. Para futuras mudanças: altere o modelo e gere uma nova migration; preserve `InitialModel`, que congela o modelo inicial. Em produção, revise e aplique o script em uma etapa controlada.

## Testes e validação

```powershell
dotnet test DESAFIO_ESTAGIO.sln -c Release
```

- **Unitários:** limites e atualização de votos, validação dos contratos e JWT assinado com identidade, perfil e expiração.
- **Integração HTTP/SQLite:** Identity real, hash de senha, login, bloqueio, papéis, CRUD de administradores, exclusão lógica, revogação, paginação, filtros, médias, atualização de voto e documentação Swagger. SQLite é relacional, mas não reproduz todas as particularidades do SQL Server.
- **Modelo SQL Server:** verifica divergência entre snapshot/modelo e geração de script idempotente sem exigir conexão.
- **Integração SQL Server:** aplica a migration em banco de nome exclusivo, executa consultas e votação e valida a constraint das notas. Requer `TEST_SQLSERVER` com permissão para criar/excluir bancos. O banco de teste é excluído ao final. Sem a variável, somente esse teste é marcado como ignorado.

```powershell
$env:TEST_SQLSERVER = 'Server=localhost;User Id=sa;Password=SUA_SENHA;Encrypt=True;TrustServerCertificate=True'
dotnet test DESAFIO_ESTAGIO.sln -c Release
```

O workflow `.github/workflows/ci.yml` prepara .NET 8 e SQL Server 2022, compila, executa a suíte completa, armazena os resultados e constrói a imagem Docker.

**Estado de validação nesta entrega:** `docker compose config --quiet` passou, assim como a leitura dos arquivos JSON e XML dos projetos. Os testes foram adicionados, mas não puderam ser executados no ambiente de edição: há apenas runtime .NET, sem SDK; o download do SDK falhou por DNS e o acesso ao engine Docker foi negado. É necessário executar os comandos acima ou verificar o workflow antes de considerar a entrega validada.

## Entrega do desafio

Faça o fork do repositório original no GitHub, coloque estes arquivos no fork e execute a validação. Envie o link do fork pelo canal indicado pela equipe. Não é necessário abrir Pull Request. A pasta recebida neste ambiente não contém metadados Git nem URL do repositório original; o fork e o envio não foram realizados.

