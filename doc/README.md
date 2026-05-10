# Documentação — Contos Interativos (IS)

Documentação técnica do projeto **Contos Interativos** (Interactive Stories).

## Estrutura

| Pasta | Finalidade |
| --- | --- |
| [`overview/`](overview/) | Visão geral do produto e descrição de funcionalidades |
| [`specs/`](specs/) | Especificações técnicas de componentes e funcionalidades implementadas |
| [`proposals/`](proposals/) | Propostas de novas funcionalidades ou alterações de arquitetura |
| [`guidelines/`](guidelines/) | Padrões e convenções de desenvolvimento adotadas no projeto |

## Índice

### Visão Geral

- [Descrição do Projeto](overview/project-description.md) — Funcionalidades da biblioteca, do leitor e estrutura de recursos

### Especificações

#### Biblioteca

- [Biblioteca](specs/library.md) — Páginas e componentes da UI de listagem e seleção de livros; modelo de dados

#### Leitor

- [Leitor](specs/reader.md) — Página `BookReader`, componente `Background`, sistema de eventos e animações

#### Parser Storybasic

- [Storybasic — Visão Geral](specs/parser/storybasic.md) — Formato de arquivo, sintaxe, modelo de navegação e arquitetura de parsing
- [Storybasic — Comando `background`](specs/parser/background.md) — Imagem de fundo, animações de transição, sub-comandos e eventos

### Proposals

- [0001 — Botões de Navegação do Leitor](proposals/0001-navegacao-leitor.md) — Home, Troféus, Som e Voltar sobrepostos ao leitor

### Diretrizes

- [Preferências de Código](guidelines/code-preferences.md) — Convenções de codificação C#/Blazor adotadas no projeto
