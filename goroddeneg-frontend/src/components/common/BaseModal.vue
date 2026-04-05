<template>
  <teleport to="body">
    <transition name="fade">
      <div v-if="modelValue" class="modal-overlay" @click.self="$emit('update:modelValue', false)">
        <div class="modal-box" :style="{ maxWidth: width }">
          <div class="modal-header">
            <div>
              <h2 v-if="title">{{ title }}</h2>
              <p v-if="subtitle" class="text-muted" style="margin-top:4px;font-size:14px">{{ subtitle }}</p>
            </div>
            <button class="modal-close" @click="$emit('update:modelValue', false)">✕</button>
          </div>
          <div class="modal-body">
            <slot />
          </div>
          <div v-if="$slots.footer" class="modal-footer">
            <slot name="footer" />
          </div>
        </div>
      </div>
    </transition>
  </teleport>
</template>

<script setup>
defineProps({
  modelValue: Boolean,
  title:      String,
  subtitle:   String,
  width:      { type: String, default: '500px' }
})
defineEmits(['update:modelValue'])
</script>

<style scoped>
.modal-footer {
  padding: 0 28px 24px;
  display: flex; gap: 10px; justify-content: flex-end;
  border-top: 1px solid var(--border); padding-top: 20px;
}
</style>
