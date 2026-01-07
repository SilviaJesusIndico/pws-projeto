# 📊 GUIA DE APRESENTAÇÃO - PetShop

## 🎯 Objetivo da Apresentação

Apresentar um **Sistema de Gestão de PetShop** desenvolvido com:
- ASP.NET Core 9.0 (Framework web moderno)
- Entity Framework Core (ORM para acesso a dados)
- SQLite (Banco de dados leve)
- Bootstrap 5 (Design responsivo)
- Razor Pages (Padrão MVC simplificado)

---

## 📝 Roteiro da Apresentação

### 1. **INTRODUÇÃO (2 minutos)**

**O que é PetShop?**
- Sistema web de gestão para clínicas/lojas de animais de estimação
- Permite gerenciar: Clientes, Animais, Serviços, Agendamentos e Técnicos
- Todos os dados armazenados de forma segura em banco de dados

**Por que foi criado?**
- Demonstrar conhecimentos de ASP.NET Core e Entity Framework Core
- Aplicar padrões de desenvolvimento profissional
- Exemplificar CRUD completo com paginação, filtros e validação

---

### 2. **ARQUITETURA TÉCNICA (3 minutos)**

**Stack Tecnológico:**

```
┌─────────────────────────────────────┐
│         Camada de Apresentação      │
│      (Razor Pages + Bootstrap 5)    │
├─────────────────────────────────────┤
│      Camada de Lógica de Negócio    │
│    (PageModels com Razor Pages)     │
├─────────────────────────────────────┤
│       Camada de Acesso a Dados      │
│    (Entity Framework Core + DbSet)  │
├─────────────────────────────────────┤
│      Camada de Persistência         │
│       (SQLite Database)             │
└─────────────────────────────────────┘
```

**Padrões de Design Utilizados:**
- ✅ **Razor Pages Pattern**: Simplificação do MVC
- ✅ **Repository Pattern**: Via DbContext
- ✅ **Dependency Injection**: Container nativo do ASP.NET Core
- ✅ **Pagination Pattern**: Classe genérica `PaginaList<T>`

---

### 3. **MODELO DE DADOS (3 minutos)**

**Visualizar o diagrama ER:**

```
┌─────────────┐         ┌─────────────┐         ┌──────────────┐
│  CLIENTE    │────────▶│   ANIMAL    │────────▶│ AGENDAMENTO  │
│             │ 1:N     │             │ 1:N     │              │
│ ClienteID   │         │ AnimalID    │         │ AgendamentoID│
│ Nome        │         │ Especie     │         │ DataHora     │
│ Email       │         │ Raca        │         │ Status       │
│ Telefone    │         │ DataNasc.   │         └──────┬───────┘
└─────────────┘         └─────────────┘                │
                                                        │
┌─────────────┐         ┌──────────────┐                │
│  TECNICO    │◀────────│  SERVICO     │◀───────────────┘
│             │ N:N     │              │ N:1
│ TecnicoID   │         │ ServicoID    │
│ Nome        │         │ Nome         │
│ Email       │         │ Preco        │
│ Especial.   │         │ Duracao      │
└─────────────┘         └──────────────┘
    │
    │ (via AtribuicaoTecnico)
    │
    └──────────N:N────────────────────┘
```

**Relacionamentos:**
- `Cliente` 1:N `Animal` (um cliente tem vários animais)
- `Animal` 1:N `Agendamento` (um animal pode ter vários agendamentos)
- `Servico` 1:N `Agendamento` (um serviço tem vários agendamentos)
- `Tecnico` 1:N `Agendamento` (um técnico faz vários agendamentos)
- `Tecnico` N:N `Servico` (via AtribuicaoTecnico)

---

### 4. **DEMONSTRAÇÃO PRÁTICA (10 minutos)**

#### 4.1 Página Inicial (Home)
**Mostrar:**
- Design responsivo com Bootstrap
- Cards de navegação rápida para cada módulo
- Informações sobre o sistema

**Destacar:**
- Menu de navegação com acesso a todas as funcionalidades
- Página intuitiva e fácil de usar

#### 4.2 Módulo de Clientes
**Mostrar:**
1. **Index (Lista com Paginação)**
   - Lista de clientes com paginação (5 por página)
   - Filtro por nome/email
   - Ordenação por nome e email
   - Botões de ação (Editar, Detalhes, Eliminar)

2. **Criar Cliente**
   - Formulário com validação
   - Campos obrigatórios: Nome, Apelido, Email, Telefone
   - Validação de Email e Telefone

3. **Editar Cliente**
   - Pré-populado com dados existentes
   - Validação de modificações

4. **Detalhes do Cliente**
   - Mostrar informações completas
   - Listar animais do cliente
   - Links para editar/eliminar

5. **Eliminar Cliente**
   - Página de confirmação
   - Aviso sobre eliminação em cascata (animais também serão deletados)

#### 4.3 Módulo de Animais
**Mostrar:**
- Criar novo animal (deve estar ligado a um cliente)
- Editar dados do animal
- Validações: Especie, Raça, Peso, etc.

#### 4.4 Módulo de Serviços
**Mostrar:**
- Lista de serviços (Banho, Tosa, Consulta, etc.)
- Preços e duração
- Editar serviços

#### 4.5 Módulo de Agendamentos
**Mostrar:**
- Agendar serviço para um animal
- Seleção de data/hora
- Atribuição de técnico (se disponível)
- Status do agendamento

#### 4.6 Módulo de Técnicos
**Mostrar:**
- Lista de técnicos
- Especializações
- Data de contratação

---

### 5. **PONTOS-CHAVE DO CÓDIGO (3 minutos)**

**Explicar os seguintes aspetos:**

#### 5.1 Entity Framework Core
```csharp
// DbContext configurado em Program.cs
builder.Services.AddDbContext<PetShopContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("PetShopDatabase")
    )
);
```

#### 5.2 Relacionamentos com Delete Behavior
```csharp
// Exclusão em cascata: deletar cliente deleta seus animais
modelBuilder.Entity<Animal>()
    .HasOne(a => a.Cliente)
    .WithMany(c => c.Animais)
    .HasForeignKey(a => a.ClienteID)
    .OnDelete(DeleteBehavior.Cascade);
```

#### 5.3 Paginação Reutilizável
```csharp
// Classe genérica para paginação
var clientes = await PaginaList<Cliente>.CreateAsync(
    _context.Cliente.OrderBy(c => c.Nome),
    pageNumber: 1,
    pageSize: 5
);

// No template
<a asp-page="./Index" asp-route-indexPagina="@(Model.Clientes.PaginaIndex + 1)">Próximo</a>
```

#### 5.4 Validações DataAnnotations
```csharp
[Required(ErrorMessage = "O email é obrigatório")]
[EmailAddress(ErrorMessage = "Formato inválido")]
public string Email { get; set; }
```

---

### 6. **FUNCIONALIDADES AVANÇADAS (2 minutos)**

**Mostrar:**

1. **Validação Server-side e Client-side**
   - Validações DataAnnotations
   - jQuery Validation no browser
   - Erro exibido em tempo real

2. **Paginação Inteligente**
   - Mantém filtro ao navegar páginas
   - Mantém ordenação ao navegar
   - Desabilita botões "Anterior/Próximo" quando apropriado

3. **Integridade Referencial**
   - Não é possível deletar cliente enquanto tem animais
   - Cascata automática quando permitido

4. **Transações ACID**
   - SaveChangesAsync garante atomicidade
   - Rollback automático em caso de erro

---

### 7. **COMO EXECUTAR O PROJETO (2 minutos)**

**Passo a Passo:**

1. **Abrir Terminal**
   ```bash
   cd "C:\Users\Rafael Reis\OneDrive\Ambiente de Trabalho\petshop\PetShop"
   ```

2. **Restaurar Dependências**
   ```bash
   dotnet restore
   ```

3. **Criar Base de Dados (Migrations)**
   ```bash
   dotnet ef database update
   ```

4. **Rodar Aplicação**
   ```bash
   dotnet run
   ```

5. **Aceder em Navegador**
   ```
   https://localhost:5001
   ```

---

### 8. **ESTRUTURA DO CÓDIGO (2 minutos)**

**Mostrar ficheiros-chave:**

```
📁 Models/
  ├── Cliente.cs          (Comentários explicando responsabilidades)
  ├── Animal.cs           (Relacionamentos, Foreign Keys)
  ├── Servico.cs          (Preços, Duração)
  ├── Agendamento.cs      (Ligação entre Animal, Serviço, Técnico)
  ├── Tecnico.cs          (Funcionários, Especializações)
  └── AtribuicaoTecnico.cs (Relação N:N)

📁 Pages/
  ├── Clientes/
  │   ├── Index.cshtml.cs  (PageModel com lógica)
  │   ├── Create.cshtml.cs (Herança múltipla: CreateModel, EditModel, etc.)
  │   ├── Index.cshtml     (View com paginação e filtros)
  │   ├── Create.cshtml    (Formulário com validação)
  │   └── ...

📁 Data/
  └── PetShopContext.cs    (DbContext, OnModelCreating)

Program.cs                 (Configuração de serviços e pipeline)
```

---

### 9. **VANTAGENS DA TECNOLOGIA (2 minutos)**

**Por que ASP.NET Core?**
- ✅ **Segurança**: HTTPS, CSRF Protection, validações automáticas
- ✅ **Performance**: Compilado, otimizado, rápido
- ✅ **Escalabilidade**: Suporta milhões de usuários
- ✅ **Produtividade**: Framework completo, muito código automático
- ✅ **Comunidade**: Suporte oficial Microsoft, muitos recursos

**Vantagens de Entity Framework Core:**
- ✅ Abstração de BD (fácil mudar de SQLite para SQL Server)
- ✅ LINQ (queries type-safe)
- ✅ Migrations automáticas
- ✅ Lazy loading, eager loading
- ✅ Change tracking automático

**Vantagens de Razor Pages:**
- ✅ Mais simples que MVC completo
- ✅ Menos boilerplate
- ✅ Cada página é independente
- ✅ Perfeito para CRUD

---

### 10. **CONCLUSÕES (1 minuto)**

**Resumo:**
- Projeto funcional e pronto para uso
- Demonstra conhecimentos de desenvolvimento web profissional
- Aplicado padrões de design e best practices
- Código bem comentado e documentado
- Fácil de expandir e manter

**Possíveis Melhorias Futuras:**
- [ ] Autenticação e Autorização (Identity)
- [ ] Relatórios em PDF
- [ ] SMS/Email de lembretes
- [ ] Dashboard com gráficos
- [ ] API REST para mobile

---

## 🎤 Dicas de Apresentação

1. **Pratique antes**: Execute o projeto várias vezes
2. **Mostre com calma**: Deixe o público acompanhar
3. **Explique o "porquê"**: Não apenas o "como"
4. **Interaja**: Pergunte se têm dúvidas
5. **Tenha backup**: Prepare screenshots em caso de problema técnico
6. **Mostre o código**: Abra o IDE para mostrar a estrutura
7. **Destaque a validação**: Tente entrar dados inválidos

---

## 📊 Tempo Total: ~35 minutos

- Introdução: 2 min
- Arquitetura: 3 min
- Modelo de Dados: 3 min
- Demonstração: 10 min
- Pontos-Chave: 3 min
- Funcionalidades: 2 min
- Como Executar: 2 min
- Estrutura: 2 min
- Vantagens: 2 min
- Conclusões: 1 min
- **Total: 30 minutos (+ 5 min buffer para dúvidas)**

---

**Boa sorte na apresentação! 🍀**
