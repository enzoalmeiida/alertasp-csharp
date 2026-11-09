# Registro de uso de IA

Use este arquivo para registrar prompts usados, respostas aproveitadas, revisões humanas e o que a equipe alterou. Não inclua dados pessoais, tokens ou conversas com informação sensível.

## Sessão de implementação inicial

- **Objetivo:** estruturar uma API C#/.NET pequena para registrar e consultar relatos de alagamento, cobrindo os critérios da página 30 do PDF FIAP CitizenTech.
- **Decisões revisadas pela equipe:** usar SQLite persistente; marcar explicitamente a origem e o horário observado; não afirmar integração com fontes em tempo real; não coletar identificação do relator.
- **Revisão necessária:** a equipe deve ler cada arquivo, executar a aplicação e os testes e confirmar os critérios do professor no Teams antes da entrega.

## Revisão e testes de integração

- **Objetivo:** aumentar a evidência da entrega C# com testes HTTP reais contra SQLite persistente, cobrindo filtros, validação, status, Swagger e recuperação após reiniciar o host.
- **Resultado:** 4 testes unitários e 2 testes de integração; a equipe deve executar os comandos do README e revisar o significado de cada cenário antes da entrega.
