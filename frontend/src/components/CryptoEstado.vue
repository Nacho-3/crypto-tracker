<script setup>
defineProps({
  estadoBilletera: Object
});
</script>

<template>
  <div class="card">
    <h2>Estado Financiero en Tiempo Real</h2>
    <p class="subtitle">Valores actualizados consumiendo cotizaciones de mercado en vivo.</p>
    
    <div class="table-container">
      <table>
        <thead>
          <tr>
            <th>Criptomoneda</th>
            <th>Cantidad Disponible</th>
            <th>Precio de Mercado (ARS)</th>
            <th>Valor Estimado (ARS)</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="asset in estadoBilletera.assets" :key="asset.crypto_code">
            <td class="crypto-title">{{ asset.crypto_code }}</td>
            
            <td class="font-mono">
              {{ Number(asset.amount).toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 6 }) }}
            </td>
            
            <td class="font-mono">
              $ {{ Number(asset.current_price).toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }}
            </td>
            
            <td class="font-mono text-green">
              $ {{ Number(asset.total_value_ars).toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }}
            </td> 
          </tr>
          <tr v-if="!estadoBilletera.assets || estadoBilletera.assets.length === 0">
            <td colspan="4" class="text-center empty">No posees activos netos en cartera actualmente.</td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="total-box">
      <span>Monto Total de Dinero:</span>
      <span class="total-amount">
        $ {{ estadoBilletera.total_wallet_value_ars ? Number(estadoBilletera.total_wallet_value_ars).toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : '0,00' }}
      </span>
    </div>
  </div>
</template>
<style scoped>
.card {
  background: white;
  padding: 24px;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
}
h2 { color: #333; margin-bottom: 4px; }
.subtitle { color: #666; font-size: 14px; margin-bottom: 24px; }
table { width: 100%; border-collapse: collapse; text-align: left; }
th { background-color: #eef2ff; color: #4338ca; padding: 12px; font-size: 14px; text-transform: uppercase; border-bottom: 2px solid #c7d2fe; }
td { padding: 12px; border-bottom: 1px solid #e4e4e7; }
.crypto-title { font-weight: bold; text-transform: uppercase; color: #4f46e5; }
.font-mono { font-family: monospace; }
.text-green { color: #16a34a; font-weight: bold; }
.total-box {
  background: #4f46e5;
  color: white;
  padding: 20px;
  border-radius: 6px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 24px;
}
.total-amount { font-size: 24px; font-weight: bold; font-family: monospace; }
.empty { color: #71717a; padding: 20px; text-align: center; }
</style>