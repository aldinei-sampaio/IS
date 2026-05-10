# Spec: Leitor

O Leitor é a UI de leitura do capítulo de um livro, controlado pelo motor `IS.Reading` via storyboard.

## Página

### `BookReader` — `/book/{BookId}/read/{Chapter}`

Carrega o arquivo `.stbasic` do capítulo via `HttpClient`, faz o parse com `IStoryboardParser` e aguarda interação do usuário para avançar na narrativa.

Cada toque na área de leitura aciona `storyboard.MoveNextAsync()`. Ao atingir o fim do capítulo, o usuário é redirecionado de volta para `BookDetails`.

### Carregamento do capítulo

```
HttpClient.GetStringAsync("assets/books/{bookId}/chapters/{chapter}.stbasic")
  → DocumentLineReader
  → DocumentReader
  → IStoryboardParser.ParseAsync
  → IStoryboard (navegável)
```

Após o parse, `BookReader` chama `StateHasChanged()` seguido de `await Task.Yield()` para garantir que os componentes filhos renderizem e assinem os eventos do storyboard antes do primeiro `MoveAsync`.

## Componentes Implementados

### `Background`

Renderiza e anima a imagem de fundo ou cor sólida do leitor.

**Parâmetros:**

| Parâmetro | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `Storyboard` | `IStoryboard` | Sim | Instância do storyboard ativo |
| `BookId` | `string` | Sim | Identificador do livro; usado para montar a URL das imagens |

**Eventos assinados:**

| Evento | Handler | Descrição |
|---|---|---|
| `IBackgroundChangeEvent` | `HandleChange` | Atualiza tipo, nome e posição; executa animação de entrada |
| `IBackgroundScrollEvent` | `HandleScroll` | Anima scroll horizontal entre `Left` e `Right` |

Os handlers são `async Task` e aguardam a conclusão da animação antes de retornar, garantindo que o motor de navegação espere o fim de cada animação antes de disparar o próximo evento.

**Estado interno:**

| Campo | Tipo | Descrição |
|---|---|---|
| `type` | `BackgroundType` | `Undefined`, `Image` ou `Color` |
| `name` | `string` | Nome da imagem (sem extensão) ou valor de cor CSS |
| `displayPosition` | `BackgroundPosition` | `Left` ou `Right` |
| `isScrolling` | `bool` | Scroll em andamento — ativa classe `bg-scrolling` |
| `animationClass` | `string` | Classe CSS da animação de entrada ativa |
| `flashColor` | `string?` | Cor CSS do overlay de flash |
| `isFlashVisible` | `bool` | Overlay de flash presente no DOM |
| `isFlashFading` | `bool` | Overlay em fade-out (transition ativa) |
| `animationCts` | `CancellationTokenSource?` | Cancelamento de animações em andamento |

**Ciclo de vida:**
- Assinatura de eventos em `OnInitialized`
- Cancelamento de animações e desinscrição em `Dispose` (com `try/catch EventHandlerNotFoundException`)

### Animações de entrada (500 ms)

Disparadas por `IBackgroundChangeEvent.Animation` durante a navegação **forward**.

| Tipo | Efeito CSS |
|---|---|
| `None` | Substituição imediata — sem transição |
| `FadeIn` | Classe `bg-fadein`: keyframe `opacity 0→1`, easing `ease` |
| `Zoom` | Classe `bg-zoom`: keyframe `background-size 115%→100%`, easing `ease` |
| `Dissolve` | Classe `bg-dissolve`: keyframe `opacity 0→1`, easing `linear` |
| `Flash` | Overlay com `opacity 1`, aguarda 16 ms, transition `opacity→0` em 500 ms |

O flash usa a cor de `IBackgroundChangeEvent.FlashColor` (padrão: `white`). O overlay aparece instantaneamente (elementos novos no DOM não herdam transição CSS) e depois desfade.

Na navegação **backward**, o evento sempre chega com `Animation = None` e `FlashColor = null` — a restauração do fundo anterior é sempre imediata.

### Animação de scroll (1000 ms)

Disparada por `IBackgroundScrollEvent`. O handler aguarda 16 ms para que o browser pinte a posição atual antes de habilitar a classe `bg-scrolling` (`transition: background-position 1s ease`) e atualizar `displayPosition`.

Como os eventos do storyboard são disparados sequencialmente e os handlers são awaited, o scroll sempre inicia **após** a conclusão de qualquer animação de entrada que o preceda.

### URL das imagens de fundo

```
assets/books/{BookId}/background/{name}.jpg
```

A posição é controlada por `background-position: left center` ou `right center`, permitindo que imagens mais largas que a tela sejam posicionadas e animadas horizontalmente.

## Sistema de Eventos

O motor `IS.Reading` dispara eventos via `IEventManager`. Os handlers são `Func<TEvent, Task>` registrados com `Subscribe<T>` e removidos com `Unsubscribe<T>` (requer a mesma instância de delegate).

Eventos relevantes para o leitor:

| Evento | Payload principal |
|---|---|
| `IBackgroundChangeEvent` | `State` (nome/tipo/posição), `Animation`, `FlashColor` |
| `IBackgroundScrollEvent` | `Position` (destino do scroll) |

## Dependências

| Interface | Responsabilidade |
|---|---|
| `IAssetManager` | Resolve URLs de assets |
| `IStoryboardParser` | Faz parse do `.stbasic` em `IStoryboard` |
| `HttpClient` | Carrega o arquivo `.stbasic` do capítulo |
| `NavigationManager` | Redirecionamento ao fim do capítulo |
