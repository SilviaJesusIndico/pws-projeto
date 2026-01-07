#!/usr/bin/env bash
# ===========================================================
# SCRIPT: run-petshop.sh (para Linux/Mac) ou run-petshop.bat (para Windows)
# ===========================================================
# 
# Script para iniciar o projeto PetShop facilmente
# 
# Uso: ./run-petshop.sh (Linux/Mac) ou run-petshop.bat (Windows)
#
# ===========================================================

cd "$(dirname "$0")"

echo "========================================================"
echo "   🐾 PetShop - Sistema de Gestão"
echo "========================================================"
echo ""

# Verificar se .NET SDK está instalado
if ! command -v dotnet &> /dev/null; then
    echo "❌ ERRO: .NET SDK não encontrado!"
    echo "   Descarregue de: https://dotnet.microsoft.com/download"
    exit 1
fi

echo "✅ .NET SDK encontrado: $(dotnet --version)"
echo ""

# Restaurar dependências
echo "📦 Restaurando dependências NuGet..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "❌ Erro ao restaurar dependências!"
    exit 1
fi

# Criar base de dados
echo ""
echo "🗄️  Criando base de dados (Migrations)..."
dotnet ef database update --project .
if [ $? -ne 0 ]; then
    echo "❌ Erro ao criar base de dados!"
    echo "   Tente: dotnet tool install -g dotnet-ef"
    exit 1
fi

# Compilar
echo ""
echo "🔨 Compilando projeto..."
dotnet build
if [ $? -ne 0 ]; then
    echo "❌ Erro na compilação!"
    exit 1
fi

# Executar
echo ""
echo "🚀 Iniciando PetShop..."
echo "========================================================"
echo ""
echo "✅ Aplicação iniciada com sucesso!"
echo ""
echo "📱 Aceda em: https://localhost:5001"
echo ""
echo "🛑 Pressione CTRL+C para parar a aplicação"
echo "========================================================"
echo ""

dotnet run
