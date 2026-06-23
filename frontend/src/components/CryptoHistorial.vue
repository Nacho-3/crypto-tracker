<script setup>
defineProps({
  transacciones: Array
});
const emit = defineEmits(['ver', 'editar', 'eliminar']);

const formatearFecha = (fechaStr) => {
  return new Date(fechaStr).toLocaleString();
};
</script>

<template>
  <div class="card">
    <h2>Historial de Operaciones</h2>
    <div class="table-container">
      <table>
        <thead>
          <tr>
            <th>Fecha</th>
            <th>Cripto</th>
            <th>Acción</th>
            <th>Cantidad</th>
            <th>Total (ARS)</th>
            <th class="text-center">Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="t in transacciones" :key="t.id">
            <td>{{ formatearFecha(t.datetime) }}</td>
            <td class="crypto-code">{{ t.cryptoCode }}</td>
            <td>
            <span :class="['badge', t.action === 'purchase' ? 'badge-success' : 'badge-danger']">
                {{ t.action === 'purchase' ? 'Compra' : 'Venta' }}
            </span>
            </td>
            <td class="font-mono">{{ Number(t.cryptoAmount).toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 6 }) }}</td>
            <td class="font-mono price">$ {{ t.money.toLocaleString() }}</td>
            <td class="text-center actions-cell">
              <button @click="emit('ver', t)" class="btn-text btn-ver">Ver</button>
              <button @click="emit('editar', t)" class="btn-text btn-editar">Editar</button>
              <button @click="emit('eliminar', t.id)" class="btn-text btn-borrar">Borrar</button>
            </td>
          </tr>
          <tr v-if="transacciones.length === 0">
            <td colspan="6" class="text-center empty-msg">No hay transacciones registradas.</td>
          </tr>
        </tbody>
      </table>
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
h2 {
  color: #333;
  margin-bottom: 20px;
}
.table-container {
  overflow-x: auto;
}
table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}
th {
  background-color: #f4f4f5;
  color: #555;
  padding: 12px;
  font-size: 14px;
  text-transform: uppercase;
}
td {
  padding: 12px;
  border-bottom: 1px solid #e4e4e7;
}
.crypto-code {
  font-weight: bold;
  text-transform: uppercase;
}
.font-mono {
  font-family: monospace;
}
.price {
  font-weight: 600;
}
.text-center { text-align: center; }
.badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: bold;
}
.badge-success { background: #dcfce7; color: #15803d; }
.badge-danger { background: #fee2e2; color: #b91c1c; }
.actions-cell {
  display: flex;
  justify-content: center;
  gap: 10px;
}
.btn-text {
  background: none;
  border: none;
  cursor: pointer;
  font-weight: 500;
  font-size: 14px;
}
.btn-ver { color: #4f46e5; }
.btn-editar { color: #d97706; }
.btn-borrar { color: #dc2626; }
.empty-msg { color: #71717a; padding: 20px; }
</style>