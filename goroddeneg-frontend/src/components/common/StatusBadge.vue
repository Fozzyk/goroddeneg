<template>
  <span class="badge" :class="badgeClass">{{ label }}</span>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  status: { type: String, required: true }
})

const MAP = {
  Draft:     { label: 'Черновик',      cls: 'badge-muted'   },
  Pending:   { label: 'На модерации',  cls: 'badge-warning' },
  Active:    { label: 'Активный',      cls: 'badge-success' },
  Rejected:  { label: 'Отклонён',     cls: 'badge-danger'  },
  Completed: { label: 'Завершён',     cls: 'badge-info'    },
  Failed:    { label: 'Не состоялся', cls: 'badge-muted'   },
}

const badgeClass = computed(() => MAP[props.status]?.cls || 'badge-muted')
const label      = computed(() => MAP[props.status]?.label || props.status)
</script>
