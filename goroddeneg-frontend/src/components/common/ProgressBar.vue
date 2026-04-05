<template>
  <div class="prog-wrap">
    <div
      class="progress"
      :class="[`progress-${size}`, { 'progress-animate': animate }]"
      :title="`${pct}% выполнено`"
    >
      <div
        class="progress-fill"
        :style="{ width: pct + '%', background: color || undefined }"
      />
    </div>
    <div v-if="showLabel" class="prog-label">
      <slot>{{ pct }}%</slot>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  collected: { type: Number, default: 0 },
  goal:      { type: Number, default: 0 },
  pctOverride: { type: Number, default: null },
  size:      { type: String, default: 'md' },  // sm | md | lg
  color:     { type: String, default: null },
  animate:   { type: Boolean, default: true },
  showLabel: { type: Boolean, default: false },
})

const pct = computed(() => {
  if (props.pctOverride !== null) return Math.min(100, props.pctOverride)
  if (!props.goal || props.goal === 0) return 0
  return Math.min(100, Math.round((props.collected / props.goal) * 100))
})
</script>

<style scoped>
.prog-wrap { width: 100%; }
.progress-sm { height: 4px; }
.progress-lg { height: 10px; }
.progress-animate .progress-fill { transition: width .7s cubic-bezier(.4,0,.2,1); }
.prog-label { font-size: 12px; color: var(--text-muted); margin-top: 5px; text-align: right; }
</style>
