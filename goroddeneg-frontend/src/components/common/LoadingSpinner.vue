<template>
  <div class="spinner-wrap" :class="{ fullpage }">
    <div class="spinner" :style="{ width: size, height: size }" />
    <p v-if="text" class="spinner-text text-muted">{{ text }}</p>
  </div>
</template>

<script setup>
defineProps({
  size:     { type: String,  default: '36px' },
  text:     { type: String,  default: null   },
  fullpage: { type: Boolean, default: false  },
})
</script>

<style scoped>
.spinner-wrap {
  display: flex; flex-direction: column;
  align-items: center; justify-content: center; gap: 14px;
}
.spinner-wrap.fullpage {
  min-height: 320px; width: 100%;
}
.spinner-text { font-size: 14px; }
</style>
