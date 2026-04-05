// src/composables/useToast.js
import { ref, readonly } from 'vue'

const toasts = ref([])
let nextId = 0

export function useToast() {
  function add(message, type = 'info', duration = 4000) {
    const id = ++nextId
    toasts.value.push({ id, message, type })
    if (duration > 0) setTimeout(() => remove(id), duration)
    return id
  }

  function remove(id) {
    const idx = toasts.value.findIndex(t => t.id === id)
    if (idx !== -1) toasts.value.splice(idx, 1)
  }

  return {
    toasts: readonly(toasts),
    success: (msg, ms) => add(msg, 'success', ms),
    error:   (msg, ms) => add(msg, 'error',   ms),
    info:    (msg, ms) => add(msg, 'info',    ms),
    warning: (msg, ms) => add(msg, 'warning', ms),
    remove
  }
}
