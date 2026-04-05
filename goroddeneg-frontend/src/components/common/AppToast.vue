<template>
  <teleport to="body">
    <div class="toast-container">
      <transition-group name="toast">
        <div
          v-for="t in toasts"
          :key="t.id"
          class="toast"
          :class="`toast-${t.type}`"
          @click="remove(t.id)"
        >
          <span class="toast-icon">{{ icons[t.type] }}</span>
          <span class="toast-msg">{{ t.message }}</span>
          <button class="toast-close">✕</button>
        </div>
      </transition-group>
    </div>
  </teleport>
</template>

<script setup>
import { useToast } from '@/composables/useToast'
const { toasts, remove } = useToast()
const icons = { success: '✅', error: '❌', info: 'ℹ️', warning: '⚠️' }
</script>

<style scoped>
.toast-container {
  position: fixed; bottom: 24px; right: 24px;
  z-index: 9999; display: flex; flex-direction: column; gap: 10px;
  max-width: 380px; width: 100%;
}
.toast {
  display: flex; align-items: flex-start; gap: 12px;
  padding: 14px 16px; border-radius: var(--r-md);
  box-shadow: var(--shadow-lg); cursor: pointer;
  border: 1px solid; animation: slideInRight .25s ease;
  background: #fff;
}
.toast-success { border-color: rgba(42,122,80,.3);  background: var(--success-bg); color: var(--success); }
.toast-error   { border-color: rgba(192,57,43,.3);  background: var(--danger-bg);  color: var(--danger);  }
.toast-info    { border-color: rgba(26,94,168,.3);  background: var(--info-bg);    color: var(--info);    }
.toast-warning { border-color: rgba(160,112,0,.3);  background: var(--warning-bg); color: var(--warning); }

.toast-icon { font-size: 18px; flex-shrink: 0; }
.toast-msg  { flex: 1; font-size: 14px; font-weight: 500; line-height: 1.4; }
.toast-close { background: none; border: none; font-size: 14px; cursor: pointer; opacity: .6; padding: 0; color: inherit; }
.toast-close:hover { opacity: 1; }

@keyframes slideInRight { from { transform: translateX(100%); opacity: 0 } to { transform: translateX(0); opacity: 1 } }

.toast-enter-active { animation: slideInRight .25s ease; }
.toast-leave-active { animation: slideInRight .2s ease reverse; }
</style>
