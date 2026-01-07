// ============================================================================
// SITE.JS - JavaScript Personalizado da Aplicação PetShop
// ============================================================================
//
// Este ficheiro contém código JavaScript customizado para a aplicação.
// É carregado em todas as páginas através do _Layout.cshtml.
//
// Bibliotecas disponíveis:
// - jQuery (carregado antes deste ficheiro)
// - Bootstrap 5.x (carregado antes deste ficheiro)
// - FontAwesome (ícones via classes CSS)
//
// Documentação:
// https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// ============================================================================

// ===== CÓDIGO EXECUTADO QUANDO O DOM ESTÁ PRONTO =====
$(document).ready(function() {
    
    // ----- Auto-esconder alertas de sucesso após 5 segundos -----
    // Os alertas .alert-success desaparecem automaticamente
    setTimeout(function() {
        $('.alert-success').fadeOut('slow');
    }, 5000);
    
    // ----- Confirmação antes de eliminar registos -----
    // Adiciona confirmação a qualquer botão/link com classe .btn-delete
    $('.btn-delete').on('click', function(e) {
        if (!confirm('Tem certeza que deseja eliminar este registo?')) {
            e.preventDefault();
            return false;
        }
    });
    
    // ----- Tooltips Bootstrap -----
    // Inicializa tooltips em elementos com data-bs-toggle="tooltip"
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
});

// ===== FUNÇÕES UTILITÁRIAS =====

/**
 * Formata um número como moeda portuguesa (euros)
 * @param {number} valor - O valor a formatar
 * @returns {string} - Valor formatado (ex: "25,00 €")
 */
function formatarEuros(valor) {
    return new Intl.NumberFormat('pt-PT', { 
        style: 'currency', 
        currency: 'EUR' 
    }).format(valor);
}

/**
 * Formata uma data no formato português
 * @param {Date|string} data - Data a formatar
 * @returns {string} - Data formatada (ex: "24/12/2025")
 */
function formatarData(data) {
    var d = new Date(data);
    return d.toLocaleDateString('pt-PT');
}

/**
 * Formata data e hora no formato português
 * @param {Date|string} dataHora - Data e hora a formatar
 * @returns {string} - Data e hora formatada (ex: "24/12/2025 14:30")
 */
function formatarDataHora(dataHora) {
    var d = new Date(dataHora);
    return d.toLocaleDateString('pt-PT') + ' ' + d.toLocaleTimeString('pt-PT', {hour: '2-digit', minute:'2-digit'});
}
