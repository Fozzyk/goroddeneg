<template>
  <button
    class="btn"
    :class="[`btn-${variant}`, size && `btn-${size}`, { 'btn-full': fullWidth }]"
    :disabled="disabled || loading"
    v-bind="$attrs"
  >
    <span v-if="loading" class="spinner" style="width:16px;height:16px;border-width:2px" />
    <span v-if="!loading && icon" class="btn-icon-prefix">{{ icon }}</span>
    <span v-if="!loading"><slot /></span>
  </button>
</template>

<script setup>
defineProps({
  variant:   { type: String, default: 'primary' },  // primary|secondary|outline|ghost|danger
  size:      { type: String, default: null },         // sm|lg
  loading:   { type: Boolean, default: false },
  disabled:  { type: Boolean, default: false },
  fullWidth: { type: Boolean, default: false },
  icon:      { type: String, default: null },
})
</script>

<style scoped>
.btn-icon-prefix { font-size: 15px; }
</style>
