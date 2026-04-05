<template>
  <div class="empty-state-wrap">
    <div class="es-icon">{{ icon }}</div>
    <h3 class="es-title">{{ title }}</h3>
    <p v-if="description" class="es-desc text-muted">{{ description }}</p>
    <slot />
  </div>
</template>

<script setup>
defineProps({
  icon:        { type: String, default: '📭' },
  title:       { type: String, default: 'Ничего не найдено' },
  description: { type: String, default: null },
})
</script>

<style scoped>
.empty-state-wrap { text-align: center; padding: 64px 24px; }
.es-icon  { font-size: 56px; margin-bottom: 18px; display: block; }
.es-title { font-family: 'Cormorant Garamond', serif; font-size: 1.4rem; margin-bottom: 10px; }
.es-desc  { font-size: 15px; line-height: 1.6; max-width: 360px; margin: 0 auto; }
</style>
