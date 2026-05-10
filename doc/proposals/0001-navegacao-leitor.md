# 0001 — Botões de Navegação do Leitor e Controle de Áudio

**Status:** Aprovada

## Contexto

O leitor (`BookReader`) atualmente permite apenas navegação forward via toque em qualquer área da tela. Não há como retornar ao menu do livro, voltar um passo no storyboard ou controlar o áudio sem sair da página. Esta proposal define os botões de navegação sobrepostos ao leitor e o sistema de reprodução de música.

## Proposta

### Botões de navegação

Adicionar quatro botões ao leitor, exibidos em uma camada na frente de todos os outros componentes (incluindo os ainda não implementados). Os botões ficam posicionados na parte superior da área de leitura.

| Botão | Ícone | Posição | Ação |
| --- | --- | --- | --- |
| **Home** | Casa | Superior esquerdo | Navega para `/book/{BookId}` (BookDetails) |
| **Troféus** | Troféu | Superior (ao lado de Home) | *Placeholder — sem ação por enquanto* |
| **Som** | Alto-falante / mudo | Superior (ao lado de Troféus) | Alterna música ligada/desligada |
| **Voltar** | Seta para trás | Superior direito | Executa `storyboard.MovePreviousAsync()` |

#### Regras de visibilidade

Os botões são **ocultos por padrão** e aparecem nas seguintes situações:

1. **Timeout de inatividade:** o leitor está em estado de pausa e o usuário não toca em nada por mais de **5 segundos**.
2. **Toque na área superior:** o usuário toca na região superior do leitor (onde os botões ficam), independentemente do estado de pausa.

Ao aparecerem, os botões ficam visíveis por um período fixo (a definir na implementação) e se ocultam automaticamente. Qualquer toque fora da área dos botões reinicia o temporizador de ocultação.

#### Camada de renderização

Os botões devem ser renderizados com `z-index` superior ao de todos os outros componentes do leitor, ficando sempre acessíveis mesmo com balões, quadros de personagem ou outros elementos ativos.

### Controle de áudio

#### Reprodução de música

O sistema assina `IMusicChangeEvent` do storyboard. O evento expõe `MusicName` (string?):

- `MusicName != null` — iniciar (ou trocar para) a faixa `assets/books/{BookId}/music/{MusicName}.mp3`
- `MusicName == null` — parar a música em reprodução

O componente que assina `IMusicChangeEvent` armazena sempre o `MusicName` do último evento recebido, independentemente do estado do áudio.

Comportamento por estado:

| Estado do áudio | Evento recebido | Ação |
| --- | --- | --- |
| Ativo | `MusicName != null` | Inicia ou troca para a faixa indicada |
| Ativo | `MusicName == null` | Para a reprodução |
| Desativado | qualquer | Atualiza `MusicName` armazenado; não altera a reprodução |

Ao **reativar** o áudio:

- Se `MusicName` armazenado for `null`, nenhuma ação.
- Se `MusicName` armazenado não for `null` e for a **mesma faixa** que estava tocando antes de ser pausada, a reprodução **retoma do ponto onde parou**.
- Se `MusicName` armazenado não for `null` e for uma faixa **diferente** (ou não havia nada tocando), inicia do início.

#### Estado de áudio compartilhado

A preferência de áudio (ligado/desligado) é **compartilhada entre `BookDetails` e `BookReader`** e deve ser persistida, de modo que:

- Alterar o som em `BookDetails` reflita imediatamente ao entrar no leitor.
- Alterar o som em `BookReader` (botão Som) reflita ao voltar para `BookDetails`.

A persistência deve usar o mesmo mecanismo de armazenamento local adotado para o progresso de leitura (LocalStorage do navegador), sob uma chave global (não vinculada a um livro específico).

#### Serviço de áudio

Implementar `IAudioService` com as seguintes responsabilidades:

- Manter o estado de áudio ativo/inativo
- Iniciar, trocar e parar a reprodução de uma faixa
- Persistir e carregar a preferência de áudio do LocalStorage

O serviço deve ser registrado como **singleton** na DI do Blazor, garantindo instância única compartilhada entre páginas.

## Funcionalidade do botão Troféus

A lógica de exibição da lista de troféus será definida em uma ADR separada. Por ora, o botão deve existir visualmente mas não executar nenhuma ação ao ser clicado.

## Adendo — Visibilidade, Slide Counter e Indicador de Inatividade

**Data:** 2026-05-09

### Visibilidade dos botões de navegação

A regra de auto-ocultação é **removida**. Uma vez exibidos, os botões de navegação permanecem visíveis até que o usuário avance ou retroceda na história. Quando o storyboard é movido (para frente ou para trás), os botões voltam ao estado oculto e o temporizador de inatividade é reiniciado.

A tabela de gatilhos de exibição atualizada:

| Gatilho | Botões de navegação | Indicador de inatividade |
| --- | --- | --- |
| 5 s de inatividade | ✅ exibe | ✅ exibe |
| Toque na área superior | ✅ exibe | ❌ não exibe |
| Avanço ou retrocesso na história | ❌ oculta | ❌ oculta |

### Contador de slides

O campo `slideCount` do `BookReader` deve ser decrementado quando o storyboard for movido para trás (`MoveAsync(false)`). O comportamento é simétrico ao avanço.

O botão **Voltar** deve ser renderizado **desabilitado** enquanto `slideCount == 1`, impedindo que o usuário tente retroceder antes do primeiro slide.

### Botão "Clique aqui" (indicador de inatividade)

Adicionar um ícone animado na **parte inferior** da área de leitura com a função exclusiva de sinalizar ao usuário que o aplicativo está aguardando interação.

| Atributo | Valor |
| --- | --- |
| Posição | Inferior centralizado |
| Exibição | Junto com os botões de navegação, somente no gatilho de 5 s de inatividade |
| Animação | Pulsação ou bounce em loop, sugerindo "toque aqui" |
| Ação ao tocar | Idêntica ao toque no fundo do leitor (avança a história) |

O indicador **não aparece** quando o usuário toca na área superior para exibir os botões de navegação — nesse caso apenas os botões aparecem.

Por ser um indicador de interação pendente e não um controle de navegação, o indicador deve ficar em uma camada separada dos botões, com `z-index` equivalente para garantir que o toque seja capturado corretamente.

## Adendo — Visibilidade dos botões ao retroceder

**Data:** 2026-05-10

A regra definida no adendo anterior (avanço ou retrocesso oculta os botões) é **parcialmente revisada** para o caso de retrocesso.

Quando o usuário pressiona o botão **Voltar**, é comum que queira retroceder vários slides em sequência. Forçar a re-exibição dos botões a cada passo prejudica essa experiência.

### Nova regra para o botão Voltar

O retrocesso na história **não oculta os botões**. Eles permanecem visíveis durante toda a animação de transição e após sua conclusão, até que outro gatilho de ocultação ocorra.

A tabela de gatilhos atualizada:

| Gatilho | Botões de navegação | Indicador de inatividade |
| --- | --- | --- |
| 5 s de inatividade | ✅ exibe | ✅ exibe |
| Toque na área superior | ✅ exibe | ❌ não exibe |
| **Avanço** na história | ❌ oculta | ❌ oculta |
| **Retrocesso** na história | ✅ mantém visível | ❌ oculta |

### Implicação no temporizador

Ao retroceder, o temporizador de inatividade é **reiniciado** (como antes), mas os botões **não são ocultados** — eles permanecem visíveis independentemente do temporizador enquanto o usuário continuar interagindo via botão Voltar.

## Análise de Viabilidade

| Item | Viável | Observação |
| --- | --- | --- |
| Componente `ReaderControls` com overlay | ✅ | CSS `z-index` + Blazor, sem complexidade |
| Timer de 5 segundos | ✅ | `CancellationTokenSource` + `Task.Delay`, mesmo padrão já usado no `Background` |
| Toque na área superior | ✅ | Handler `@onclick` ou `@ontouchstart` na div da barra |
| Botão Home | ✅ | `NavigationManager.NavigateTo()` |
| Botão Voltar | ✅ | `storyboard.MovePreviousAsync()` já existe; convém verificar se o engine expõe um sinalizador de "sem histórico" para desabilitar o botão no início do capítulo |
| Botão Troféus (placeholder) | ✅ | Trivial |
| `IMusicChangeEvent` | ✅ | `MusicName: string?` já modelado no engine |
| Reprodução de mp3 | ✅ | Via JS interop — elemento `<audio>` do HTML5 |
| Retomar do ponto onde parou | ✅ | `audio.currentTime` salvo e restaurado via JS interop |
| Estado compartilhado singleton | ✅ | `IJSRuntime` é efetivamente singleton em WASM |
| Persistência no LocalStorage | ✅ | Já previsto no projeto para progresso de leitura |
| Política de autoplay do browser | ✅ | Usuário já interagiu com a tela antes de qualquer reprodução |

## Alternativas Consideradas

- **Gesto de swipe para voltar** — descartado por conflitar com o scroll horizontal do fundo.
- **Barra de navegação fixa sempre visível** — descartado por ocupar espaço de leitura permanentemente, prejudicando a imersão.
- **Botão Voltar sempre visível** — descartado por consistência; todos os controles de navegação seguem a mesma regra de visibilidade.
- **Estado de áudio via parâmetro de rota/query string** — descartado; a preferência é do usuário, não do livro, e deve sobreviver à navegação.
- **Aguardar o próximo evento de música ao reativar o som** — descartado; o usuário espera que a música retome imediatamente, sem precisar avançar na história para obter um novo evento.

## Impacto

- Novo componente Blazor (ex: `ReaderControls`) a ser incluído em `BookReader.razor`
- `BookReader` precisa expor o estado de pausa para que `ReaderControls` saiba quando iniciar o temporizador
- Novo serviço `IAudioService` / `AudioService` registrado como singleton
- `IAudioService` requer `IJSRuntime` e um módulo JS (`wwwroot/js/audioService.js`) para controlar o elemento `<audio>` do HTML5 (`play`, `pause`, `currentTime`)
- `BookDetails` e `BookReader` substituem o estado local de áudio pela referência ao `IAudioService`
- `BookReader` assina `IMusicChangeEvent` e delega reprodução ao `IAudioService`
- O botão **Voltar** chamará `storyboard.MovePreviousAsync()` — o componente precisa garantir que não seja chamado quando não há histórico de navegação (início do capítulo)
