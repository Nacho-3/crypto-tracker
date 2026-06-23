<script setup>
import { ref, watch } from 'vue';

const props = defineProps({
  transaccionEditar: Object
});
const emit = defineEmits(['guardar', 'cancelar']);

const form = ref({
  cryptoCode: 'bitcoin',
  action: 'purchase',
  cryptoAmount: '',
  money: 0,
  datetime: new Date().toISOString().slice(0, 16)
});

watch(() => props.transaccionEditar, (newVal) => {
  if (newVal) {
    form.value = { ...newVal, datetime: newVal.datetime.slice(0, 16) };
  }
}, { immediate: true });

const enviarFormulario = () => {
  if (form.value.cryptoAmount <= 0) {
    alert("La cantidad debe ser mayor a 0.");
    return;
  }
  emit('guardar', form.value);
};
</script>

<template>
  <div class="card-form">
    <h2>{{ transaccionEditar?.id ? 'Editar Transacción' : 'Registrar Operación' }}</h2>
    
    <form @submit.prevent="enviarFormulario">
      <div class="form-group">
        <label>Criptomoneda</label>
        <select v-model="form.cryptoCode" :disabled="transaccionEditar?.id">
          <option value="btc">Bitcoin (BTC)</option>
          <option value="eth">Ethereum (ETH)</option>
          <option value="usdc">USD Coin (USDC)</option>
        </select>
      </div>

      <div class="form-group">
        <label>Tipo de Acción</label>
        <select v-model="form.action" :disabled="transaccionEditar?.id">
          <option value="purchase">Compra</option>
          <option value="sale">Venta</option>
        </select>
      </div>

      <div class="form-group">
        <label>Cantidad</label>
        <input type="number" step="any" v-model.number="form.cryptoAmount" required placeholder="Ej: 0.005">
      </div>

      <div class="form-group" v-if="transaccionEditar?.id">
        <label>Monto total de la operación (ARS)</label>
        <input type="number" step="any" v-model.number="form.money" required>
      </div>

      <div class="form-group">
        <label>Fecha y Hora</label>
        <input type="datetime-local" v-model="form.datetime" required>
      </div>

      <div class="actions">
        <button type="button" @click="emit('cancelar')" class="btn btn-cancelar">Cancelar</button>
        <button type="submit" class="btn btn-guardar">Guardar</button>
      </div>
    </form>
  </div>
</template>

<style scoped>
.card-form {
  background: white;
  padding: 24px;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
  max-width: 500px;
  margin: 0 auto;
}
h2 { color: #333; margin-bottom: 20px; }
.form-group {
  margin-bottom: 16px;
}
label {
  display: block;
  font-size: 14px;
  color: #555;
  margin-bottom: 6px;
  font-weight: 500;
}
input, select {
  width: 100%;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
  box-sizing: border-box;
  font-size: 14px;
}
input:disabled, select:disabled {
  background-color: #f4f4f5;
  color: #a1a1aa;
}
.actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
}
.btn {
  padding: 10px 16px;
  border: none;
  border-radius: 4px;
  font-weight: 500;
  cursor: pointer;
}
.btn-cancelar { background: #e4e4e7; color: #333; }
.btn-guardar { background: #4f46e5; color: white; }
</style>