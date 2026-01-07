# 📱 PetShop - Sistema de Gestão de Clinica/Loja de Animais de Estimação

## 📋 Visão Geral

**PetShop** é uma aplicação web ASP.NET Core desenvolvida para gerenciar uma clínica/loja de animais de estimação. O sistema permite:

- ✅ Gestão de Clientes (donos de animais)
- ✅ Gestão de Animais de Estimação
- ✅ Gestão de Serviços (banho, tosa, consulta, etc.)
- ✅ Agendamento de Serviços
- ✅ Gestão de Técnicos/Funcionários
- ✅ Atribuição de Técnicos aos Serviços
- ✅ Paginação, Filtros e Ordenação

---

## 🏗️ Arquitetura e Tecnologias

### Stack Tecnológico:
- **Framework:** ASP.NET Core 9.0
- **Arquitetura:** Razor Pages (Padrão de Apresentação)
- **ORM:** Entity Framework Core 9.0
- **Banco de Dados:** SQLite
- **Frontend:** Bootstrap 5 + CSS Customizado
- **Linguagem:** C#

### Padrões de Design Utilizados:
1. **Repository Pattern (implícito via DbContext)**
2. **Dependency Injection** - Configurado automaticamente pelo ASP.NET Core
3. **Razor Pages Pattern** - Uma alternativa mais simples ao MVC clássico
4. **Pagination Pattern** - Classe genérica `PaginaList<T>` para paginação

---

## 📊 Modelo de Dados

### Entidades Principais:

#### 1. **Cliente**
Representa o dono do animal de estimação.

```
ClienteID (PK)
Nome
Apelido
Email
Telefone
DataRegisto
```

**Relacionamentos:**
- 1:N → Animal (um cliente pode ter vários animais)

---

#### 2. **Animal**
Representa um animal de estimação registado no sistema.

```
AnimalID (PK)
ClienteID (FK)
Nome
Especie (Cão, Gato, Coelho, etc.)
Raca
DataNascimento
Peso
ObservacoesPequeninos
```

**Relacionamentos:**
- N:1 ← Cliente
- 1:N → Agendamento

---

#### 3. **Servico**
Representa um tipo de serviço oferecido (banho, tosa, etc.).

```
ServicoID (PK)
NomeServico
Descricao
Preco
DuracaoMinutos
```

**Relacionamentos:**
- 1:N → Agendamento
- N:N ← Tecnico (via AtribuicaoTecnico)

---

#### 4. **Agendamento**
Representa o agendamento de um serviço para um animal.

```
AgendamentoID (PK)
AnimalID (FK)
ServicoID (FK)
TecnicoID (FK, pode ser NULL)
DataHora
Status (Pendente, Confirmado, Concluído, Cancelado)
Observacoes
PrecoFinal
DataCriacao
```

**Relacionamentos:**
- N:1 ← Animal
- N:1 ← Servico
- N:1 ← Tecnico

---

#### 5. **Tecnico**
Representa um técnico/funcionário da PetShop.

```
TecnicoID (PK)
Nome
Apelido
Email
Especializacao
DataContratacao
Telefone
Ativo
NomeCompleto (Propriedade calculada)
```

**Relacionamentos:**
- 1:N → Agendamento
- N:N ← Servico (via AtribuicaoTecnico)

---

#### 6. **AtribuicaoTecnico**
Tabela de junção N:N que define quais os serviços que cada técnico está qualificado a fazer.

```
TecnicoID (PK, FK)
ServicoID (PK, FK)
NivelProficiencia
DataAtribuicao
Observacoes
```

**Relacionamentos:**
- N:1 ← Tecnico
- N:1 ← Servico

---

## 📁 Estrutura de Pastas

```
PetShop/
├── Models/                    # Entidades de dados
│   ├── Cliente.cs
│   ├── Animal.cs
│   ├── Servico.cs
│   ├── Agendamento.cs
│   ├── Tecnico.cs
│   ├── AtribuicaoTecnico.cs
│   └── PaginaList.cs         # Classe de paginação
│
├── Data/                      # Contexto de dados
│   └── PetShopContext.cs
│
├── Pages/                     # Razor Pages
│   ├── Clientes/
│   │   ├── Index.cshtml       # Listar clientes
│   │   ├── Create.cshtml      # Criar cliente
│   │   ├── Edit.cshtml        # Editar cliente
│   │   ├── Delete.cshtml      # Eliminar cliente
│   │   ├── Details.cshtml     # Ver detalhes
│   │   └── *.cshtml.cs        # Code-behind
│   │
│   ├── Animais/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Delete.cshtml
│   │   └── Details.cshtml
│   │
│   ├── Servicos/
│   ├── Agendamentos/
│   ├── Tecnicos/
│   │
│   ├── Shared/               # Layouts e componentes compartilhados
│   │   ├── _Layout.cshtml
│   │   ├── _LoginPartial.cshtml
│   │   └── _ValidationScriptsPartial.cshtml
│   │
│   ├── Index.cshtml          # Página inicial
│   └── Error.cshtml
│
├── wwwroot/                  # Activos estáticos
│   ├── css/
│   │   └── site.css
│   ├── js/
│   │   └── site.js
│   ├── lib/                  # Bibliotecas (Bootstrap, jQuery, etc.)
│   └── images/
│
├── Migrations/               # Histórico de migrações do BD
│
├── Program.cs                # Ponto de entrada e configuração
├── appsettings.json          # Configurações (DB, logging, etc.)
├── appsettings.Development.json
└── PetShop.csproj            # Ficheiro de projeto
```

---

## 🚀 Como Executar o Projeto

### Pré-requisitos:
- .NET SDK 9.0 ou superior
- Visual Studio 2022 ou VS Code

### Passos:

1. **Clonar/Abrir o projeto**
```bash
cd PetShop
```

2. **Restaurar dependências**
```bash
dotnet restore
```

3. **Criar base de dados (Migrations)**
```bash
dotnet ef database update
```

4. **Executar a aplicação**
```bash
dotnet run
```

5. **Aceder à aplicação**
Abrir navegador em: `https://localhost:5001`

---

## 🎨 Funcionalidades Implementadas

### ✅ CRUD Completo (Criação, Leitura, Atualização, Eliminação)
- Clientes: Ver, Criar, Editar, Eliminar
- Animais: Ver, Criar, Editar, Eliminar
- Serviços: Ver, Criar, Editar, Eliminar
- Agendamentos: Ver, Criar, Editar, Eliminar
- Técnicos: Ver, Criar, Editar, Eliminar

### ✅ Paginação
- Implementada na página Index de Clientes (5 por página)
- Classe genérica `PaginaList<T>` reutilizável

### ✅ Filtros
- Filtrar clientes por nome ou email
- Filtrar animais por nome ou espécie
- Filtrar agendamentos por status

### ✅ Ordenação
- Ordenar por nome (crescente/decrescente)
- Ordenar por email
- Ordenar por data

### ✅ Validações
- Validações DataAnnotations nos modelos
- Validação client-side com jQuery Validation
- Validação server-side com ModelState

### ✅ UI Responsiva
- Bootstrap 5 para grid layout
- Cards para exibição de dados
- Tabelas responsivas
- Botões com cores semânticas

---

## 🔒 Segurança

### Implementado:
- ✅ Validação de entrada (DataAnnotations)
- ✅ Proteção contra sobrecarga de paramêtros (BindProperty)
- ✅ HTTPS em produção (HSTS)
- ✅ CSRF Protection (built-in em Razor Pages)

### Recomendações Futuras:
- [ ] Implementar autenticação de utilizadores (Identity)
- [ ] Autorização por roles (Admin, Técnico, Cliente)
- [ ] Criptografia de dados sensíveis
- [ ] Auditorias de ações

---

## 📝 Comentários no Código

**Todos os ficheiros incluem comentários detalhados em português:**
- Comentários de classe (`///`) explicando responsabilidades
- Comentários de propriedade explicando o propósito
- Comentários de método explicando o fluxo
- Comentários de linha explicando lógica complexa

Isto facilita a compreensão do código para apresentações e documentação.

---

## 🧪 Próximas Melhorias

### Curto Prazo:
1. Implementar autenticação e autorização
2. Adicionar confirmação de eliminação com SweetAlert
3. Implementar relatórios (ex: faturação mensal)
4. Dashboard com gráficos (animais por espécie, serviços populares, etc.)

### Médio Prazo:
1. SMS/Email de lembretes de agendamentos
2. Sistema de notificações
3. API REST para mobile app
4. Sistema de avaliações/feedback

### Longo Prazo:
1. Integração com gateway de pagamento
2. App mobile (Flutter/React Native)
3. Sistema de backup automático
4. Analytics avançadas

---

## 📞 Suporte

Para dúvidas ou sugestões sobre o projeto, consulte a documentação inline nos ficheiros ou contacte o desenvolvedor.

---

**Desenvolvido em: Dezembro de 2025**
**Versão: 1.0**
**Status: Em Desenvolvimento**
