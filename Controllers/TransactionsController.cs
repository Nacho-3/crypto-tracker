using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoTrackerAPI.Data;
using CryptoTrackerAPI.Models;
using System.Net.Http.Json;

namespace CryptoTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public TransactionsController(AppDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient; // Uso HttpClient para pegarle a CriptoYa
        }

        // Endpoint GET: api/transactions (Historial completo ordenado)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetHistory()
        {
            var history = await _context.Transactions
                .OrderByDescending(t => t.Datetime)
                .ToListAsync();
            return Ok(history);
        }

        // Endpoint GET: api/transactions/{id} (Ver detalle de una sola)
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound(new { message = "Transacción no encontrada" });
            return Ok(transaction);
        }

        // Endpoint POST: api/transactions (Crear transacción con validación de CriptoYa)
        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction([FromBody] Transaction transaction)
        {
            transaction.CryptoCode = transaction.CryptoCode.ToLower();

            // Si es venta, valido que tenga fondos suficientes
            if (transaction.Action.ToLower() == "sale")
            {
                // Sumo lo comprado y resto lo vendido de esa cripto específica
                var totalPurchased = await _context.Transactions
                    .Where(t => t.CryptoCode == transaction.CryptoCode && t.Action == "purchase")
                    .SumAsync(t => t.CryptoAmount);

                var totalSold = await _context.Transactions
                    .Where(t => t.CryptoCode == transaction.CryptoCode && t.Action == "sale")
                    .SumAsync(t => t.CryptoAmount);

                decimal currentBalance = totalPurchased - totalSold;

                if (transaction.CryptoAmount > currentBalance)
                {
                    // Si intento vender más de lo que tengo, reboto la petición
                    return BadRequest(new { message = "Fondos insuficientes para realizar la venta." });
                }
            }

            // CONSULTA A API EXTERNA: Obtengo el precio en tiempo real
            string url = $"https://criptoya.com/api/satoshitango/{transaction.CryptoCode}/ars";

            try
            {
                var response = await _httpClient.GetFromJsonAsync<CriptoYaResponse>(url);
                if (response != null)
                {
                    // Si compro, pago el precio 'ask'. Si vendo, cobro el 'bid'
                    decimal price = transaction.Action.ToLower() == "purchase" ? response.Ask : response.Bid;

                    // Calculo el dinero total en ARS automáticamente en el backend
                    transaction.Money = transaction.CryptoAmount * price;
                }
            }
            catch
            {
                return StatusCode(500, new { message = "Error al conectar con el proveedor de precios (CriptoYa)." });
            }

            // Si todo está bien guardo
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }

        // Endpoint PATCH: api/transactions/{id} (Modificación directa del registro)
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] Transaction updatedData)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            if (updatedData.CryptoAmount > 0) transaction.CryptoAmount = updatedData.CryptoAmount;
            if (updatedData.Money > 0) transaction.Money = updatedData.Money;
            if (updatedData.Datetime != default) transaction.Datetime = updatedData.Datetime;
            if (!string.IsNullOrEmpty(updatedData.CryptoCode)) transaction.CryptoCode = updatedData.CryptoCode.ToLower();

            await _context.SaveChangesAsync();
            return Ok(transaction);
        }

        // Endpoint DELETE: api/transactions/{id} (Eliminación)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Transacción eliminada con éxito" });
        }

        // Endpoint GET: api/transactions/status 
        [HttpGet("status")]
        public async Task<IActionResult> GetWalletStatus()
        {
            var allTransactions = await _context.Transactions.ToListAsync();

            // Agrupo por criptomoneda
            var wallet = allTransactions
                .GroupBy(t => t.CryptoCode)
                .Select(g => new
                {
                    CryptoCode = g.Key,
                    // Calculo cantidad que posee el usuario (Compras - Ventas)
                    CurrentAmount = g.Where(t => t.Action == "purchase").Sum(t => t.CryptoAmount)
                                  - g.Where(t => t.Action == "sale").Sum(t => t.CryptoAmount)
                })
                .Where(w => w.CurrentAmount > 0) 
                .ToList();

            var result = new List<object>();
            decimal totalWalletValueARS = 0;

            foreach (var item in wallet)
            {
                // Vuelvo a consultar a CriptoYa para saber cuánto valen HOY esas monedas
                string url = $"https://criptoya.com/api/satoshitango/{item.CryptoCode}/ars";
                decimal currentPriceARS = 0;

                try
                {
                    var response = await _httpClient.GetFromJsonAsync<CriptoYaResponse>(url);
                    // Uso 'bid' porque es el valor al que el usuario podría vender su posición actual

                    if (response != null) currentPriceARS = response.Bid;
                }
                catch { }

                decimal totalValueInARS = item.CurrentAmount * currentPriceARS;
                totalWalletValueARS += totalValueInARS;

                result.Add(new
                {
                    crypto_code = item.CryptoCode,
                    amount = item.CurrentAmount,
                    current_price = currentPriceARS,
                    total_value_ars = totalValueInARS
                });
            }

            return Ok(new
            {
                assets = result,
                total_wallet_value_ars = totalWalletValueARS // Valor de toda la cartera sumada
            });
        }
    }
}