# Roadmap — CardgameDungeon

Plano de desenvolvimento organizado em fases sequenciais. Cada fase tem objetivos claros, tecnologias envolvidas, tarefas principais e critérios de conclusão.

---

## Fase 1: Backend C# (Concluída)

**Objetivo:** Construir toda a lógica de negócio, domínio do jogo, features e API HTTP.

**Tecnologias:** C# / .NET 10, xUnit, Swagger

**Tarefas principais:**
- Modelagem do domínio: entidades Card, Deck, Match, Collection, Marketplace, Booster, Tournament, ELO
- Implementação de vertical slices para cada feature (Handler + Request + Response + Validation)
- Seed de 3000 cartas D&D distribuídas em 10 sets temáticos
- 28 endpoints HTTP com Swagger documentado
- Sistemas de meta: matchmaking, ELO, torneios, marketplace P2P, boosters
- 193 testes xUnit cobrindo regras críticas de negócio

**Critérios de conclusão:**
- [x] Todos os 193 testes passando
- [x] API funcional com todos os endpoints documentados
- [x] Seed completo de 3000 cartas em 10 sets
- [x] Regras de combate, iniciativa, custo e campo implementadas

---

## Fase 2: Unity Client (Em andamento)

**Objetivo:** Criar o cliente Unity que consome a API, com navegação funcional e fluxo básico de jogo.

**Tecnologias:** Unity 2022+, C#, UnityWebRequest, TextMeshPro

**Tarefas principais:**
- Estrutura de cenas: Login, Main Menu, Collection, Deck Builder, Match, Shop
- Sistema de API client para comunicação com o backend
- Renderização de cartas com dados vindos da API
- UI de construção de deck com validação de regras
- Tela de partida com campo de batalha, mão e dungeon rooms
- Navegação entre telas e gerenciamento de estado local
- HUD de match: HP, deck count, exile count, turno atual

**Critérios de conclusão:**
- [ ] 23 scripts integrados e funcionais
- [ ] Fluxo completo jogável: menu → deck → fila → partida → resultado
- [ ] Comunicação estável com todos os endpoints da API
- [ ] UI responsiva com feedback visual para ações do jogador

---

## Fase 3: Persistência (Concluída)

**Objetivo:** Substituir os repositórios em memória por banco de dados real com PostgreSQL e Entity Framework Core.

**Tecnologias:** PostgreSQL, EF Core 10, Npgsql, EF Migrations

**Tarefas principais:**
- Configurar DbContext com mapeamento de todas as entidades do domínio
- Criar migrations para o schema inicial (cards, decks, collections, matches, users, marketplace, tournaments)
- Implementar repositórios concretos substituindo os in-memory
- Seed do banco com as 3000 cartas via migration
- Configurar connection strings por ambiente (Development, Production)
- Adicionar health check para o banco de dados
- Testes de integração com banco real (Testcontainers)

**Critérios de conclusão:**
- [x] Todos os repositórios usando PostgreSQL via EF Core
- [x] Migrations reproduzíveis do zero até o estado atual
- [ ] Seed de cartas executado automaticamente na primeira migration
- [x] Testes existentes continuam passando com a nova camada de dados
- [x] Health check do banco disponível no endpoint /health

---

## Fase 4: Autenticação (Concluída)

**Objetivo:** Implementar autenticação segura com JWT e login via provedor externo, vinculando identidade ao perfil do jogador.

**Tecnologias:** JWT Bearer, BCrypt, System.IdentityModel.Tokens.Jwt

**Tarefas principais:**
- Criar entidade Player com Username, Email, PasswordHash, timestamps
- Implementar registro e login com username/senha (BCrypt hashing)
- Emissão e validação de JWT (access token com claims: PlayerId, Username, Email, Tier)
- Refresh token com rotação segura e revogação
- Proteger todos os endpoints existentes com RequireAuthorization
- Endpoints Auth públicos: register, login, refresh, revoke
- Swagger configurado com suporte a Bearer token
- Migration AddAuthEntities com tabelas Players e RefreshTokens

**Critérios de conclusão:**
- [x] Login funcional com username/senha
- [x] JWT emitido e validado em todos os endpoints protegidos
- [x] Refresh token com rotação segura
- [x] Perfil do jogador persistido e acessível via API
- [ ] Unity client autentica e mantém sessão entre cenas

---

## Fase 5: Multiplayer em Tempo Real (Concluída)

**Objetivo:** Implementar partidas ao vivo entre dois jogadores com sincronização de estado em tempo real.

**Tecnologias:** SignalR, WebSockets, JWT auth para hubs

**Tarefas principais:**
- MatchHub com JoinMatch, LeaveMatch, SendMatchAction e autenticação JWT
- IMatchNotifier interface no Features com 13 métodos de notificação
- MatchNotificationService envia notificações via IHubContext para grupos de match
- Todos os 11 handlers de Match/Combat atualizados para notificar em tempo real
- 12 eventos server→client: MatchStarted, TeamRevealed, InitiativeResolved, CombatAssigned, CombatResolved, RoomAdvanced, RoomConceeded, MatchFinished, BetPlaced, OpportunityAttackResolved, RetargetCompleted, SetupTeamSubmitted
- JWT via query string para conexões SignalR
- MatchHubClient.cs no Unity com callbacks para todos os eventos
- Validação server-side mantida — hub apenas notifica, lógica permanece nos handlers

**Critérios de conclusão:**
- [x] Hub SignalR configurado com autenticação JWT
- [x] Estado do match sincronizado entre servidor e ambos os clientes via notificações
- [x] Todos os handlers notificam em tempo real após cada ação
- [x] Unity client com MatchHubClient pronto para integração
- [ ] Timeout de turno aplicado automaticamente
- [ ] Reconexão funcional dentro do grace period

---

## Fase 6: Unity Polishing

**Objetivo:** Elevar a qualidade visual e sonora do cliente para nível de produto jogável.

**Tecnologias:** Unity Animator, Shader Graph, DOTween, FMOD ou Unity Audio, Spine/2D Animation

**Tarefas principais:**
- Arte de cartas: ilustrações para as cartas principais (ou sistema de arte procedural)
- Animações de combate: ataque, dano, morte, buff/debuff
- Efeitos visuais: partículas para habilidades, transições de dungeon room
- Sistema de áudio: música de fundo por cena, efeitos sonoros para ações
- UI final: menus polidos, tooltips, indicadores visuais de estado
- Feedback tátil: shake na câmera ao receber dano, highlight de cartas jogáveis
- Otimização de performance: atlas de sprites, object pooling, LOD
- Tela de loading e transições suaves entre cenas

**Critérios de conclusão:**
- [ ] Todas as cenas com arte e UI finais
- [ ] Animações de combate fluidas para todas as ações principais
- [ ] Áudio implementado em todas as cenas (música + SFX)
- [ ] Performance estável a 60fps em dispositivos alvo
- [ ] Nenhum placeholder visual restante na build

---

## Fase 7: Infraestrutura

**Objetivo:** Containerizar a aplicação, configurar deploy em cloud e pipeline de CI/CD.

**Tecnologias:** Docker, Docker Compose, GitHub Actions, Azure/AWS/GCP, Nginx, Let's Encrypt

**Tarefas principais:**
- Dockerfile para a API .NET
- Docker Compose com API + PostgreSQL + Redis (cache de sessão)
- Pipeline CI/CD com GitHub Actions: build → test → publish → deploy
- Deploy da API em cloud (Azure App Service, AWS ECS ou GCP Cloud Run)
- Banco de dados gerenciado em cloud (Azure Database for PostgreSQL ou equivalente)
- Configurar HTTPS com certificado válido
- Monitoramento e logging centralizado (Seq, Application Insights ou CloudWatch)
- Rate limiting e proteção básica contra abuso
- Backup automatizado do banco de dados

**Critérios de conclusão:**
- [ ] API rodando em container Docker em produção
- [ ] Pipeline CI/CD executando build e testes a cada push
- [ ] Deploy automático em staging; deploy em produção com aprovação manual
- [ ] HTTPS configurado com certificado válido
- [ ] Monitoramento ativo com alertas para erros críticos
- [ ] Backup diário automatizado do banco

---

## Fase 8: Beta

**Objetivo:** Testar o jogo com jogadores reais, coletar feedback e balancear cartas e sistemas.

**Tecnologias:** Analytics (Mixpanel/Amplitude), Feature Flags (LaunchDarkly ou similar), Ferramentas de feedback

**Tarefas principais:**
- Recrutar grupo de beta testers (20-50 jogadores)
- Implementar telemetria: winrate por carta, pick rate, match duration, ELO distribution
- Dashboard de analytics para acompanhar métricas de balanceamento
- Ciclos de balanceamento: ajustar stats de cartas com base nos dados coletados
- Calibrar K-factor do ELO e cutoffs dos tiers com dados reais
- Stress test do servidor com carga simulada
- Corrigir bugs reportados pelos beta testers
- Ajustar UX com base no feedback dos jogadores

**Critérios de conclusão:**
- [ ] Pelo menos 100 partidas jogadas por beta testers
- [ ] Winrate das cartas mais jogadas entre 45%-55%
- [ ] Distribuição de ELO saudável (curva normal)
- [ ] Nenhum bug crítico ou crash em aberto
- [ ] Servidor estável sob carga de pelo menos 50 partidas simultâneas
- [ ] Feedback dos testers endereçado ou documentado para futuro

---

## Fase 9: Launch

**Objetivo:** Lançar o jogo com todos os sistemas de monetização, economia e competição ativos.

**Tecnologias:** Payment gateway (Stripe), CDN, Store platforms (Steam/itch.io)

**Tarefas principais:**
- Integração com payment gateway para compra de moeda in-game
- Loja funcional: boosters com composição por raridade, preços definidos
- Marketplace P2P ativo: listagem, compra, taxa de 10%, reserva de cópias
- Torneios automáticos: brackets de 8 jogadores por tier, entrada paga, distribuição de prêmios
- Daily rewards funcionando com cap diário
- Página de landing / site do jogo
- Publicação na plataforma escolhida (Steam, itch.io ou web)
- Termos de serviço e política de privacidade
- Suporte ao jogador: canal de comunicação e FAQ

**Critérios de conclusão:**
- [ ] Compra de moeda in-game funcional com pagamento real
- [ ] Boosters compráveis e abrindo cartas corretamente
- [ ] Marketplace com listagens ativas e transações funcionando
- [ ] Torneios automáticos rodando diariamente
- [ ] Jogo publicado e acessível ao público
- [ ] Infraestrutura escalável para crescimento de jogadores
