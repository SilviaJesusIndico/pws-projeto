# 🚀 QUICKSTART - PetShop

## Iniciar Rapidamente

### Opção 1: Windows (PowerShell)

```powershell
# 1. Abrir Terminal
cd "C:\Users\Rafael Reis\OneDrive\Ambiente de Trabalho\petshop\PetShop"

# 2. Restaurar dependências
dotnet restore

# 3. Criar base de dados
dotnet ef database update

# 4. Executar
dotnet run

# 5. Aceder em: https://localhost:5001
```

### Opção 2: Linux/Mac (Bash)

```bash
cd ~/Desktop/petshop/PetShop
./run-petshop.sh
```

### Opção 3: Visual Studio

1. Abrir `PetShop.sln` no Visual Studio 2022
2. Menu: Build → Build Solution
3. Menu: Tools → NuGet Package Manager → Package Manager Console
4. Executar: `Update-Database`
5. Pressionar F5 ou clicar "Run"

---

## 📋 Próximos Passos Após Iniciar

### 1️⃣ Criar um Cliente
- Clicar em "Clientes" no menu
- Clicar "Criar Novo Cliente"
- Preencher formulário (Nome, Apelido, Email, Telefone)
- Guardar

### 2️⃣ Adicionar um Animal
- Clicar em "Animais" no menu
- Clicar "Criar Novo Animal"
- Selecionar cliente
- Preencher: Nome, Espécie, Raça, Peso, Data Nascimento
- Guardar

### 3️⃣ Definir Serviços
- Clicar em "Serviços" no menu
- Clicar "Criar Novo Serviço"
- Preencher: Nome, Descrição, Preço, Duração
- Guardar

### 4️⃣ Registar Técnicos
- Clicar em "Técnicos" no menu
- Clicar "Criar Novo Técnico"
- Preencher: Nome, Apelido, Email, Especialização, Data Contratação
- Guardar

### 5️⃣ Criar Agendamento
- Clicar em "Agendamentos" no menu
- Clicar "Criar Novo Agendamento"
- Selecionar: Animal, Serviço, Data/Hora, Técnico
- Guardar

---

## 🆘 Problemas Comuns

### ❌ "dotnet: comando não encontrado"
**Solução:** Descarregar .NET SDK de https://dotnet.microsoft.com/download

### ❌ "Connection string not found"
**Solução:** Verificar `appsettings.json` tem `"PetShopDatabase"`

### ❌ "Database file not found"
**Solução:** Executar `dotnet ef database update`

### ❌ "Port already in use (5001)"
**Solução:** Mudar porta em `launchSettings.json` ou matar processo anterior

### ❌ "Erro de validação no formulário"
**Solução:** Preencher todos os campos obrigatórios com dados válidos

---

## 📚 Documentação Completa

Consulte:
- `README.md` - Documentação técnica
- `APRESENTACAO.md` - Guia de apresentação
- `Models/*.cs` - Comentários detalhados nos modelos
- `Pages/*/*.cshtml.cs` - Comentários nos PageModels

---

## 🎯 Estrutura do Projeto

```
PetShop/
├── Models/                 ← Entidades de dados (com comentários)
├── Pages/                  ← Razor Pages (com comentários)
│   ├── Clientes/          ← CRUD de Clientes
│   ├── Animais/           ← CRUD de Animais
│   ├── Servicos/          ← CRUD de Serviços
│   ├── Agendamentos/      ← CRUD de Agendamentos
│   ├── Tecnicos/          ← CRUD de Técnicos
│   └── Shared/            ← Layouts e componentes
├── Data/                   ← DbContext (com comentários)
├── wwwroot/               ← CSS, JS, imagens
├── Program.cs             ← Configuração (com comentários)
├── README.md              ← Documentação
├── APRESENTACAO.md        ← Guia de apresentação
└── QUICKSTART.md          ← Este ficheiro
```

---

## 💡 Dicas Rápidas

✅ **Paginação**: Na página Index de Clientes, vê-se 5 clientes por página
✅ **Filtro**: Buscar clientes por nome ou email
✅ **Ordenação**: Clicar nas colunas para ordenar
✅ **Validação**: Todos os campos têm validação automática
✅ **Responsive**: Interface adapta-se a tablets e telemóveis

---

**Divirta-se com PetShop! 🐾**
