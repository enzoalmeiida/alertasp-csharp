# ALERTASP — API de ocorrências de alagamento

Projeto da disciplina **C# Software Development — FIAP Global Solution 2026.2**. Esta entrega independente permite registrar e consultar ocorrências de alagamento em São Paulo. Ela não consome CGE, GeoSampa ou sensores e não apresenta relatos como dados em tempo real.

**Entrega geral informada pela equipe:** 19/11/2026 às 23h55. Confirme a tarefa da disciplina no Teams.

## Equipe

- Enzo Almeida — RM556900
- Gabriel Mello — RM554421
- Guilherme Moreira — RM557290

## Ficha CitizenTech

- **Território:** município de São Paulo; distrito e via são informados em cada ocorrência.
- **Cidadão beneficiado:** pessoa que precisa decidir por onde circular durante chuva intensa.
- **Problema:** falta uma consulta simples e localizada que indique a situação e o horário observado de registros de alagamento.
- **Solução desta entrega:** API para registrar, consultar e atualizar relatos, mantendo origem, horário observado e última atualização visíveis.
- **ODS relacionados:** ODS 11 e 13 como referências de cidades resilientes e ação climática; ODS 3 como benefício complementar de segurança. O protótipo não mede impacto nem prova redução de acidentes.

## Escopo e confiabilidade dos dados

O banco começa vazio. A API aceita três origens: `Citizen`, `PublicAgency` e `SyntheticDemo`. `PublicAgency` apenas identifica a origem informada pelo cliente; não significa integração ou verificação com um órgão. Use `SyntheticDemo` em demonstrações. Não envie nome, telefone, imagem de rosto ou outra informação pessoal. `ObservedAt` é quando o evento foi observado; `CreatedAt` e `UpdatedAt` são carimbos do sistema. Dados históricos do GeoSampa não são tratados como tempo real.

Antes da apresentação, a equipe deve observar um território e conversar com pessoas que convivem com o problema; este repositório não afirma que essa pesquisa já ocorreu. Confirme no Teams se a tarefa do professor acrescenta critérios.

## Requisitos

- .NET SDK 10.0.100 (ou SDK 10.x compatível), SQLite local e acesso ao NuGet no primeiro restore.
- Linux, macOS ou Windows com runtime ASP.NET Core compatível.

## Executar

Na raiz do repositório:

```bash
dotnet restore Alertasp.sln
dotnet run --project src/Alertasp.Api
```

Abra `http://localhost:5080/swagger` para explorar a documentação. A aplicação aplica as migrations ao iniciar e cria `alertasp.db` no diretório de execução. Reiniciar o servidor preserva os registros. Para parar, use Ctrl+C.

### Exemplo de demonstração sintética

```bash
curl -i -X POST http://localhost:5080/api/reports \
  -H 'Content-Type: application/json' \
  -d '{"roadName":"Avenida de demonstração","district":"Sé","latitude":-23.5505,"longitude":-46.6333,"observedAt":"2026-11-08T12:00:00Z","source":"SyntheticDemo"}'
```

O exemplo é sintético, não é ocorrência real e deve ser substituído por data/hora apropriada à demonstração. A resposta `201 Created` inclui o identificador e os horários do sistema.

## Endpoints

| Método e rota | Uso | Respostas principais |
| --- | --- | --- |
| `GET /api/reports?district=Sé&status=Reported` | Lista por distrito/situação, mais recentes primeiro | `200` |
| `GET /api/reports/{id}` | Consulta um registro | `200`, `404` |
| `POST /api/reports` | Cria um relato; não recebe dados pessoais | `201`, `400` |
| `PATCH /api/reports/{id}/status` | Atualiza para `Reported`, `Confirmed` ou `Cleared` | `200`, `400`, `404` |
| `GET /health` | Verifica processo HTTP | `200` |

Exemplo de corpo para mudança de situação: `{"status":"Confirmed"}`.

## Persistência e migrations

EF Core usa SQLite em `alertasp.db`; consultas aplicam filtro e ordenação via LINQ no banco, com `AsNoTracking` e `ToListAsync`. Os horários são persistidos em Unix milliseconds para que o SQLite ordene as ocorrências. O startup executa `Database.MigrateAsync()`. Para criar uma migration após alterar o modelo:

```bash
dotnet tool restore
dotnet build Alertasp.sln
dotnet tool run dotnet-ef migrations add NomeDaAlteracao --project src/Alertasp.Api --no-build
```

## Testes e critérios de aceite

```bash
dotnet test Alertasp.sln
```

Os testes unitários exercitam validação de coordenadas/horário, criação e situação inicial, e a atualização de um identificador ausente. Critérios para demonstração manual: Swagger abre; criação retorna `201`; consulta retorna o mesmo ID; filtros respeitam distrito/situação; status pode ser alterado; após reiniciar, o registro continua no SQLite; payload inválido retorna `400`; ID ausente retorna `404`.

## Como os critérios aparecem no código

| Critério | Peso | Implementação | Evidência verificada | Risco/pendência |
| --- | ---: | --- | --- | --- |
| POO | 3,0 | `FloodReport` encapsula invariantes e mudança de situação; enums e DTOs separam conceitos | Quatro testes unitários passam, incluindo coordenadas e horário | Validar se os estados cobrem a linguagem usada na aula e pelo cidadão |
| Web API | 2,5 | Controller REST, DTOs, validação, códigos HTTP e Swagger | Smoke test: Swagger 200; POST 201; GET/PATCH 200; inválido 400; ID ausente 404 | Sem autenticação por escopo; confirmar se a atividade pede algo adicional |
| EF Core + LINQ | 1,5 | SQLite persistente, repositório, filtros/ordenação LINQ e migration inicial | Filtro retornou o resultado esperado; registro sobreviveu ao reinício | Nenhuma fonte pública real está integrada; banco de protótipo local |
| Async/Await | 1,0 | Operações de persistência assíncronas com `CancellationToken` | Rotas funcionais exercitadas contra SQLite | Testes unitários isolam persistência; smoke test cobre o caminho real |
| Qualidade do código | 2,0 | DI por interfaces, handler global de erros, sem PII, README e prompts | Build sem warnings, 4 testes aprovados e auditoria NuGet sem vulnerabilidades conhecidas | Faltam pesquisa territorial comprovada, nomes da equipe e capturas de evidência |

## Dependências entre disciplinas

A entrega C# é executável por conta própria; depende de .NET, NuGet no primeiro restore e SQLite local. Não depende tecnicamente do Mobile, SOA, IA/ML, IoT ou Cybersecurity, embora todas compartilhem a definição do ALERTASP. Para Operating Systems, esta Web API é candidata a aplicação web hospedada em Linux, mas confirmem com o professor se a API sozinha atende ao enunciado ou se ele espera uma interface web. Não aleguem integração entre as entregas enquanto ela não existir.

## Etapas restantes e conclusão

1. Conferir no Teams se a tarefa acrescenta requisitos e confirmar com o professor a compatibilidade da API com Operating Systems.
2. Fazer observação territorial e conversas com cidadãos; registrar fonte, contexto e limitações sem guardar dados pessoais.
3. Executar os critérios de aceite acima com dados `SyntheticDemo`, capturar Swagger, respostas HTTP, testes e persistência após reinício.
4. Revisar README, adicionar integrantes e evidências verificáveis; cada integrante deve conseguir explicar os arquivos e as regras.
5. Simular a rubrica e fechar pendências antes de 19/11/2026 às 23h55.

## Estrutura

```text
src/Alertasp.Api/       API, domínio, aplicação, persistência e migrations
tests/Alertasp.Tests/   testes unitários
prompts.md              registro de uso de IA para revisão pela equipe
```

## Pendências antes de entregar

- Validar o escopo com a tarefa do Teams e a versão/ferramentas efetivamente usadas em aula.
- Fazer observação territorial e conversas com cidadãos; registrar evidências reais sem dados pessoais.
- Adicionar nomes dos integrantes, capturas da API/testes e evidências de persistência ao pacote de entrega.
- Rever os endpoints com a equipe e demonstrar que todos sabem explicar as regras e cada parte do código.
