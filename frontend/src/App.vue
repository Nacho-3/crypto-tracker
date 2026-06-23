<script setup>
import { ref, onMounted } from 'vue';
// 1. Importamos tus 3 componentes customizados
import CryptoHistorial from './components/CryptoHistorial.vue';
import CryptoFormulario from './components/CryptoFormulario.vue';
import CryptoEstado from './components/CryptoEstado.vue';

const API_BASE = "https://localhost:7019/api/transactions"; 

const vistaActual = ref('historial');
const transacciones = ref([]);
const transaccionSeleccionada = ref(null);

const estadoBilletera = ref({
  assets: [],
  total_wallet_value_ars: 0
});

// --- MÉTODOS DE CONEXIÓN CON EL BACKEND (FETCH) ---

// LLAMADO GET: Trae todo el historial de la API al cargar la página
const cargarHistorial = async () => {
  try {
    const res = await fetch(API_BASE);
    if (res.ok) {
      transacciones.value = await res.json();
    }
  } catch (error) {
    console.error("Error de red al conectar con .NET:", error);
  }
};

// LLAMADO POST / PATCH: Guarda o edita de forma cruda
const dectectarGuardar = async (formData) => {
  try {
    let url = API_BASE;
    let method = 'POST';
    
    // Si hay una transacción seleccionada previa, modificamos mediante PATCH
    if (transaccionSeleccionada.value) {
      url = `${API_BASE}/${transaccionSeleccionada.value.id}`;
      method = 'PATCH';
    }

    const res = await fetch(url, {
      method: method,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(formData)
    });

    if (res.ok) {
      alert(transaccionSeleccionada.value ? "Modificación cruda guardada correctamente." : "Operación registrada con éxito.");
      vistaActual.value = 'historial';
      cargarHistorial(); // Refrescamos la tabla
    } else {
      const err = await res.json();
      // Atrapa el error de saldo insuficiente que valida tu backend
      alert(`Error: ${err.message || "Operación rechazada por el servidor."}`);
    }
  } catch (error) {
    alert("No se pudo establecer comunicación con el backend.");
  }
};

// LLAMADO DELETE: Elimina un registro de la base de datos por ID
const eliminarTransaccion = async (id) => {
  if (confirm("¿Estás seguro de que deseas eliminar permanentemente esta transacción?")) {
    try {
      const res = await fetch(`${API_BASE}/${id}`, { method: 'DELETE' });
      if (res.ok) {
        cargarHistorial();
      }
    } catch (error) {
      alert("Error al intentar borrar el registro.");
    }
  }
};

// LLAMADO GET TO ENDPOINT /STATUS: Trae las cotizaciones de Criptoya y montos netos
const cargarEstadoActual = async () => {
  vistaActual.value = 'estado';
  try {
    const res = await fetch(`${API_BASE}/status`);
    if (res.ok) {
      estadoBilletera.value = await res.json();
    }
  } catch (error) {
    console.error("Error al calcular el estado financiero:", error);
  }
};

// --- CONTROLADORES DE INTERFAZ ---
const abrirFormularioNuevo = () => {
  transaccionSeleccionada.value = null;
  vistaActual.value = 'formulario';
};

const abrirFormularioEditar = (t) => {
  transaccionSeleccionada.value = t;
  vistaActual.value = 'formulario';
};

const verDetalle = (t) => {
  alert(`DETALLE DE LA OPERACIÓN\n\n` +
        `• Identificador: ${t.id}\n` +
        `• Moneda: ${t.cryptoCode.toUpperCase()}\n` +
        `• Acción: ${t.action === 'purchase' ? 'Compra' : 'Venta'}\n` +
        `• Cantidad: ${t.cryptoAmount}\n` +
        `• Total ARS: $${t.money.toLocaleString()}\n` +
        `• Fecha cargada: ${new Date(t.datetime).toLocaleString()}`);
};

// Hook de inicio: Pide las transacciones automáticamente al abrir la app
onMounted(() => {
  cargarHistorial();
});
</script>

<template>
  <div class="app-layout">
    <header class="main-header">
      <h1>Crypto Wallet Tracker</h1>
      <p class="subtitle">Trabajo Final (Vite + Vue 3) - Programación III</p>
    </header>

    <nav class="nav-tabs">
      <button @click="vistaActual = 'historial'" :class="{ active: vistaActual === 'historial' }">Historial</button>
      <button @click="abrirFormularioNuevo()" :class="{ active: vistaActual === 'formulario' }">Nueva Transacción</button>
      <button @click="cargarEstadoActual()" :class="{ active: vistaActual === 'estado' }">Estado Financiero</button>
    </nav>

    <main class="main-content">
      <CryptoHistorial v-if="vistaActual === 'historial'" :transacciones="transacciones" @ver="verDetalle" @editar="abrirFormularioEditar" @eliminar="eliminarTransaccion" />
      <CryptoFormulario v-if="vistaActual === 'formulario'" :transaccionEditar="transaccionSeleccionada" @guardar="dectectarGuardar" @cancelar="vistaActual = 'historial'" />
      <CryptoEstado v-if="vistaActual === 'estado'" :estadoBilletera="estadoBilletera" />
    </main>
  </div>
</template>

<style scoped>
.app-layout {
  max-width: 1200px;
  margin: 0 auto;
  padding: 40px 20px;
  box-sizing: border-box;
}

.main-header {
  text-align: center;
  margin-bottom: 35px;
}

.main-header h1 {
  color: #4f46e5;
  font-size: 2.5rem;
  margin: 0;
  font-weight: 800;
  letter-spacing: -0.05em;
}

.subtitle {
  color: #71717a;
  font-size: 1rem;
  margin-top: 8px;
  font-weight: 500;
}

.nav-tabs {
  display: flex;
  justify-content: center;
  gap: 12px;
  margin-bottom: 35px;
}

.nav-tabs button {
  padding: 12px 24px;
  border: none;
  background: white;
  color: #4b5563;
  font-weight: 600;
  font-size: 0.95rem;
  border-radius: 8px;
  cursor: pointer;
  box-shadow: 0 2px 4px rgba(0,0,0,0.04);
  transition: all 0.2s ease;
}

.nav-tabs button:hover {
  background: #f9fafb;
  color: #1f2937;
}

.nav-tabs button.active {
  background: #4f46e5;
  color: white;
  box-shadow: 0 4px 6px rgba(79, 70, 229, 0.2);
}

.main-content {
  margin-top: 20px;
}
</style>